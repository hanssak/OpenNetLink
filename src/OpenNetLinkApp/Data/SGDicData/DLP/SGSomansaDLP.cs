using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace OpenNetLinkApp.Data.SGDicData.DLP
{
    class SGSomansaDLP
    {
        String m_strRst;
        public const string strSomansaLibName = "PIScanLibCOM.dll";
        [DllImport(strSomansaLibName)]
        public static extern string ScanPrivacy(string sPath);
        public static object COMCreateObject(string sProgID)
        {
            Type oType = Type.GetTypeFromProgID(sProgID);
            if (oType != null)
            {
                return Activator.CreateInstance(oType);
            }
            return null;
        }
        public static void DlpCheck(string filePath)
        {
            if(!Directory.Exists("TempDLP"))
            {
                Directory.CreateDirectory("TempDLP");
            }

            string tempPath = Path.Combine("TempDLP", Path.GetFileName(filePath));
            File.Copy(filePath, tempPath, true);

            try
            {
                

                Type objClassType = Type.GetTypeFromProgID("PIScanLibCom.PIScanLib.1");

                if (objClassType != null)
                {
                    object obj = Activator.CreateInstance(objClassType);
                    object value = obj.GetType().InvokeMember("ScanPrivacy", System.Reflection.BindingFlags.InvokeMethod, null, obj, new object[] { filePath });

                    // Clean up
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);

                    obj = null;
                }
            }
            catch(Exception ex)
            {

            }
            finally
            {
                if (Directory.Exists("TempDLP"))
                    Directory.Delete("TempDLP", true);
            }
        }
        public static void DlpCheck1(string filePath)
        {
            string str = ScanPrivacy(filePath);
        }
    }
}
