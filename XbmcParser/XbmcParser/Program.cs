using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace XbmcParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter User Name and Password for ssh connection to Xbmc server.");

            Console.Write("User Name:");
            var userName = Console.ReadLine();

            Console.Write("Password:");

            var password = GetPassword();

            var copiedFile = ScpLogFile(userName, password);

            var unmatchedFiles = ExtractUnmatchedFilesForSmb(copiedFile);

            Console.Write(unmatchedFiles);
            
            Console.ReadKey();
        }

        public static string GetPassword()
        {
            var password = new StringBuilder();
            while (true)
            {
                var i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if (i.Key == ConsoleKey.Backspace)
                {
                    password.Remove(0, (password.Length - 1));
                    Console.Write("\b \b");
                }
                else
                {
                    password.Append(i.KeyChar);;
                    Console.Write("*");
                }
            }
            return password.ToString();
        }

        private static string ScpLogFile(string userName, string password)
        {
            var hostName = ConfigurationManager.AppSettings["HostName"];
            var sshHostKeyFingerprint = ConfigurationManager.AppSettings["SshHostKeyFingerprint"];
            var xbmcLogFileSource = ConfigurationManager.AppSettings["XbmcLogFileSource"];
            var xbmcLogFileDestination = ConfigurationManager.AppSettings["XbmcLogFileDestination"];
            var retryCount = int.Parse(ConfigurationManager.AppSettings["RetryCount"]);

            IScpCommands scpGateway = new ScpGateway(userName, password, sshHostKeyFingerprint, hostName);
            scpGateway = new TimeOutDecorator(scpGateway, retryCount);

            scpGateway.ScpFile(xbmcLogFileSource, xbmcLogFileDestination);

            return xbmcLogFileDestination;
        }

        private static string ExtractUnmatchedFilesForSmb(string xbmcLogFileDestination)
        {
            var xbmcLogSearchText = ConfigurationManager.AppSettings["XbmcLogSearchText"];

            var lines = File.ReadAllLines(xbmcLogFileDestination);
            var number = 0;

            var unmatchedFileNames = new StringBuilder();
            foreach (var line in lines.Where(line => line.Contains(xbmcLogSearchText)))
            {
                number += 1;
                var firstCommaIndex = line.IndexOf("smb", StringComparison.Ordinal);
                var secondCommaIndex = line.IndexOf('\'', firstCommaIndex);
                var extractedString = line.Substring(firstCommaIndex, secondCommaIndex - firstCommaIndex);
                var spiltArray = extractedString.Split('/');

                unmatchedFileNames.AppendLine(String.Format("{0}: {1}", number, spiltArray[spiltArray.Length - 1]));
            }

            return unmatchedFileNames.ToString();
        }
    }
}
