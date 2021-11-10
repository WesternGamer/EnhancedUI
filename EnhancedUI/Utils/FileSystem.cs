using System.IO;

namespace EnhancedUI.Utils
{
    public static class FileSystem
    {
        /// <summary>
        /// Gets the location of the plugin assembly file.
        /// </summary>
        /// <returns>Returns the directory of the folder where the plugin assembly file is located as a string.</returns>
        public static string GetPluginFolderDir()
        {
            System.Reflection.Assembly? executingAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            string? dllDirectory = Path.GetDirectoryName(executingAssembly.Location);
            return dllDirectory ?? ".";
        }
    }
}