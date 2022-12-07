using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices;

using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.Events;
using NetSparkleUpdater.SignatureVerifiers;
using NetSparkleUpdater.Downloaders;
using System.Threading;
using System.Net;

namespace OpenNetLinkApp.Services.SGAppUpdater
{
    internal class SelfReleaseNotesGrabber : NetSparkleUpdater.ReleaseNotesGrabber
    {
        /// <summary>
        /// Base constructor for ReleaseNotesGrabber
        /// </summary>
        /// <param name="separatorTemplate">Template to use for separating each item in the HTML</param>
        /// <param name="htmlHeadAddition">Any additional header information to stick in the HTML that will show up in the release notes</param>
        /// <param name="sparkle">Sparkle updater being used</param>
        public SelfReleaseNotesGrabber(string separatorTemplate, string htmlHeadAddition, SparkleUpdater sparkle)
                : base(separatorTemplate, htmlHeadAddition, sparkle) { }

        /// <summary>
        /// Download the release notes at the given link. Does not do anything else
        /// for the release notes (verification, display, etc.) -- just downloads the
        /// release notes and passes them back as a string.
        /// </summary>
        /// <param name="link">string URL to the release notes to download</param>
        /// <param name="cancellationToken">token that can be used to cancel a download operation</param>
        /// <param name="sparkle"><see cref="SparkleUpdater"/> that can be used for logging information
        /// about the download process (or its failures)</param>
        /// <returns></returns>
        protected override async Task<string> DownloadReleaseNotes(string link, CancellationToken cancellationToken, SparkleUpdater sparkle)
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    ServicePointManager.ServerCertificateValidationCallback += (mender, certificate, chain, sslPolicyErrors) => true;
                    webClient.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                    webClient.Encoding = Encoding.UTF8;
                    //if (cancellationToken != null)
                    //{
                    using (cancellationToken.Register(() => webClient.CancelAsync()))
                    {

                        return await webClient.DownloadStringTaskAsync(Utilities.GetAbsoluteURL(link, sparkle.AppCastUrl));
                    }
                    //}
                    //return await webClient.DownloadStringTaskAsync(Utilities.GetAbsoluteURL(link, sparkle.AppCastUrl));
                }
            }
            catch (WebException ex)
            {
                sparkle.LogWriter.PrintMessage("Cannot download release notes from {0} because {1}", link, ex.Message);
                return "";
            }
        }
    } // internal class SelfReleaseNotesGrabber : NetSparkleUpdater.ReleaseNotesGrabber

}
