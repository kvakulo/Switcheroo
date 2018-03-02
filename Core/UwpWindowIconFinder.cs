using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Switcheroo.Core
{
    public class UwpWindowIconFinder
    {
        public Icon Find(AppWindow uwpWindow)
        {
            var processPath = uwpWindow.ExecutablePath;
            var directoryPath = Path.GetDirectoryName(processPath);
            var directoryName = Path.GetFileName(directoryPath);
            var manifest = XDocument.Parse(File.ReadAllText(Path.Combine(directoryPath, "AppxManifest.xml")));
            var ns = manifest.Root.Name.Namespace;
            var logoPath = manifest.Root.Element(ns + "Properties").Element(ns + "Logo").Value;
            var name = manifest.Root.Element(ns + "Identity").Attribute("Name").Value;

            var executable = Path.GetFileName(processPath);

            var application = manifest.Root
                .Element(ns + "Applications")
                .Elements(ns + "Application")
                .FirstOrDefault(e => executable.Equals(e.Attribute("Executable").Value, StringComparison.InvariantCultureIgnoreCase));

            if (application != null)
            {
                XNamespace uapNs = "http://schemas.microsoft.com/appx/manifest/uap/windows10";

                var visualElements = application.Element(uapNs + "VisualElements");

                var attribute = visualElements.Attribute("Square44x44Logo");

                if (attribute != null)
                {
                    logoPath = attribute.Value;
                }
            }

            var resourcePath = "@{" + directoryName + "?ms-resource://" + name + "/Files/" + logoPath.Replace("\\", "/") + "}";

            var logoFullPath = ExtractNormalPath(resourcePath);

            if (File.Exists(logoFullPath))
            {
                var bitmap = new Bitmap(logoFullPath);
                var iconHandle = bitmap.GetHicon();
                return Icon.FromHandle(iconHandle);
            }

            return Icon.ExtractAssociatedIcon(processPath);
        }

        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

        public string ExtractNormalPath(string indirectString)
        {
            StringBuilder outBuff = new StringBuilder(1024);
            int result = SHLoadIndirectString(indirectString, outBuff, outBuff.Capacity, IntPtr.Zero);

            return outBuff.ToString();
        }
    }
}
