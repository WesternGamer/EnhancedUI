using System.IO;

namespace EnhancedUI.Utils
{
    public static class FileSystem
    {
        public static string GetPluginsDir()
        {
            System.Reflection.Assembly? executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string? dllDirectory = Path.GetDirectoryName(executingAssembly.Location);
            return dllDirectory ?? ".";
        }
    }
}