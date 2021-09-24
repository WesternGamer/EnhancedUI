using System.IO;

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
    }
}