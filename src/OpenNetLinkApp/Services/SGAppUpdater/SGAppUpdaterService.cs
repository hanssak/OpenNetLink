using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

using Serilog;
using Serilog.Events;
using AgLogManager;

using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.Events;
using NetSparkleUpdater.Interfaces;
using NetSparkleUpdater.Downloaders;
using NetSparkleUpdater.Configurations;
using NetSparkleUpdater.SignatureVerifiers;
using NetSparkleUpdater.AppCastHandlers;
using NetSparkleUpdater.AssemblyAccessors;

using OpenNetLinkApp.Models.SGUserInfo;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Models.SGConfig;
using OpenNetLinkApp.Components.SGUpdate;


namespace OpenNetLinkApp.Services.SGAppUpdater
{
    /// <summary>
    /// Class to communicate with a sparkle-based appcast to download
    /// and install updates to an application
    /// </summary>
    public class SelfSparkleUpdater : NetSparkleUpdater.SparkleUpdater
    {
        #region Constructors

        /// <summary>
        /// ctor which needs the appcast url
        /// </summary>
        /// <param name="appcastUrl">the URL of the appcast file</param>
        /// <param name="signatureVerifier">the object that will verify your appcast signatures.</param>
        public SelfSparkleUpdater(string appcastUrl, NetSparkleUpdater.Interfaces.ISignatureVerifier signatureVerifier)
            : this(appcastUrl, signatureVerifier, null)
        { }

        /// <summary>
        /// ctor which needs the appcast url and a referenceassembly
        /// </summary>        
        /// <param name="appcastUrl">the URL of the appcast file</param>
        /// <param name="signatureVerifier">the object that will verify your appcast signatures.</param>
        /// <param name="referenceAssembly">the name of the assembly to use for comparison when checking update versions</param>
        public SelfSparkleUpdater(string appcastUrl, ISignatureVerifier signatureVerifier, string referenceAssembly)
            : this(appcastUrl, signatureVerifier, referenceAssembly, null)
        { }

        /// <summary>
        /// ctor which needs the appcast url and a referenceassembly
        /// </summary>        
        /// <param name="appcastUrl">the URL of the appcast file</param>
        /// <param name="signatureVerifier">the object that will verify your appcast signatures.</param>
        /// <param name="referenceAssembly">the name of the assembly to use for comparison when checking update versions</param>
        /// <param name="factory">a UI factory to use in place of the default UI</param>
        public SelfSparkleUpdater(string appcastUrl, ISignatureVerifier signatureVerifier, string referenceAssembly, IUIFactory factory)
                : base(appcastUrl, signatureVerifier, referenceAssembly, factory) { }

        #endregion

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SelfSparkleUpdater()
        {
            base.Dispose(false);
        }

        protected override string GetInstallerCommand(string downloadFilePath)
        {
            // get the file type
            string installerExt = Path.GetExtension(downloadFilePath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetWindowsInstallerCommand(downloadFilePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (DoExtensionsMatch(installerExt, ".pkg") ||
                    DoExtensionsMatch(installerExt, ".dmg"))
                {
                    return "open \"" + downloadFilePath + "\"";
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (DoExtensionsMatch(installerExt, ".deb"))
                {
                    if (IsDistrubID("TmaxOS"))
                    {
                        return "/system/bin/deb_installer \"" + downloadFilePath + "\"";
                    }
                    else
                    {
                        //return "sudo dpkg -i \"" + downloadFilePath + "\"";
                        return "gdebi-gtk \"" + downloadFilePath + "\"";
                    }
                }
                if (DoExtensionsMatch(installerExt, ".rpm"))
                {
                    return "sudo rpm -i \"" + downloadFilePath + "\"";
                }
            }
            return downloadFilePath;
        }

        // arg - id: HamoniKR, TmaxOS, Gooroom
        private bool IsDistrubID(string id)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return false;
            }

            string[] strLsbLines = System.IO.File.ReadAllLines("/etc/lsb-release");
            Dictionary<string, string> strLsbDic = new Dictionary<string, string>();
            foreach (string strLsbLine in strLsbLines)
            {
                string[] strLsbWords = strLsbLine.Split('=');
                strLsbDic[strLsbWords[0]] = strLsbWords[1];
            }

            LogWriter.PrintMessage("Get lsb-release in DISTRIB_ID : {0}", strLsbDic["DISTRIB_ID"]);
            if (0 == String.Compare(id.ToLower(), strLsbDic["DISTRIB_ID"].ToLower()))
            {
                return true;
            }
            return false;
        }

