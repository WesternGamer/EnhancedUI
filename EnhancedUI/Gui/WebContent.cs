using System.IO;
using System.Web;
using EnhancedUI.Utils;

namespace EnhancedUI.Gui
{
    public class WebContent
    {
        private readonly string baseUrl;

        public WebContent()
        {
            /* Document and resources are loaded from the Content folder next to the plugin DLL.
             * If no Content folder found, then from the development server: http://127.0.0.1:3000
             */
            var contentDir = Path.Combine(FileSystem.GetPluginFolderDir(), "Content");
            baseUrl = Directory.Exists(contentDir)
                ? "file://" + HttpUtility.UrlPathEncode(contentDir.Replace('\\', '/'))
                : "http://127.0.0.1:3000";
        }

        public string FormatIndexUrl(string name)
        {
            return $"{baseUrl}/{name}.html";
        }
    }
}