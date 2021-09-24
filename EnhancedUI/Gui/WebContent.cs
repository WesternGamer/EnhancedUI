using System.IO;
using System.Web;
using EnhancedUI.Utils;

namespace EnhancedUI.Gui
{
    public class WebContent
    {
        private readonly string rootDir;
        private readonly string rootUrl;

        public WebContent()
        {
            /*  Static content files are loaded form the EnhancedUI folder.
             *
             *  In production it will be deployed by PluginLoader.
             *  In development it should be linked to a project or build folder.
             *
             */
            rootDir = Path.Combine(FileSystem.GetPluginsDir(), "EnhancedUI");
            rootUrl = "file://" + HttpUtility.UrlPathEncode(rootDir.Replace('\\', '/'));
        }

        private string FormatIndexPath(string name)
        {
            return Path.Combine(rootDir, $"{name}.html");
        }

        public bool HasIndex(string name)
        {
            var path = FormatIndexPath(name);
            return File.Exists(path);
        }

        public string FormatIndexUrl(string name)
        {
            return $"{rootUrl}/{name}.html";
        }
    }
}