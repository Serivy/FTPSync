////using System;
////using System.Collections.Generic;
////using System.Configuration;
////using System.Linq;
////using System.Net.Sockets;
////using System.Text;
////using System.Threading.Tasks;
////using Renci.SshNet;
////using Renci.SshNet.Sftp;

////namespace SFTPS
////{
////    public class Client : SftpClient
////    {
////        public Client(ConnectionInfo connectionInfo) : base(connectionInfo)
////        {

////        }

////        public Client(string host, int port, string username, string password) : base(host, port, username, password)
////        {
////        }

////        public Client(string host, string username, string password) : base(host, username, password)
////        {
////        }

////        public Client(string host, int port, string username, params PrivateKeyFile[] keyFiles) : base(host, port, username, keyFiles)
////        {
////        }

////        public Client(string host, string username, params PrivateKeyFile[] keyFiles) : base(host, username, keyFiles)
////        {
////        }

////        public List<RemoteFileInfo> GetFiles(bool hiddenFiles, string path = null, bool recursive = true)
////        {
////            ////using (var fileListClient = new SClient(ConnectionInfo))
////            ////{
////            ////    fileListClient.Connect();
////            ////    var quickList = fileListClient.GetFiles(true, path).ToList();
////            ////    fileListClient.Disconnect();
////            ////}
////            var files = new List<SftpFile>();
////            var toParse = new Stack<string>();
////            toParse.Push(path);

////            while (toParse.Count > 0)
////            {
////                var task = ListDirectoryAsync(toParse.Pop());
////                task.Wait();
////                foreach (var file in task.Result.Where(o => o.IsDirectory && !o.Name.Equals(".") && !o.Name.Equals("..") && !o.Name.StartsWith(".")))
////                {
////                    toParse.Push(file.FullName);
////                }

////                files.AddRange(task.Result.Where(o => o.IsRegularFile));
////            }

////            return files.Select(o => new RemoteFileInfo(o)).ToList();
////        }

////        public Task<IEnumerable<SftpFile>> ListDirectoryAsync(string path)
////        {
////            var tcs = new TaskCompletionSource<IEnumerable<SftpFile>>();
////            this.BeginListDirectory(path, iar =>
////            {
////                try { tcs.TrySetResult(EndListDirectory(iar)); }
////                catch (OperationCanceledException) { tcs.TrySetCanceled(); }
////                catch (Exception exc) { tcs.TrySetException(exc); }
////            }, null);
////            return tcs.Task;
////        }
////    }
////}
