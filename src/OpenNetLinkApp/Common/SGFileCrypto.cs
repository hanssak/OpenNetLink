using HsNetWorkSG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNetLinkApp
{
    public class SGFileCrypto
    {
        private static SGFileCrypto _instance { get; set; }
        public static SGFileCrypto Instance
        {
            get
            {
                _instance = _instance ?? new SGFileCrypto();
                return _instance;
            }
        }

        const string initDirPath = "wwwroot/conf/Init";
        const string saveDirPath = "wwwroot/conf";     

        public bool EncryptSettingFiles()
        {
            if (!Directory.Exists(initDirPath))
                return true;

            byte[] masterKey = new byte[0];
            byte[] originalSetFile = new byte[0];
            byte[] EncSetFile = new byte[0];
            try
            {
                string[] initFileList = Directory.GetFiles(initDirPath);
                //DEK로 재암호화
                foreach (string setFilePath in initFileList)
                {
                    masterKey = SGCrypto.GetMasterKey();
                    byte[] contents = File.ReadAllBytes(setFilePath);
                    //Master로 복호화
                    SGCrypto.AESDecrypt256(contents, ref masterKey, System.Security.Cryptography.PaddingMode.PKCS7, ref originalSetFile);

                    //DEK로 암호화
                    SGCrypto.AESEncrypt256WithDEK(originalSetFile, ref EncSetFile);

                    File.WriteAllBytes(setFilePath, EncSetFile);
                    //DEK로 암호화된 파일 이동
                    string name = Path.GetFileName(setFilePath);
                    File.Move(setFilePath, Path.Combine(saveDirPath, name), true);
                }

                foreach(string fileList in Directory.GetFiles(initDirPath))
                {
                    File.Delete(fileList);
                }
                foreach (string dirList in Directory.GetDirectories(initDirPath))
                {
                    Directory.Delete(dirList);
                }
                Directory.Delete(initDirPath);

                return true;
            }
            catch (Exception ex)
            {
                HsLog.err("EncryptSettingFiles Exception : " + ex.ToString());
                return false;
            }
            finally
            {
                masterKey.hsClear(3);
                originalSetFile.hsClear(3);
                EncSetFile.hsClear(3);
            }
        }

        public bool EncryptSettingFiles(string SP1Path)
        {
            if (!Directory.Exists(SP1Path))
                return true;

            byte[] masterKey = new byte[0];
            byte[] originalSetFile = new byte[0];
            byte[] EncSetFile = new byte[0];
            try
            {
                string[] initFileList = Directory.GetFiles(SP1Path);
                //DEK로 재암호화
                foreach (string setFilePath in initFileList)
                {
                    originalSetFile = File.ReadAllBytes(setFilePath);
                 
                    //DEK로 암호화
                    SGCrypto.AESEncrypt256WithDEK(originalSetFile, ref EncSetFile);

                    File.WriteAllBytes(setFilePath, EncSetFile);
                    //DEK로 암호화된 파일 이동
                    string name = Path.GetFileName(setFilePath);
                    File.Move(setFilePath, Path.Combine(saveDirPath, name), true);
                }

                //Directory.Delete(initDirPath);
                foreach (string fileList in Directory.GetFiles(SP1Path))
                {
                    File.Delete(fileList);
                }
                foreach (string dirList in Directory.GetDirectories(SP1Path))
                {
                    Directory.Delete(dirList);
                }
                Directory.Delete(SP1Path);

                return true;
            }
            catch (Exception ex)
            {
                HsLog.err("EncryptSettingFiles Exception : " + ex.ToString());
                return false;
            }
            finally
            {
                masterKey.hsClear(3);
                originalSetFile.hsClear(3);
                EncSetFile.hsClear(3);
            }
        }



    }
}