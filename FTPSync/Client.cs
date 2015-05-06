using System;
using System.Collections.Generic;
using System.Linq;
using WinSCP;

namespace FTPSync
{
    public class Client : IDisposable
    {
        private SessionOptions options;
        private Session session;

        public Client(string host, int port, string username, string password, string hostkey)
        {
             // Setup session options
            this.options = new SessionOptions
            {
                Protocol = Protocol.Sftp,
                HostName = host,
                PortNumber = port,
                UserName = username,
                Password = password,
                SshHostKeyFingerprint = hostkey
            };

            session = new Session();
        }

        public void Connect()
        {
            session.Open(this.options);
        }

        public List<RemoteFileInfo> GetFiles(bool hiddenFiles, string path = null, bool recursive = true)
        {
            ////using (var fileListClient = new SClient(ConnectionInfo))
            ////{
            ////    fileListClient.Connect();
            ////    var quickList = fileListClient.GetFiles(true, path).ToList();
            ////    fileListClient.Disconnect();
            ////}
            var files = new List<WinSCP.RemoteFileInfo>();
            var toParse = new Stack<string>();
            toParse.Push(path);

            while (toParse.Count > 0)
            {
                var dirInfo = session.ListDirectory(toParse.Pop());
                
                foreach (var file in dirInfo.Files.Where(o => o.IsDirectory && !o.Name.Equals(".") && !o.Name.Equals("..") && !o.Name.StartsWith(".")))
                {
                    toParse.Push(file.Name);
                }

                files.AddRange(dirInfo.Files.Where(o => !o.IsDirectory));
            }


            return files.Select(o => new RemoteFileInfo(o)).ToList();
        }

        public void DownloadFile(string name, string targetPath)
        {
            var tr = new TransferOptions()
            {
                PreserveTimestamp = true,
                TransferMode = TransferMode.Binary
            };
            this.session.GetFiles(name, targetPath, true, tr);
        }

        public void Disconnect()
        {
            session.Abort();
        }

        public void Dispose()
        {
            session.Dispose();
        }
    }
}