        private bool IsZipDownload(string downloadFilePath)
        {
            string installerExt = Path.GetExtension(downloadFilePath);
            bool isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            bool isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            if ((isMacOS && DoExtensionsMatch(installerExt, ".zip")) ||
                (isLinux && downloadFilePath.EndsWith(".tar.gz")))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the application via the file at the given path. Figures out which command needs
        /// to be run, sets up the application so that it will start the downloaded file once the
        /// main application stops, and then waits to start the downloaded update.
        /// </summary>
        /// <param name="downloadFilePath">path to the downloaded installer/updater</param>
        /// <returns>the awaitable <see cref="Task"/> for the application quitting</returns>
        protected override async Task RunDownloadedInstaller(string downloadFilePath)
        {
            LogWriter.PrintMessage("Running downloaded installer");
            // get the commandline 
            string cmdLine = Environment.CommandLine;
            string workingDir = Utilities.GetFullBaseDirectory();

            // generate the batch file path
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            bool isMacOS = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

            var extension = isWindows ? ".cmd" : ".sh";
            string batchFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + extension);
            string installerCmd;
            try
            {
                installerCmd = GetInstallerCommand(downloadFilePath);
                if (!string.IsNullOrEmpty(CustomInstallerArguments))
                {
                    installerCmd += " " + CustomInstallerArguments;
                }
            }
            catch (InvalidDataException)
            {
                UIFactory?.ShowUnknownInstallerFormatMessage(downloadFilePath);
                return;
            }

            // generate the batch file                
            LogWriter.PrintMessage("Generating batch in {0}", Path.GetFullPath(batchFilePath));

            string processID = Process.GetCurrentProcess().Id.ToString();

            using (StreamWriter write = new StreamWriter(batchFilePath, false, new UTF8Encoding(false)))
            {
                if (isWindows)
                {
                    write.WriteLine("@echo off");
                    // We should wait until the host process has died before starting the installer.
                    // This way, any DLLs or other items can be replaced properly.
                    // Code from: http://stackoverflow.com/a/22559462/3938401
                    string relaunchAfterUpdate = "";
                    if (RelaunchAfterUpdate)
                    {
                        relaunchAfterUpdate = $@"
                        cd {workingDir}
                        {cmdLine}";
                    }

                    string output = $@"
                        set /A counter=0                       
                        setlocal ENABLEDELAYEDEXPANSION
                        :loop
                        set /A counter=!counter!+1
                        if !counter! == 90 (
                            goto :afterinstall
                        )
                        tasklist | findstr ""\<{processID}\>"" > nul
                        if not errorlevel 1 (
                            timeout /t 1 > nul
                            goto :loop
                        )
                        :install
                        {installerCmd}
                        {relaunchAfterUpdate}
                        :afterinstall
                        endlocal";
                    write.Write(output);
                    write.Close();
                }
                else
                {
                    // We should wait until the host process has died before starting the installer.
                    var waitForFinish = $@"
                        COUNTER=0;
                        while ps -p {processID} > /dev/null;
                            do sleep 1;
                            COUNTER=$((++COUNTER));
                            if [ $COUNTER -eq 90 ] 
                            then
                                exit -1;
                            fi;
                        done;
                    ";
                    string relaunchAfterUpdate = "";
                    if (RelaunchAfterUpdate)
                    {
                        relaunchAfterUpdate = $@"{Process.GetCurrentProcess().MainModule.FileName}";
                    }
                    if (IsZipDownload(downloadFilePath)) // .zip on macOS or .tar.gz on Linux
                    {
                        // waiting for finish based on http://blog.joncairns.com/2013/03/wait-for-a-unix-process-to-finish/
                        // use tar to extract
                        var tarCommand = isMacOS ? $"tar -x -f {downloadFilePath} -C \"{workingDir}\""
                            : $"tar -xf {downloadFilePath} -C \"{workingDir}\" --overwrite ";
                        var output = $@"
                            {waitForFinish}
                            {tarCommand}
                            {relaunchAfterUpdate}";
                        output.Replace(@"\r\n", System.Environment.NewLine);
                        write.Write(output);
                    }
                    else
                    {
                        string installerExt = Path.GetExtension(downloadFilePath);
                        if (DoExtensionsMatch(installerExt, ".pkg") ||
                            DoExtensionsMatch(installerExt, ".dmg"))
                        {
                            relaunchAfterUpdate = ""; // relaunching not supported for pkg or dmg downloads
                        }
                        var output = $@"
                            {waitForFinish}
                            {installerCmd}
                            {relaunchAfterUpdate}";
                        output.Replace(@"\r\n", System.Environment.NewLine);
                        write.Write(output);
                    }
                    write.Close();
                    try
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo("chmod");
                        startInfo.WindowStyle = ProcessWindowStyle.Normal;
                        startInfo.ArgumentList.Add("755");
                        startInfo.ArgumentList.Add(batchFilePath);
                        // Run the external process & wait for it to finish
                        using (Process proc = Process.Start(startInfo))
                        {
                            proc.WaitForExit();
                        }
                    }
                    catch (Exception e)
                    {
                        LogWriter.PrintMessage($"Got Exception: execute (chmod) => {e}");
                    }
                }
            }

