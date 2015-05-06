using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTPSync
{
    public class SyncedRecordKeeper
    {
        private readonly string file;
        private List<string> syncedFiles;

        public SyncedRecordKeeper(string file)
        {
            this.file = file;
            Read();
        }

        public bool FileAlreadyDownloaded(string checkFile)
        {
            return syncedFiles.Contains(checkFile);
        }

        public void AddFile(string newFile)
        {
            syncedFiles.Add(newFile);
            Write();
        }

        private void Write()
        {
            File.WriteAllLines(this.file, syncedFiles);
        }

        private void Read()
        {
            if (File.Exists(this.file))
            {
                syncedFiles = File.ReadAllLines(this.file).ToList();
            }
            else
            {
                syncedFiles = new List<string>();
            }
        }
    }
}
