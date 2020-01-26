using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimatePresentation.Presentation
{
        /// <summary>
        /// 提供可显示与移除Dialog的容器
        /// </summary>
        public interface IDialogContainer
        {
                Dialog TopDialog { get; }
                void ShowDialog(Dialog dialog);
                void CloseDialog(object content);
                void CloseTopDialog();
        }
}
