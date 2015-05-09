using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Limilabs.FTP.Client;

namespace FTPSync
{
    public class Client : IDisposable
    {
        private readonly Ftp ftp;

        public Client(string host, int port, string username, string password, string hostkey)
        {
            ftp = new Ftp();
            
            Port = port;
            Host = host;
            Username = username;
            Password = password;

            ServicePointManager.ServerCertificateValidationCallback = delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
            ftp.ServerCertificateValidate += delegate(object sender, ServerCertificateValidateEventArgs e)
            {
                const SslPolicyErrors ignoredErrors = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch;
                if ((e.SslPolicyErrors & ~ignoredErrors) == SslPolicyErrors.None)
                {
                    e.IsValid = true;
                    return;
                }
                e.IsValid = false;
            };
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public void Connect()
        {
            ftp.Connect(Host, Port);
            ftp.AuthTLS();
            ftp.Login(Username, Password);
        }

        public List<RemoteFileInfo> GetFiles(bool hiddenFiles, string path = null, bool recursive = true)
        {
            var files = new List<RemoteFileInfo>();
            var toParse = new Stack<string>();
            toParse.Push(path);

            while (toParse.Count > 0)
            {
                var currentPath = toParse.Pop();
                var dirInfo = ftp.GetList(RemoteFileInfo.EncodePath(currentPath));

                foreach (var file in dirInfo.Where(o => o.IsFolder))
                {
                    toParse.Push(PathCombine(currentPath, file.Name));
                }
                files.AddRange(dirInfo.Where(o => o.IsFile).Select(z => new RemoteFileInfo(z.Name, PathCombine(currentPath, z.Name))));
            }

            return files;
        }

        public void DownloadFile(string name, string targetPath)
        {
            ftp.Download(name, targetPath);
        }

        public void Disconnect()
        {
        }

        public void Dispose()
        {
            ftp.Dispose();
        }

        private string PathCombine(string directory, string filename)
        {
            if (true) // If unix
            {
                return Path.Combine(directory, filename).Replace("\\", "/");
            }
        }
    }
}
