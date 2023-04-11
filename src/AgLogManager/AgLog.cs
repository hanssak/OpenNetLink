using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;

namespace AgLogManager
{
    public static class AgLog
    {
        public static LoggingLevelSwitch LogLevelSwitch {get; set; } = new LoggingLevelSwitch();

        public static Serilog.ILogger Here(
            this Serilog.ILogger logger,
            [CallerMemberName] string memberName = "",   
            [CallerFilePath] string filePath = "",    
            [CallerLineNumber] int lineNumber = 0)
        {
            return logger.ForContext("MemberName", "in method " + memberName)
                .ForContext("FilePath", "at " + Path.GetFileName(filePath))
                .ForContext("FileName", Path.GetFileNameWithoutExtension(filePath))
                .ForContext("LineNumber", ":" + lineNumber);
        }

        public static string Dump(byte[] bytes, int bytesPerLine = 16)
        {
            if (bytes == null) return "<null>";
            int bytesLength = bytes.Length;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                    8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - Environment.NewLine.Length) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
            }
            return result.ToString();
        }
    }

    public class OperationIdEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {     
            if (logEvent.Properties.TryGetValue("RequestId", out var requestId))
                logEvent.AddPropertyIfAbsent(new LogEventProperty("operationId", requestId));
        }
    }

    public class HsLogDel
    {
        public bool Delete(int nDay = 7)
        {
            string strDelDay = DateTime.Now.AddDays(-1 * nDay).ToString("yyyyMMdd");
            string strDelFileName = "SecureGate-" + strDelDay;

            string strPath = System.IO.Path.Combine(System.Environment.CurrentDirectory, "wwwroot");
            strPath = System.IO.Path.Combine(strPath, "Log");
            System.IO.Directory.CreateDirectory(strPath);
            //strPath = System.IO.Path.Combine(strPath, "SecureGate-{Date}.Log");

            try
            {
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(strPath);
                foreach (System.IO.FileInfo File in di.GetFiles())
                {

                    if (File.Extension.Length > 0)
                    {
                        string strFileNameOnly = File.Name.Substring(0, File.Name.Length - File.Extension.Length);
                        string strFileDayinfo = "";

                        // 공통Log 삭제 : "SecureGate-20220117.log" 형태 Log 파일들 삭제
                        if (strFileNameOnly.Contains("SecureGate-") && strFileNameOnly.Length >= 19)
                        {
                            strFileDayinfo = strFileNameOnly.Substring(11,8);
                            if (strDelDay.CompareTo(strFileDayinfo) >= 0)
                            {
                                // Log 파일들 삭제
                                System.IO.File.Delete(File.FullName);
                                Log.Logger.Here().Information($"Log File Delete : [{File.FullName}]");
                            }
                            // OS별 Log 삭제 : "SecureGate-WinDLL-20220117.log" 형태 Log 파일들 삭제
                            else if (strFileNameOnly.Contains("SecureGate-WinDLL-"))
                            {
                                strFileDayinfo = strFileNameOnly.Substring(18);
                                if (strDelDay.CompareTo(strFileDayinfo) >= 0)
                                {
                                    // Log 파일들 삭제
                                    System.IO.File.Delete(File.FullName);
                                    Log.Logger.Here().Information($"Log File Delete : [{File.FullName}]");
                                }
                            }
                        }

                    }
                }
            }
            catch (DirectoryNotFoundException dirNotFound)
            {
                // dirNotFound.Message
                Log.Logger.Here().Information($"Log File Delete error try-catch (Message) : [{dirNotFound.Message}]");
            }

            return true;
        }
    }

}