namespace FTPSync
{
    public class RemoteFileInfo
    {
        public RemoteFileInfo(string filename, string fullname)
        {
            FileName = filename;
            FullName = fullname;
        }

        public string FullName { get; set; }

        public string FileName { get; set; }

        public string EncodedPath()
        {
            return EncodePath(FullName);
        }

        public static string EncodePath(string path)
        {
            return path.Replace("[", "\\[").Replace("]", "\\]");
        }
    }
}
