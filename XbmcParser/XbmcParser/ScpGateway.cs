using System;
using WinSCP;

namespace XbmcParser
{
    public class ScpGateway : IScpCommands
    {
        private readonly SessionOptions sessionOptions;

        public ScpGateway(string userName, string password, string sshHostKeyFingerprint, string hostName)
        {
            sessionOptions = new SessionOptions
                            {
                                Protocol = Protocol.Scp,
                                HostName = hostName,
                                UserName = userName,
                                Password = password,
                                SshHostKeyFingerprint = sshHostKeyFingerprint
                            };
        }

        public void ScpFile(string sourceFile, string destinationFile)
        {
            using (var session = new Session())
            {
                session.Open(sessionOptions);

                var transferResult = session.GetFiles(sourceFile, destinationFile);

                if (transferResult.Failures.Count > 0)
                {
                    throw new ApplicationException(transferResult.Failures.ToString());
                }
            }
        }
    }
}
