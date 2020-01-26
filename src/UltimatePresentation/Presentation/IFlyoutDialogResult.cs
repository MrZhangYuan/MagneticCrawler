using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimatePresentation.Presentation
{
        public interface IFlyoutDialogResult
        {
                /// <summary>
                /// 对话框是否被取消了
                /// </summary>
                bool IsCanceled { get; set; }
        }
}
