
using System;

namespace XbmcParser
{
    public class TimeOutDecorator : IScpCommands
    {
        readonly IScpCommands someInterfaceinterface;
        private int retryCount;

        public TimeOutDecorator(IScpCommands someInterfaceinterface, int retryCount)
        {
            this.someInterfaceinterface = someInterfaceinterface;
            this.retryCount = retryCount;
        }

        public void ScpFile(string sourceFile, string destinationFile)
        {
            try
            {
                someInterfaceinterface.ScpFile(sourceFile, destinationFile);
            }
            catch (Exception)
            {
                if (retryCount > 0)
                {
                    retryCount -= 1;
                    ScpFile(sourceFile, destinationFile);
                }

                throw;
            }
            
        }
    }
}
