using System;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

namespace MD5HashUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            OSPlatform osInfo = OSPlatform.Windows;
            string osStr = String.Empty;
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                osInfo = OSPlatform.Windows; //Window
                osStr = "windows";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                osInfo = OSPlatform.OSX; //MAC    
                osStr = "mac";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                osInfo = OSPlatform.Linux; //Linux
                osStr = "linux";
            }

            string hashFileList = String.Empty; //Hash 파일 작성 
            string binFilePath = String.Empty; // hsck.bin 파일 작성 경로
            string sqlFilePath = String.Empty; // 작성한 sql문 파일 경로
            string targetFolderPath = String.Empty; //Hash Code를 작성할 파일들 위치
            string buildPath = String.Empty; //Directory.Build.props 빌드파일 경로

            string version = String.Empty; //버전정보
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            int point = appPath.IndexOf("src\\");
            appPath = appPath.Substring(0, point + 4);

            
            //같은 위치에....HashFileList.txt 찾기
            //hashFileList = appPath + "OpenNetLinkApp\\HashFileList.txt";
#if DEBUG
            hashFileList = appPath + "OpenNetLinkApp\\HashFileList.txt";
            binFilePath = appPath + "OpenNetLinkApp\\bin\\Debug\\net5.0\\win-x64\\hsck.bin";
            targetFolderPath = appPath + "OpenNetLinkApp\\bin\\Debug\\net5.0\\win-x64\\";
            sqlFilePath = appPath + "OpenNetLinkApp\\VersionHash.txt";
            buildPath = appPath + "OpenNetLinkApp\\Directory.Build.props";
#else
            if (osInfo == OSPlatform.Windows)
            {
                hashFileList = appPath + "OpenNetLinkApp\\HashFileList.txt";
                binFilePath = appPath + "artifacts\\windows\\published\\hsck.bin";
                targetFolderPath = appPath + "artifacts\\windows\\published\\";
                sqlFilePath = appPath + "OpenNetLinkApp\\VersionHash.txt";
                buildPath = appPath + "OpenNetLinkApp\\Directory.Build.props";
            }
            else if(osInfo == OSPlatform.OSX)
            {
                hashFileList = "./OpenNetLinkApp/HashFileList.txt";
                binFilePath = "./artifacts/mac/published/hsck.bin";
                targetFolderPath = "./artifacts/mac/published/";
                sqlFilePath = "./OpenNetLinkApp/VersionHash.txt";
                buildPath = "./OpenNetLinkApp/Directory.Build.props";
            }
            else
            {
                hashFileList = "./OpenNetLinkApp/HashFileList.txt";
                binFilePath = "./artifacts/mac/published/hsck.bin";
                targetFolderPath = "./artifacts/mac/published/";
                sqlFilePath = "./OpenNetLinkApp/VersionHash.txt";
                buildPath = "./OpenNetLinkApp/Directory.Build.props";
            }
#endif
            //sqlFilePath = appPath + "OpenNetLinkApp\\VersionHash.txt";
            XmlTextReader xmlReader = new XmlTextReader(buildPath);
            while(xmlReader.Read())
            {
                switch(xmlReader.NodeType)
                {
                    case XmlNodeType.Element:
                        if(xmlReader.Name == "Version")
                        {
                            version = xmlReader.ReadString();
                        }
                        break;
                }
            }

            List<string> fileList = GetFileList(hashFileList);

            if (fileList.Count == 0)
                return;

            string finalString = String.Empty;
            foreach(string fileName in fileList)
            {
                finalString += fileName + "\n";
            }

            finalString += "\n";

            string hash = GetAgentHash(hashFileList, targetFolderPath);

            finalString += hash;

            string sqlInsert = String.Empty;

            sqlInsert = $@"
-- sql에서 등록 / Agent 해시값
-- 내부망 I, 외부망 E, SQL 쿼리문 실행시 I, E를 변경해 주세요.
delete from tbl_agent_hash where system_type='I' and version ='OpenNetLink {version}:{osStr}';
insert into tbl_agent_hash values ('I', 'OpenNetLink {version}:{osStr}', '{hash}', to_char(now(), 'YYYYMMDDHH24miss'), '1');
";


            byte[] info = HsckCrypto.AESEncrypt256(finalString);

            using(FileStream fs = File.Create(sqlFilePath))
            {
                byte[] hashInfo = new UTF8Encoding(true).GetBytes(sqlInsert);
                fs.Write(hashInfo, 0, hashInfo.Length);
            }

            using (FileStream fs = File.Create(binFilePath))
            {
                fs.Write(info, 0, info.Length);
            }
        }

        private static List<string> GetFileList(string fileName)
        {
            List<string> fileList = new List<string>();
            string line;

            using (StreamReader reader = new StreamReader(fileName))
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

            return fileList;
        }
        private static string GetAgentHash(string hashFileList, string targetFolder)
        {
            if (String.IsNullOrEmpty(hashFileList))
            {
                return String.Empty;
            }

            List<string> fileList = GetFileList(hashFileList);
            

            IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.MD5);

            // Hash 체크할 파일들을 정리해야함....
            foreach (string fileName in fileList)
            {
                byte[] fileByte = File.ReadAllBytes(targetFolder + fileName);
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
