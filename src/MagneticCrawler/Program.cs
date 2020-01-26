using MagneticCrawler.Crawlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MagneticCrawler
{
    public class Program
    {
        private static Mutex _mutex = null;

        [STAThread]
        public static void Main(string[] args)
        {
            //new CiLiNingMengCrawler().Start("钢铁侠");
            //Thread.Sleep(1000000);

            _mutex = new Mutex(true, "DB74DA6D-F0AA-4E9B-8965-C2C3DDD1EE58", out bool nothasinstance);
            if (!nothasinstance)
            {
                return;
            }

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
