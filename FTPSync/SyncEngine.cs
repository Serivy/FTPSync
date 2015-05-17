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
        private Configuration userConfiguration;

        public SyncEngine()
        {
            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
            configFileMap.ExeConfigFilename = "User.Config";
            userConfiguration = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);

            var destination = GetConfig("destination");
            metaFolder = Path.Combine(destination, ".ftpsync");
            log = new Logger(metaFolder);
        }

        public void PerformSync(bool baseline = false)
        {
            var host = GetConfig("host");
            var port = int.Parse(GetConfig("port"));
            var username = GetConfig("username");
            var password = GetConfig("password");
            var source = GetConfig("source");
            var destination = GetConfig("destination");
            var hostKey = GetConfig("hostKey");

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

        private string GetConfig(string key)
        {
            var value = userConfiguration.AppSettings.Settings.AllKeys.Contains(key) ? userConfiguration.AppSettings.Settings[key].Value : ConfigurationManager.AppSettings[key];
            return value;
        }
    }
}
