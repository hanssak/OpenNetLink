using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.Events;
using NetSparkleUpdater.SignatureVerifiers;

using OpenNetLinkApp.Models.SGUserInfo;
using OpenNetLinkApp.Models.SGNetwork;
using OpenNetLinkApp.Models.SGConfig;

namespace OpenNetLinkApp.Services.SGAppUpdater
{
    public interface ISGAppUpdaterService
    {
        /* To Manage Header State */
        /// <summary>
        /// Declared: Header Action Service for UI Header, included ISGHeaderUI(SGHeaderUI)
        /// </summary>
        SparkleUpdater SparkleInst { get; }

        /* To Manage Footer State */
        /// <summary>
        /// Declared: Footer Action Service for UI Footer, included ISGFooterUI(SGFooterUI)
        /// </summary>
        UpdateInfo UpdateInfo { get; }

        /* To Manage Corporate Identity State */
        /// <summary>
        /// Declared: Corporate Identity(CI) Service for CI Info/Image.
        /// </summary>
        string DownloadPath { get; }

        /* To Function Features */
        void CheckUpdatesClick();
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
        public SGAppUpdaterService()
        {
            SparkleInst = new SparkleUpdater("https://netsparkleupdater.github.io/NetSparkle/files/sample-app/appcast.xml", new DSAChecker(SecurityMode.Strict))
            {
                UIFactory = null,
            };
            // TLS 1.2 required by GitHub (https://developer.github.com/changes/2018-02-01-weak-crypto-removal-notice/)
            SparkleInst.SecurityProtocolType = System.Net.SecurityProtocolType.Tls12;
        }

        /* To Manage Header State */
        public SparkleUpdater SparkleInst { get; private set; } = null;

        /* To Manage Footer State */
        public UpdateInfo UpdateInfo { get; private set; } = null;

        /* To Manage Corporate Identity State */
        public string DownloadPath { get; private set; } = string.Empty;

        /* To Function Features */
        public async void CheckUpdatesClick()
        {
            //UpdateInfo.Content = "Checking for updates...";
            UpdateInfo = await SparkleInst.CheckForUpdatesQuietly();
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
                        break;
                    case UpdateStatus.UpdateNotAvailable:
                        //UpdateInfo.Content = "There's no update available :(";
                        //DownloadUpdateButton.IsEnabled = false;
                        break;
                    case UpdateStatus.UserSkipped:
                        //UpdateInfo.Content = "The user skipped this update!";
                        //DownloadUpdateButton.IsEnabled = false;
                        break;
                    case UpdateStatus.CouldNotDetermine:
                        //UpdateInfo.Content = "We couldn't tell if there was an update...";
                        //DownloadUpdateButton.IsEnabled = false;
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