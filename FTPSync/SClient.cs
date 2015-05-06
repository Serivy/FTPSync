////using System;
////using System.Collections.Generic;
////using System.Configuration;
////using System.Linq;
////using System.Text;
////using Renci.SshNet;

////namespace SFTPS
////{
////    public class SClient : SshClient
////    {
////        public SClient(ConnectionInfo connectionInfo)
////            : base(connectionInfo)
////        {
////        }

////        public SClient(string host, int port, string username, string password)
////            : base(host, port, username, password)
////        {
////        }

////        public SClient(string host, string username, string password)
////            : base(host, username, password)
////        {
////        }

////        public SClient(string host, int port, string username, params PrivateKeyFile[] keyFiles)
////            : base(host, port, username, keyFiles)
////        {
////        }

////        public SClient(string host, string username, params PrivateKeyFile[] keyFiles)
////            : base(host, username, keyFiles)
////        {
////        }

////        public IEnumerable<string> GetFilesTest(bool hiddenFiles, string path = null)
////        {
////            if (path != null)
////            {
////                var cdResult = RunCommand("cd " + path);
////                if (cdResult.ExitStatus != 0)
////                {
////                    throw new Exception("Could not change directory." + Environment.NewLine + cdResult.Result);
////                }
////            }

////            var result = RunCommand(@"find . \( ! -regex '.*/\..*' \) -type f -exec md5sum {} \;" + (hiddenFiles ? "A" : string.Empty));
////            string baseDir = string.Empty;
////            foreach (var cmd in result.Result.Split('\n'))
////            {
////                if (cmd.EndsWith(":"))
////                {
////                    baseDir = cmd.Replace(":", "/");
////                }
////                else if (!string.IsNullOrEmpty(cmd) && (!cmd.EndsWith("/")))
////                {
////                     Else if it is a file listing
////                    yield return baseDir + cmd;
////                }
////            }
////        }

////        public IEnumerable<string> GetFiles(bool hiddenFiles, string path = null)
////        {
////            var cmdText = (path != null ? "cd " + path + "; " : string.Empty);
////            cmdText += "ls --file-type -F1R";
////            cmdText += (hiddenFiles ? "A" : string.Empty);

////            var result = RunCommand(cmdText);
////            if (result.ExitStatus != 0)
////            {
////                throw new Exception("Could process command." + Environment.NewLine + result.Result);
////            }

////            string baseDir = string.Empty;
////            foreach (var cmd in result.Result.Split('\n'))
////            {
////                if (cmd.EndsWith(":"))
////                {
////                    baseDir = cmd.Replace(":", "/");
////                } 
////                else if (!string.IsNullOrEmpty(cmd) && (!cmd.EndsWith("/")))
////                {
////                     Else if it is a file listing
////                    yield return baseDir + cmd;
////                }
////            }
////        }

////        public bool DownloadFile(string source, string destination)
////        {
////            var cmdText = "get " + source;

////            var result = RunCommand(cmdText);
////            if (result.ExitStatus != 0)
////            {
////                throw new Exception("Could process command." + Environment.NewLine + result.Result);
////            }

////            return true;
////        }
////    }
////}
