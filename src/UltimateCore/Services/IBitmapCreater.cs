using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UltimateCore.Services
{
        public interface IBitmapCreater
        {
                ImageSource CreateBitmap(object param);
                ImageSource CreateBitmap(object param, bool iscache);
        }
}
