using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
                               SGMessageNotification sgMessageNotification = null);
        void DownloadUpdateClick();
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
            (SparkleInst.LogWriter as LogWriter).PrintDiagnosticToConsole = true;
            // TLS 1.2 required by GitHub (https://developer.github.com/changes/2018-02-01-weak-crypto-removal-notice/)
            SparkleInst.SecurityProtocolType = System.Net.SecurityProtocolType.Tls12;
            (SparkleInst.AppCastDataDownloader as WebRequestAppCastDataDownloader).TrustEverySSLConnection = true;
            CLog.Here().Information($"- AppUpdaterService Initializing...Done : [UpdateSvcIP({updateSvcIP}), UpdatePlatform({updatePlatform})]");
        }

        /* To Manage Updater, NetSparkle Instance */
        public SparkleUpdater SparkleInst { get; private set; } = null;

        /* To Save the gathered Update Info */
        public UpdateInfo UpdateInfo { get; private set; } = null;

        /* To Save Downloaded Package File */
        public string DownloadPath { get; private set; } = string.Empty;

        /* To Function Features */
        public async void CheckUpdatesClick(SGCheckUpdate sgCheckUpdate = null, 
                                            SGAvailableUpdate sgAvailableUpdate = null,
                                            SGMessageNotification sgMessageNotification = null)
        {
            //UpdateInfo.Content = "Checking for updates...";
            CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ Checking for updates... ]");
            sgCheckUpdate.OpenPopUp();
            UpdateInfo = await SparkleInst.CheckForUpdatesQuietly();
            await Task.Delay(1000);
            sgCheckUpdate.ClosePopUp();
            // use _sparkle.CheckForUpdatesQuietly() if you don't want the user to know you are checking for updates!
            // if you use CheckForUpdatesAtUserRequest() and are using a UI, then handling things yourself is rather silly
            // as it will show a UI for things
            if (UpdateInfo != null)
            {
                switch (UpdateInfo.Status)
                {
                    case UpdateStatus.UpdateAvailable:
                        //UpdateInfo.Content = "There's an update available!";
                        //DownloadUpdateButton.IsEnabled = true;
                        CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ There's an update available! ]");
                        break;
                    case UpdateStatus.UpdateNotAvailable:
                        //UpdateInfo.Content = "There's no update available :(";
                        //DownloadUpdateButton.IsEnabled = false;
                        CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ There's no update available :( ]");
                        sgMessageNotification?.OpenPopUp("There's no update available :(");
                        break;
                    case UpdateStatus.UserSkipped:
                        //UpdateInfo.Content = "The user skipped this update!";
                        //DownloadUpdateButton.IsEnabled = false;
                        CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ The user skipped this update! ]");
                        sgMessageNotification?.OpenPopUp("The user skipped this update!<br>You have elected to skip this version.");
                        break;
                    case UpdateStatus.CouldNotDetermine:
                        //UpdateInfo.Content = "We couldn't tell if there was an update...";
                        //DownloadUpdateButton.IsEnabled = false;
                        CLog.Here().Information($"AppUpdaterService - CheckUpdates : [ We couldn't tell if there was an update... ]");
                        sgMessageNotification?.OpenPopUp("We couldn't tell if there was an update...");
                        break;
                }
            }
        }
        public async void DownloadUpdateClick()
        {
            // this is async so that it can grab the download file name from the server
            SparkleInst.DownloadStarted -= CBStartedDownloading;
            SparkleInst.DownloadStarted += CBStartedDownloading;

            SparkleInst.DownloadFinished -= CBFinishedDownloading;
            SparkleInst.DownloadFinished += CBFinishedDownloading;

            SparkleInst.DownloadHadError -= CBDownloadError;
            SparkleInst.DownloadHadError += CBDownloadError;

            SparkleInst.DownloadMadeProgress += CBDownloadMadeProgress;

            await SparkleInst.InitAndBeginDownload(UpdateInfo.Updates.First());
            // ok, the file is downloading now
        }
        public void CBDownloadMadeProgress(object sender, AppCastItem item, ItemDownloadProgressEventArgs e)
        {
            // TODO: Display in progress -> DownloadInfo.Text = string.Format("The download made some progress! {0}% done.", e.ProgressPercentage);
        }
        public void CBDownloadError(AppCastItem item, string path, Exception exception)
        {
            // TODO: Display in progress when error occured -> DownloadInfo.Text = "We had an error during the download process :( -- " + exception.Message;
            //InstallUpdateButton.IsEnabled = false;
        }
        public void CBStartedDownloading(AppCastItem item, string path)
        {
            // TODO: Display in progress -> DownloadInfo.Text = "Started downloading...";
            //InstallUpdateButton.IsEnabled = false;
        }
        public void CBFinishedDownloading(AppCastItem item, string path)
        {
            // TODO: Display in progress -> DownloadInfo.Text = "Done downloading!";
            //InstallUpdateButton.IsEnabled = true;
            DownloadPath = path;
        }
        public void InstallUpdateClick()
        {
            SparkleInst.CloseApplication += CBCloseApplication;
            SparkleInst.InstallUpdate(UpdateInfo.Updates.First(), DownloadPath);
        }
        public void CBCloseApplication()
        {
            // TODO: System.Windows.Application.Current.Shutdown();
        }
        public async void UpdateAutomaticallyClick()
        {
            SparkleInst.UserInteractionMode = UserInteractionMode.DownloadAndInstall;
            // TODO: RunFullUpdateUpdateStatusLabel.Text = "Checking for update...";
            SparkleInst.UpdateDetected += CBFullUpdateUpdateDetected;
            SparkleInst.DownloadStarted += CBFullUpdateStartedDownloading;
            SparkleInst.DownloadFinished += CBFullUpdateDownloadFileIsReady;
            SparkleInst.CloseApplication += CBFullUpdateCloseApplication;
            await SparkleInst.CheckForUpdatesQuietly();
        }
        public void CBFullUpdateUpdateDetected(object sender, UpdateDetectedEventArgs e)
        {
            // TODO: RunFullUpdateUpdateStatusLabel.Text = "Found update...";
        }
        public void CBFullUpdateStartedDownloading(AppCastItem item, string path)
        {
            // TODO: RunFullUpdateUpdateStatusLabel.Text = "Started downloading update...";
        }
        public void CBFullUpdateDownloadFileIsReady(AppCastItem item, string downloadPath)
        {
            // TODO: RunFullUpdateUpdateStatusLabel.Text = "Update is ready...";
        }
        public void CBFullUpdateCloseApplication()
        {
            // TODO: RunFullUpdateUpdateStatusLabel.Text = "Closing application...";
            //await Task.Delay(2000);
            //System.Windows.Application.Current.Shutdown();
        }
    }
}