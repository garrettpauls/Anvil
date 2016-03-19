using System.IO;
using System.Linq;

namespace Anvil.Framework.IO
{
    public static class DirectoryEx
    {
        public static string FirstExisting(params string[] directories)
        {
            return directories.FirstOrDefault(Directory.Exists);
        }
    }
}
