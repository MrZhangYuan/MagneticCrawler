using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MagneticCrawler
{
        public static class AppInfo
        {
                private static Version version = Assembly.GetExecutingAssembly().GetName().Version;
                public static string AppVersion { get; } = $"v{version.Major}.{version.Minor}.{version.Revision}";

                public static string QQ { get; } = "874429156";
        }
}