            // report
            LogWriter.PrintMessage("Going to execute script at path: {0}", batchFilePath);

            // init the installer helper
            if (isWindows)
            {
                _installerProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = batchFilePath,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
            }
            else
            {
                _installerProcess = new Process
                {
                    StartInfo =
                    {
                        FileName = "/bin/bash",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = batchFilePath,
                        UseShellExecute = true,
                        CreateNoWindow = true
                    }
                };
            }
            // start the installer process. the batch file will wait for the host app to close before starting.
            _installerProcess.Start();
            await QuitApplication();
        }
    }

    /// <summary>
    /// A simple class to handle log information for NetSparkleUPdater.
    /// Make sure to do any setup for this class that you want
    /// to do before calling StartLoop on your SparkleUpdater object.
    /// </summary>
    internal class SGAppUpdaterLogWriter : NetSparkleUpdater.Interfaces.ILogger
    {
        /// <summary>
        /// Base Log Instance for SGAppUpdaterService.
        /// </summary>
        private Serilog.ILogger CLog => Serilog.Log.ForContext<SGAppUpdaterLogWriter>();

        /// <summary>
        /// Tag to show before any log statements
        /// </summary>
        public static string tag = "netsparkle:";
        /// <summary>
        /// Empty constructor -> sets PrintDiagnosticToConsole to false
        /// </summary>
        public SGAppUpdaterLogWriter()
        {
            PrintDiagnosticToConsole = false;
        }

        /// <summary>
        /// LogWriter constructor that takes a bool to determine
        /// the value for printDiagnosticToConsole
        /// </summary>
        /// <param name="printDiagnosticToConsole">Whether this object should print via Debug.WriteLine or Console.WriteLine</param>
        public SGAppUpdaterLogWriter(bool printDiagnosticToConsole)
        {
            PrintDiagnosticToConsole = printDiagnosticToConsole;
        }

        #region Properties

        /// <summary>
        /// True if this class should print to Console.WriteLine;
        /// false if this object should print to Debug.WriteLine.
        /// Defaults to false.
        /// </summary>
        public bool PrintDiagnosticToConsole { get; set; }

        #endregion

        /// <inheritdoc/>
        public virtual void PrintMessage(string message, params object[] arguments)
        {
            if (PrintDiagnosticToConsole)
            {
                Console.WriteLine(tag + " " + message, arguments);
            }
            else
            {
                Log.Information(tag + " " + message, arguments);
            }
        }
    }

    public interface ISGAppUpdaterService
    {
        /* To Manage Updater, NetSparkle Instance */
        /// <summary>
        /// Declared: The Instance of NetSparkle for Updater Service
        /// </summary>
        SparkleUpdater SparkleInst { get; }

        /* To Save the gathered Update Info */
        /// <summary>
        /// Declared: To Save and Use that Gathered all Update Info from Update Server, via CheckUpdates
        /// </summary>
        UpdateInfo UpdateInfo { get; }

        /* To Save Downloaded Package File */
        /// <summary>
        /// Declared: To Save Downloaded Package File from Update Server, via DownloadUpdates
        /// </summary>
        string DownloadPath { get; }

        bool IsCancelRequested { get; set; }
        bool IsCanceled { get; set; }

        /* To Function Features */
        void Init(string updateSvcIP, string updatePlatform);
        void CheckUpdatesClick(SGCheckUpdate sgCheckUpdate = null,
                                SGAvailableUpdate sgAvailableUpdate = null,
                                SGDownloadUpdate sgDownloadUpdate = null,
                                SGFinishedDownload sgFinishedDownload = null,
                                SGMessageNotification sgMessageNotification = null);
        void DownloadUpdateClick();
        void SkipUpdateClick(AppCastItem CurrentItem);
        void CBDownloadMadeProgress(object sender, AppCastItem item, ItemDownloadProgressEventArgs e);
        void CBDownloadError(AppCastItem item, string path, Exception exception);
        void CBStartedDownloading(AppCastItem item, string path);
        void CBFinishedDownloading(AppCastItem item, string path);
        void CBDownloadCanceled(AppCastItem item, string path);
        void InstallUpdateClick();
        void CBCloseApplication();
        void DownloadUpdateBackground();
        void CBFullUpdateUpdateDetected(object sender, UpdateDetectedEventArgs e);
        void CBFullUpdateStartedDownloading(AppCastItem item, string path);
        void CBFullUpdateDownloadFileIsReady(AppCastItem item, string downloadPath);
        void CBFullUpdateCloseApplication();
        /// <summary>
        /// 별도 팝업 없이 자동 업데이트
        /// </summary>
        void CheckUpdateBackgroundDown();
    }
    internal class SGAppUpdaterService : ISGAppUpdaterService
    {
        private Serilog.ILogger CLog => Serilog.Log.ForContext<SGAppUpdaterService>();
        public SGAppUpdaterService() { }
        public void Init(string updateSvcIP, string updatePlatform)
        {
            CLog.Here().Information($"- AppUpdaterService Initializing... : [UpdateSvcIP({updateSvcIP}), UpdatePlatform({updatePlatform})]");
            //SparkleInst = new SparkleUpdater($"https://{updateSvcIP}/NetSparkle/files/sample-app/appcast.xml", new DSAChecker(SecurityMode.Strict))
            SparkleInst = new SelfSparkleUpdater($"https://{updateSvcIP}/updatePlatform/{updatePlatform}/appcast.xml",
                                                new Ed25519Checker(SecurityMode.Strict, null, "wwwroot/conf/Sparkling.service"))
            {
                UIFactory = null,
                AppCastDataDownloader = new WebRequestAppCastDataDownloader(),
                RelaunchAfterUpdate = true,
            };

            // TLS 1.2 required by GitHub (https://developer.github.com/changes/2018-02-01-weak-crypto-removal-notice/)
            SparkleInst.SecurityProtocolType = System.Net.SecurityProtocolType.Tls12;
            (SparkleInst.AppCastDataDownloader as WebRequestAppCastDataDownloader).TrustEverySSLConnection = true;
            SparkleInst.LogWriter = new SGAppUpdaterLogWriter();

            CLog.Here().Information($"- AppUpdaterService Initializing...Done : [UpdateSvcIP({updateSvcIP}), UpdatePlatform({updatePlatform})]");
        }

        /* To Manage Updater, NetSparkle Instance */
        public SparkleUpdater SparkleInst { get; private set; } = null;

        /* To Save the gathered Update Info */
        public UpdateInfo UpdateInfo { get; private set; } = null;

        /* To Save Downloaded Package File */
        public string DownloadPath { get; private set; } = string.Empty;
        public bool IsCancelRequested { get; set; } = false;
        public bool IsCanceled { get; set; } = false;
        public SGCheckUpdate CheckUpdate { get; private set; } = null;
        public SGAvailableUpdate AvailableUpdate { get; private set; } = null;
        public SGDownloadUpdate DownloadUpdate { get; private set; } = null;
        public SGFinishedDownload FinishedDownload { get; private set; } = null;
        public SGMessageNotification MessageNotification { get; private set; } = null;

        /* To Function Features */
        public async void CheckUpdatesClick(SGCheckUpdate sgCheckUpdate = null,
                                            SGAvailableUpdate sgAvailableUpdate = null,
                                            SGDownloadUpdate sgDownloadUpdate = null,
                                            SGFinishedDownload sgFinishedDownload = null,
                                            SGMessageNotification sgMessageNotification = null)
        {
            CheckUpdate = sgCheckUpdate;
            AvailableUpdate = sgAvailableUpdate;
            DownloadUpdate = sgDownloadUpdate;
            FinishedDownload = sgFinishedDownload;
            MessageNotification = sgMessageNotification;

            CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ Checking for updates... ]");
            CheckUpdate?.OpenPopUp();
            UpdateInfo = await SparkleInst.CheckForUpdatesQuietly();
            await Task.Delay(1000);

            await Task.Run(() =>
            {
                CheckUpdate?.ClosePopUp();
                // use _sparkle.CheckForUpdatesQuietly() if you don't want the user to know you are checking for updates!
                // if you use CheckForUpdatesAtUserRequest() and are using a UI, then handling things yourself is rather silly
                // as it will show a UI for things
                if (UpdateInfo != null)
                {
                    switch (UpdateInfo.Status)
                    {
                        case UpdateStatus.UpdateAvailable:
                            CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ There's an update available! ]");
                            AvailableUpdate?.OpenPopUp(SparkleInst, UpdateInfo.Updates);
                            break;
                        case UpdateStatus.UpdateNotAvailable:
                            CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ There's no update available :( ]");
                            MessageNotification?.OpenPopUp("There's no update available :(");
                            break;
                        case UpdateStatus.UserSkipped:
                            CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ The user skipped this update! ]");
                            MessageNotification?.OpenPopUp("The user skipped this update!<br>You have elected to skip this version.");
                            break;
                        case UpdateStatus.CouldNotDetermine:
                            CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ We couldn't tell if there was an update... ]");
                            MessageNotification?.OpenPopUp("We couldn't tell if there was an update...");
                            break;
                    }
                }
            });
        }
        public async void SkipUpdateClick(AppCastItem CurrentItem)
        {
            await Task.Run(() =>
            {
                CLog.Here().Information($"AppUpdaterService - SkipUpdate : [ {CurrentItem.AppName} {CurrentItem.Version} ]");
                SparkleInst.Configuration.SetVersionToSkip(CurrentItem.Version);
            });
        }
        public async void DownloadUpdateClick()
        {
            await Task.Run(() =>
            {
                CLog.Here().Information($"AppUpdaterService - DownloadUpdate : [ Download for Update... ]");
                AvailableUpdate?.ClosePopUp();

                // this is async so that it can grab the download file name from the server
                SparkleInst.DownloadStarted -= CBStartedDownloading;
                SparkleInst.DownloadStarted += CBStartedDownloading;

                SparkleInst.DownloadFinished -= CBFinishedDownloading;
                SparkleInst.DownloadFinished += CBFinishedDownloading;

                SparkleInst.DownloadHadError -= CBDownloadError;
                SparkleInst.DownloadHadError += CBDownloadError;

                SparkleInst.DownloadMadeProgress -= CBDownloadMadeProgress;
                SparkleInst.DownloadMadeProgress += CBDownloadMadeProgress;

                SparkleInst.DownloadCanceled -= CBDownloadCanceled;
                SparkleInst.DownloadCanceled += CBDownloadCanceled;

            });

            await SparkleInst.InitAndBeginDownload(UpdateInfo.Updates.First());
            // ok, the file is downloading now
        }

        public int LastProgressPercentage { get; private set; } = 0;
        public async void CBDownloadMadeProgress(object sender, AppCastItem item, ItemDownloadProgressEventArgs e)
        {
            await Task.Run(() =>
            {
                if (LastProgressPercentage != e.ProgressPercentage)
                {
                    LastProgressPercentage = e.ProgressPercentage;

                    string DownloadLog = string.Format($"The download made some progress! {e.ProgressPercentage}% done.");
                    SparkleInst.LogWriter.PrintMessage(DownloadLog);

                    if (IsCancelRequested == false)
                    {
                        string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>The download made some progress! {e.ProgressPercentage}% done.");
                        DownloadUpdate?.UpdateProgress(DownloadInfo, e.ProgressPercentage);
                    }
                    else
                    {
                        if (IsCanceled == false)
                        {
                            IsCanceled = true;
                            Task.Delay(100);
                            DownloadUpdate?.ClosePopUp();
                        }
                    }
                }
            });
        }
        public async void CBDownloadError(AppCastItem item, string path, Exception exception)
        {
            // Display in progress when error occured -> DownloadInfo.Text = "We had an error during the download process :( -- " + exception.Message;
            await Task.Run(() =>
            {
                string DownloadLog = string.Format($"{item.AppName} {item.Version}, We had an error during the download process :( -- {exception.Message}");
                CLog.Here().Error(DownloadLog);
                DownloadUpdate?.ClosePopUp();
                File.Delete(path);
                IsCancelRequested = false;
                IsCanceled = false;
            });
        }
        public async void CBStartedDownloading(AppCastItem item, string path)
        {
            await Task.Run(() =>
            {
                IsCancelRequested = false;
                IsCanceled = false;
                string DownloadLog = string.Format($"{item.AppName} {item.Version} Started downloading... : [{path}]");
                string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>Started downloading...");

                SparkleInst.LogWriter.PrintMessage(DownloadLog);
                DownloadUpdate?.OpenPopUp(DownloadInfo);
            });
        }
        public async void CBFinishedDownloading(AppCastItem item, string path)
        {
            await Task.Run(() =>
            {
                if (IsCancelRequested == false)
                {
                    string DownloadLog = string.Format($"{item.AppName} {item.Version} Done downloading! : [{path}]");
                    string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>Done downloading!");

                    SparkleInst.LogWriter.PrintMessage(DownloadLog);
                    DownloadUpdate?.UpdateProgress(DownloadInfo, 100);
                    Task.Delay(1000);

                    DownloadUpdate?.ClosePopUp();
                    DownloadPath = path;

                    string FinishedDownloadInfo = string.Format($"{item.AppName} {item.Version}");
                    FinishedDownload?.OpenPopUp(FinishedDownloadInfo);
                }
                else
                {
                    string DownloadLog = string.Format($"{item.AppName} {item.Version} Force Cancel downloading! : [{path}]");
                    SparkleInst.LogWriter.PrintMessage(DownloadLog);

                    string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>Cancel downloading!");
                    DownloadUpdate?.UpdateProgress(DownloadInfo, 100);
                    Task.Delay(1000);

                    if (IsCanceled == false)
                    {
                        IsCancelRequested = false;
                        IsCanceled = true;
                        DownloadUpdate?.ClosePopUp();
                        File.Delete(path);
                    }
                }
            });
        }
        public async void CBDownloadCanceled(AppCastItem item, string path)
        {
            await Task.Run(() =>
            {
                CLog.Here().Information($"AppUpdaterService - CBDownloadCanceled : [ {item.AppName} {item.Version} Cancel downloading! : [{path}] ]");

                if (IsCanceled == false)
                {
                    IsCancelRequested = false;
                    IsCanceled = true;
                    DownloadUpdate?.ClosePopUp();
                }
            });
        }
        public async void InstallUpdateClick()
        {
            await Task.Run(() =>
            {
                CLog.Here().Information($"AppUpdaterService - InstallUpdate : [ Install for Update [{DownloadPath}] ]");
                SparkleInst.CloseApplication += CBCloseApplication;
                SparkleInst.InstallUpdate(UpdateInfo.Updates.First(), DownloadPath);
            });
        }
        public async void CBCloseApplication()
        {
            // System.Windows.Application.Current.Shutdown();
            await Task.Run(() =>
            {
                int nProcessId = Process.GetCurrentProcess().Id;
                CLog.Here().Information($"Self Process Exit to Relaunch after Install and Upgrade: self process is kill (PID:{nProcessId})");
                Process localById = Process.GetProcessById(nProcessId);
                localById.Kill();
            });
        }

        /// <summary>
        /// 자동 업데이트 
        /// </summary>
        public async void CheckUpdateBackgroundDown()
        {
            //CheckUpdate = sgCheckUpdate;
            //AvailableUpdate = sgAvailableUpdate;
            //DownloadUpdate = sgDownloadUpdate;
            //FinishedDownload = sgFinishedDownload;
            //MessageNotification = sgMessageNotification;

            CLog.Here().Information($"AppUpdaterService - CheckUpdatesAutomatically : [ Checking for updates... ]");
            // CheckUpdate?.OpenPopUp();
            UpdateInfo = await SparkleInst.CheckForUpdatesQuietly();
            await Task.Delay(1000);

            await Task.Run(() =>
            {
                //   CheckUpdate?.ClosePopUp();
                // use _sparkle.CheckForUpdatesQuietly() if you don't want the user to know you are checking for updates!
                // if you use CheckForUpdatesAtUserRequest() and are using a UI, then handling things yourself is rather silly
                // as it will show a UI for things
                if (UpdateInfo != null)
                {
                    CLog.Here().Information($"AppUpdaterService - Check Update Status : {UpdateInfo.Status.ToString()}");
                    switch (UpdateInfo.Status)
                    {
                        case UpdateStatus.UpdateAvailable:
                            CLog.Here().Information($"AppUpdaterService - CheckUpdatesAutomatically : [ There's an update available! ]");
                            InitializeBackgroundUpdate(SparkleInst, UpdateInfo.Updates);
                            DownloadUpdateBackground();
                            // AvailableUpdate?.OpenPopUp(SparkleInst, UpdateInfo.Updates);
                            break;
                            #region [사용안함]
                            //case UpdateStatus.UpdateNotAvailable:
                            //    CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ There's no update available :( ]");
                            //    // MessageNotification?.OpenPopUp("There's no update available :(");
                            //    break;
                            //case UpdateStatus.UserSkipped:
                            //    CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ The user skipped this update! ]");
                            //    MessageNotification?.OpenPopUp("The user skipped this update!<br>You have elected to skip this version.");
                            //    break;
                            //case UpdateStatus.CouldNotDetermine:
                            //    CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ We couldn't tell if there was an update... ]");
                            //    //    MessageNotification?.OpenPopUp("We couldn't tell if there was an update...");
                            //    break; 
                            #endregion
                    }
                }
            });
        }

        /// <summary>
        /// appcast.xml 파일을 기반으로 new Ver 파일 다운로드
        /// </summary>
        /// <param name="sparkle"></param>
        /// <param name="items"></param>
        /// <param name="isUpdateAlreadyDownloaded"></param>
        /// <param name="separatorTemplate"></param>
        /// <param name="headAddition"></param>
        private async void InitializeBackgroundUpdate(SparkleUpdater sparkle, List<AppCastItem> items, bool isUpdateAlreadyDownloaded = false,
                                            string separatorTemplate = "", string headAddition = "")
        {
            CLog.Here().Information($"- InitializeBackgroundUpdate...");
            SelfReleaseNotesGrabber _ReleaseNotesGrabber = null;
            AppCastItem latestVersion = null;
            
            await Task.Run(() =>
            {
                _ReleaseNotesGrabber = new SelfReleaseNotesGrabber(separatorTemplate, headAddition, sparkle);

                AppCastItem item = items.FirstOrDefault();              
                latestVersion = items.OrderByDescending(p => p.Version).FirstOrDefault();
            });

            CancellationTokenSource _CancellationTokenSource = new CancellationTokenSource();
            CancellationToken _CancellationToken = _CancellationTokenSource.Token;

            string releaseNotes = await _ReleaseNotesGrabber.DownloadAllReleaseNotes(items, latestVersion, _CancellationToken);
            CLog.Here().Information($"- InitializeBackgroundUpdate...Done");
        }

        public async void DownloadUpdateBackground()
        {
            await Task.Run(() =>
            {
                // RunFullUpdateUpdateStatusLabel.Text = "Checking for update...";
                CLog.Here().Information($"AppUpdaterService - UpdateAutomatically : [ Checking for updates... ]");

                SparkleInst.UserInteractionMode = UserInteractionMode.DownloadAndInstall;
                SparkleInst.UpdateDetected -= CBFullUpdateUpdateDetected;
                SparkleInst.UpdateDetected += CBFullUpdateUpdateDetected;

                SparkleInst.DownloadStarted -= CBFullUpdateStartedDownloading;
                SparkleInst.DownloadStarted += CBFullUpdateStartedDownloading;

                SparkleInst.DownloadFinished -= CBFullUpdateDownloadFileIsReady;
                SparkleInst.DownloadFinished += CBFullUpdateDownloadFileIsReady;

                SparkleInst.CloseApplication -= CBFullUpdateCloseApplication;
                SparkleInst.CloseApplication += CBFullUpdateCloseApplication;

                SparkleInst.DownloadHadError -= CBDownloadError;
                SparkleInst.DownloadHadError += CBDownloadError;
            });


            await SparkleInst.InitAndBeginDownload(UpdateInfo.Updates.First());
        }
        public async void CBFullUpdateUpdateDetected(object sender, UpdateDetectedEventArgs e)
        {
            await Task.Run(() =>
            {
                // RunFullUpdateUpdateStatusLabel.Text = "Found update...";
                string UpdateLog = string.Format($"AppUpdaterService - CBFullUpdateUpdateDetected : [ Found update... ]");
                SparkleInst.LogWriter.PrintMessage(UpdateLog);
            });
        }
        public async void CBFullUpdateStartedDownloading(AppCastItem item, string path)
        {
            await Task.Run(() =>
            {
                // RunFullUpdateUpdateStatusLabel.Text = "Started downloading update...";
                string DownloadLog = string.Format($"AppUpdaterService - CBFullUpdateStartedDownloading : [{item.AppName} {item.Version} - Started downloading update... [{path}]]");
                SparkleInst.LogWriter.PrintMessage(DownloadLog);
            });
        }
        public async void CBFullUpdateDownloadFileIsReady(AppCastItem item, string downloadPath)
        {
            await Task.Run(() =>
            {
                // RunFullUpdateUpdateStatusLabel.Text = "Update is ready...";
                string DownloadLog = string.Format($"AppUpdaterService - CBFullUpdateDownloadFileIsReady : [{item.AppName} {item.Version} - Update is ready... [{downloadPath}]]");
                SparkleInst.LogWriter.PrintMessage(DownloadLog);



            });

            // Now Install
            InstallUpdateClick();
        }
        public async void CBFullUpdateCloseApplication()
        {
            //System.Windows.Application.Current.Shutdown();
            await Task.Delay(2000);
            await Task.Run(() =>
            {
                // RunFullUpdateUpdateStatusLabel.Text = "Closing application...";
                int nProcessId = Process.GetCurrentProcess().Id;
                CLog.Here().Information($"Self Process Exit to Relaunch after Install and Upgrade: self process is kill (PID:{nProcessId})");
                Process localById = Process.GetProcessById(nProcessId);
                localById.Kill();
            });
        }
    }
}