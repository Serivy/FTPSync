namespace FTPSync
{
    public class RemoteFileInfo
    {
        public RemoteFileInfo(WinSCP.RemoteFileInfo file)
        {
            FileName = file.Name;
            FullName = file.Name;
        }

        public string FullName { get; set; }

        public string FileName { get; set; }
    }
}
