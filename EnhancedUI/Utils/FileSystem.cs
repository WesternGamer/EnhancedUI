using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace EnhancedUI.Utils
{
    public static class FileSystem
    {
        public static string GetPluginsDir()
        {
            var executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var dllDirectory = Path.GetDirectoryName(executingAssembly.Location);
            return dllDirectory ?? ".";
        }

        public static string GetRandomFileFromDir(string path)
        {
            string file = null;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var di = new DirectoryInfo(path);
                    var rgFiles = di.GetFiles("*.*");
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] data = new byte[4];
                    rng.GetBytes(data);
                    int value = BitConverter.ToInt32(data, 0);
                    Random R = new Random(value);
                    file = rgFiles.ElementAt(R.Next(0, rgFiles.Count())).FullName;
                }
                catch
                {
                }
            }
            return file;
        }
    }
}