using System.Linq;

namespace FTPSync
{
    class Program
    {
        static void Main(string[] args)
        {
            var setBaseline = args.Contains("/b");

            var engine = new SyncEngine();
            engine.PerformSync(setBaseline);
        }
    }
}
