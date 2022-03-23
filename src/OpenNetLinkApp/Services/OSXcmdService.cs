using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using OpenNetLinkApp.Data.SGDicData.SGUnitData;
using System.Text;
using System.Text.Json;
using System.Xml;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace OpenNetLinkApp.Services
{
    public class OSXcmdService
    {

        public string GetOSXADConfig(string KeyName)
        {
            Process p = new Process();
            p.StartInfo = new ProcessStartInfo("dsconfigad", "-show -xml")
            {
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            p.Start();
            p.WaitForExit();

            var output = p.StandardOutput.ReadToEnd();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(output);

            XDocument doc = XDocument.Parse(output);
            Dictionary<string, string> dataDictionary = new Dictionary<string, string>();

            foreach (XElement element in doc.Descendants().Where(p => p.HasElements == false))
            {
                string keyName = element.Name.LocalName;
                string keyValue = element.Value;

                if (keyName.Equals("key"))
                {
                    if (keyValue.Equals("Active Directory Domain") || keyValue.Equals("Active Directory Forest") || keyValue.Equals("Computer Account"))
                    {
                        string value = "";
                        if (element.NextNode != null)
                        {
                            XNode test = element.NextNode;
                            value = (test as XElement).Value;
                        }

                        dataDictionary.Add(keyValue, value);
                    }
                }
            }

            return dataDictionary[KeyName];
        }



    }

}