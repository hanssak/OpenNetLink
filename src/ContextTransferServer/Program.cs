using System;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

class Program
{
    static string _socketName = "testd.sock";
    static string _linuxTempPath = "/var/tmp";
    static string _winTempPath = "%TEMP%";

    static string _socketPath;

    // C# - .NET Core 2.0 이하의 Unix Domain Socket 사용 시 System.IndexOutOfRangeException 오류
    // http://www.sysnet.pe.kr/2/0/11999


    static void Main(string[] args)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            string tempPath = Path.Combine(_winTempPath, _socketName);
            _socketPath = Environment.ExpandEnvironmentVariables(tempPath);
        }
        else
        {
            _socketPath = Path.Combine(_linuxTempPath, _socketName);
        }

        Console.WriteLine("[Server] Thread is started.");

        if (File.Exists(_socketPath) == true)
        {
            File.Delete(_socketPath);
        }

        try
        {
            using (var socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP))
            {
                var unixEp = new UnixDomainSocketEndPoint(_socketPath);

                // Under .NET Core 2.1, you can use user-defined UnixEndPoint type
                // var unixEp = new UnixEndPoint(_socketPath);

                socket.Bind(unixEp);
                socket.Listen(5);
                while (true)
                {
                    using (Socket clntSocket = socket.Accept())
                    {
                        Console.WriteLine("[Server] ClientConencted");
                        NetworkStream networkStream = new NetworkStream(clntSocket);
                        if(networkStream.CanRead){
                            byte[] ReadBuffer = new byte[1024];
                            StringBuilder CompleteMessage = new StringBuilder();
                            int numberOfBytesRead = 0;

                            // Incoming message may be larger than the buffer size.
                            do{
                                numberOfBytesRead = networkStream.Read(ReadBuffer, 0, ReadBuffer.Length);

                                CompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(ReadBuffer, 0, numberOfBytesRead));
                            }
                            while(networkStream.DataAvailable);

                            // Print out the received message to the console.
                            Console.WriteLine("You received the following message : " +
                                                        CompleteMessage);
                        }
                        else{
                            Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
                        }
                    }
                    Console.WriteLine("[Server] ClientClosed");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}