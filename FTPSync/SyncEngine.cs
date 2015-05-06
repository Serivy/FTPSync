using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace FTPSync
{
    public class SyncEngine
    {
        private SyncedRecordKeeper records;
        private Logger log;
        private readonly string metaFolder;

        public SyncEngine()
        {
            var destination = ConfigurationManager.AppSettings["destination"];
            metaFolder = Path.Combine(destination, ".sftps");
            log = new Logger(metaFolder);
        }

        public void PerformSync(bool baseline = false)
        {
            var host = ConfigurationManager.AppSettings["host"];
            var port = int.Parse(ConfigurationManager.AppSettings["port"]);
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];
            var source = ConfigurationManager.AppSettings["source"];
            var destination = ConfigurationManager.AppSettings["destination"];
            var hostKey = ConfigurationManager.AppSettings["hostkey"]; 

            if (!Directory.Exists(metaFolder))
            {
                Directory.CreateDirectory(metaFolder);
                DirectoryInfo info = Directory.CreateDirectory(metaFolder); 
                info.Attributes = FileAttributes.Directory | FileAttributes.Hidden; 
            }

            records = new SyncedRecordKeeper(Path.Combine(metaFolder, "synced"));
            try
            {
                using (var client = new Client(host, port, username, password, hostKey))
                {
                    client.Connect();
                    var files = client.GetFiles(true, source).ToList();

                    foreach (var file in files)
                    {
                        if (!records.FileAlreadyDownloaded(file.FullName))
                        {
                            log.Log(string.Format("Downloading file {0}.", file.FullName));

                            // Download the file.
                            var targetFilePath = Path.Combine(destination, file.FileName);
                            client.DownloadFile(file.FullName, targetFilePath);
                            //using (var fileStream = File.Create(targetFilePath))
                            //{
                            //    client.DownloadFile(file.FullName, fileStream);
                            //}

                            records.AddFile(file.FullName);
                            log.Log(string.Format("File downloaded {0}.", targetFilePath));
                        }
                    }

                    log.Log(string.Format("{0} files parsed.", files.Count));

                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                log.Log(string.Format("Error: {0}.", ex.Message));
            }
        }
    }
}
