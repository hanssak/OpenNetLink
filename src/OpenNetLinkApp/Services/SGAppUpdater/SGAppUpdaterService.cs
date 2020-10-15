using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

using Serilog;
using Serilog.Events;
using AgLogManager;

using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.Events;
using NetSparkleUpdater.SignatureVerifiers;
using NetSparkleUpdater.Downloaders;

using OpenNetLinkApp.Models.SGUserInfo;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Models.SGConfig;
using OpenNetLinkApp.Components.SGUpdate;

namespace OpenNetLinkApp.Services.SGAppUpdater
{
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
        void InstallUpdateClick();
        void CBCloseApplication();
        void UpdateAutomaticallyClick();
        void CBFullUpdateUpdateDetected(object sender, UpdateDetectedEventArgs e);
        void CBFullUpdateStartedDownloading(AppCastItem item, string path);
        void CBFullUpdateDownloadFileIsReady(AppCastItem item, string downloadPath);
        void CBFullUpdateCloseApplication();
    }
    internal class SGAppUpdaterService : ISGAppUpdaterService
    {
        private Serilog.ILogger CLog => Serilog.Log.ForContext<SGAppUpdaterService>();
        public SGAppUpdaterService() {}
        public void Init(string updateSvcIP, string updatePlatform)
        {
            CLog.Here().Information($"- AppUpdaterService Initializing... : [UpdateSvcIP({updateSvcIP}), UpdatePlatform({updatePlatform})]");
            //SparkleInst = new SparkleUpdater($"https://{updateSvcIP}/NetSparkle/files/sample-app/appcast.xml", new DSAChecker(SecurityMode.Strict))
            SparkleInst = new SparkleUpdater($"https://{updateSvcIP}/updatePlatform/{updatePlatform}/appcast.xml", 
                                                new Ed25519Checker(SecurityMode.Strict, null, "wwwroot/conf/Sparkling.service")) 
            {
                UIFactory = null,
                AppCastDataDownloader = new WebRequestAppCastDataDownloader(),
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
        public SGCheckUpdate CheckUpdate { get; private set; } =  null;
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

            await Task.Run(() => {
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
            await Task.Run(() => {
                CLog.Here().Information($"AppUpdaterService - SkipUpdate : [ {CurrentItem.AppName} {CurrentItem.Version} ]");
                SparkleInst.Configuration.SetVersionToSkip(CurrentItem.Version);
            });
        }
        public async void DownloadUpdateClick()
        {
            await Task.Run(() => {
                CLog.Here().Information($"AppUpdaterService - DownloadUpdate : [ Download for Update... ]");
                AvailableUpdate?.ClosePopUp();

                // this is async so that it can grab the download file name from the server
                SparkleInst.DownloadStarted -= CBStartedDownloading;
                SparkleInst.DownloadStarted += CBStartedDownloading;

                SparkleInst.DownloadFinished -= CBFinishedDownloading;
                SparkleInst.DownloadFinished += CBFinishedDownloading;

                SparkleInst.DownloadHadError -= CBDownloadError;
                SparkleInst.DownloadHadError += CBDownloadError;

                SparkleInst.DownloadMadeProgress += CBDownloadMadeProgress;

            });

            await SparkleInst.InitAndBeginDownload(UpdateInfo.Updates.First());
            // ok, the file is downloading now
        }

        public int LastProgressPercentage  { get; private set; } = 0;
        public async void CBDownloadMadeProgress(object sender, AppCastItem item, ItemDownloadProgressEventArgs e)
        {
            await Task.Run(() => {
                string DownloadLog = string.Format($"The download made some progress! {e.ProgressPercentage}% done.");
                string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>The download made some progress! {e.ProgressPercentage}% done.");

                SparkleInst.LogWriter.PrintMessage(DownloadLog);
                if(LastProgressPercentage != e.ProgressPercentage) DownloadUpdate?.UpdateProgress(DownloadInfo, e.ProgressPercentage);
                LastProgressPercentage = e.ProgressPercentage;
            });
        }
        public async void CBDownloadError(AppCastItem item, string path, Exception exception)
        {
            // Display in progress when error occured -> DownloadInfo.Text = "We had an error during the download process :( -- " + exception.Message;
            await Task.Run(() => {
                string DownloadLog = string.Format($"{item.AppName} {item.Version}, We had an error during the download process :( -- {exception.Message}");
                CLog.Here().Error(DownloadLog);
                DownloadUpdate?.ClosePopUp();
            });
        }
        public async void CBStartedDownloading(AppCastItem item, string path)
        {
            await Task.Run(() => {
                string DownloadLog = string.Format($"{item.AppName} {item.Version} Started downloading... : [{path}]");
                string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>Started downloading...");

                SparkleInst.LogWriter.PrintMessage(DownloadLog);
                DownloadUpdate?.OpenPopUp(DownloadInfo);
            });
        }
        public async void CBFinishedDownloading(AppCastItem item, string path)
        {
            await Task.Run(() => {
                string DownloadLog = string.Format($"{item.AppName} {item.Version} Done downloading! : [{path}]");
                string DownloadInfo = string.Format($"{item.AppName} {item.Version}<br>Done downloading!");

                SparkleInst.LogWriter.PrintMessage(DownloadLog);
                DownloadUpdate?.UpdateProgress(DownloadInfo, 100);
                DownloadUpdate?.ClosePopUp();
                DownloadPath = path;

                Task.Delay(1000);
                string FinishedDownloadInfo = string.Format($"{item.AppName} {item.Version}");
                FinishedDownload?.OpenPopUp(FinishedDownloadInfo);
            });
        }
        public async void InstallUpdateClick()
        {
            await Task.Run(() => {
                CLog.Here().Information($"AppUpdaterService - InstallUpdate : [ Install for Update [{DownloadPath}] ]");
                SparkleInst.CloseApplication += CBCloseApplication;
                SparkleInst.InstallUpdate(UpdateInfo.Updates.First(), DownloadPath);
            });
        }
        public async void CBCloseApplication()
        {
            // System.Windows.Application.Current.Shutdown();
            await Task.Run(() => {
                int nProcessId = Process.GetCurrentProcess().Id;
                CLog.Here().Information($"Self Process Exit to Relaunch after Install and Upgrade: self process is kill (PID:{nProcessId})");
                Process localById = Process.GetProcessById(nProcessId);
                localById.Kill();
            });
        }
        public async void UpdateAutomaticallyClick()
        {
            await Task.Run(() => {
                // RunFullUpdateUpdateStatusLabel.Text = "Checking for update...";
                CLog.Here().Information($"AppUpdaterService - UpdateAutomatically : [ Checking for updates... ]");

                SparkleInst.UserInteractionMode = UserInteractionMode.DownloadAndInstall;
                SparkleInst.UpdateDetected += CBFullUpdateUpdateDetected;
                SparkleInst.DownloadStarted += CBFullUpdateStartedDownloading;
                SparkleInst.DownloadFinished += CBFullUpdateDownloadFileIsReady;
                SparkleInst.CloseApplication += CBFullUpdateCloseApplication;
            });

            await SparkleInst.CheckForUpdatesQuietly();
        }
        public async void CBFullUpdateUpdateDetected(object sender, UpdateDetectedEventArgs e)
        {
            await Task.Run(() => {
                // RunFullUpdateUpdateStatusLabel.Text = "Found update...";
                string UpdateLog = string.Format($"AppUpdaterService - CBFullUpdateUpdateDetected : [ Found update... ]");
                SparkleInst.LogWriter.PrintMessage(UpdateLog);
            });
        }
        public async void CBFullUpdateStartedDownloading(AppCastItem item, string path)
        {
            await Task.Run(() => {
                // RunFullUpdateUpdateStatusLabel.Text = "Started downloading update...";
                string DownloadLog = string.Format($"AppUpdaterService - CBFullUpdateStartedDownloading : [{item.AppName} {item.Version} - Started downloading update... [{path}]]");
                SparkleInst.LogWriter.PrintMessage(DownloadLog);
            });
        }
        public async void CBFullUpdateDownloadFileIsReady(AppCastItem item, string downloadPath)
        {
            await Task.Run(() => {
                // RunFullUpdateUpdateStatusLabel.Text = "Update is ready...";
                string DownloadLog = string.Format($"AppUpdaterService - CBFullUpdateDownloadFileIsReady : [{item.AppName} {item.Version} - Update is ready... [{downloadPath}]]");
                SparkleInst.LogWriter.PrintMessage(DownloadLog);
            });
        }
        public async void CBFullUpdateCloseApplication()
        {
            //System.Windows.Application.Current.Shutdown();
            await Task.Delay(2000);
            await Task.Run(() => {
                // RunFullUpdateUpdateStatusLabel.Text = "Closing application...";
                int nProcessId = Process.GetCurrentProcess().Id;
                CLog.Here().Information($"Self Process Exit to Relaunch after Install and Upgrade: self process is kill (PID:{nProcessId})");
                Process localById = Process.GetProcessById(nProcessId);
                localById.Kill();
            });
        }
    }
}