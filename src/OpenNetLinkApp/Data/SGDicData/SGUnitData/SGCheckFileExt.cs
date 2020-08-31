using System;
using HeyRed.Mime;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using OpenNetLinkApp.Data;

namespace OpenNetLinkApp.Data.SGDicData.SGUnitData
{
    public class SGCheckFileExt
    {
        public SGCheckFileExt()
        {

        }
        ~SGCheckFileExt()
        {

        }
        
        private const int MaxBufferSize = 1024 * 64;

        private static byte[] StreamToByteArray(Stream stInput, int nMaxSize)
        {
            if (stInput == null) return null;
            byte[] buffer = new byte[nMaxSize];
            stInput.Position = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                read = stInput.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, read);
                byte[] temp = ms.ToArray();

                return temp;
            }
        }

        public bool IsValidFileExt(Stream stFile, string strExt)
        {
            byte[] btFileData = StreamToByteArray(stFile, MaxBufferSize);
            string strFileMime = MimeGuesser.GuessMimeType(btFileData);
            if (String.IsNullOrEmpty(strExt) == true) {
                if (String.Compare(strFileMime, "text/plain") == 0) return true;
                if (String.Compare(strFileMime, "application/x-executable") == 0) return true;

                return false;
            }

            string strFileExt = MimeGuesser.GuessExtension(btFileData);
            Console.WriteLine("FileExt [" + strFileExt + "] Ext[" + strExt + "]"); 
            if (String.Compare(strFileExt, strExt) == 0) return true;

            string strExtMime = MimeTypesMap.GetMimeType(strExt);
            Console.WriteLine("ExtMime [" + strFileMime + "] Ext [" + strExtMime + "]"); 
            if (String.Compare(strFileMime, strExtMime) == 0) return true;

            string strFileMimeToExt = MimeTypesMap.GetExtension(strExtMime);
            Console.WriteLine("ExtFileMimeToExt [" + strFileMimeToExt + "] Ext [" + strExt + "]"); 
            if (String.Compare(strFileMimeToExt, strExt) == 0) return true;

            return false;
        }
        public void InitMagicDB(string strFilePath)
        {
            MimeGuesser.MagicFilePath = strFilePath;
        }
        public void AddOrUpdate(string strMime, string strExt) => MimeTypesMap.AddOrUpdate(strMime, strExt);
    }
}
