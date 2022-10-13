using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace MD5HashUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            string hashFileList = args[0];
            string finalPath = "D:\\github\\OpenNetLink\\src\\OpenNetLinkApp\\hsck.bin";

            string hash = GetAgentHash(hashFileList);

            using (FileStream fs = File.Create(finalPath))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(hash);
                fs.Write(info, 0, info.Length);
            }
        }

        private static string GetAgentHash(string hashFileList)
        {
            if (String.IsNullOrEmpty(hashFileList))
            {
                return String.Empty;
            }

            List<string> fileList = new List<string>();
            string line;

            using (StreamReader reader = new StreamReader(hashFileList))
            {
                bool end = true;
                while (end)
                {
                    line = reader.ReadLine();
                    if (line == null)
                        end = false;
                    else
                        fileList.Add(line);
                }

                reader.Close();
            }

            IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.MD5);

            // Hash 체크할 파일들을 정리해야함....
            foreach (string fileName in fileList)
            {
                byte[] fileByte = File.ReadAllBytes(fileName);
                hash.AppendData(fileByte, 0, fileByte.Length);
            }

            return ByteToHexString(hash.GetHashAndReset());
        }

        public static string ByteToHexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }
    }
}
