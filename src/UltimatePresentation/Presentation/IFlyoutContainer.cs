using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimatePresentation.Presentation
{
        /// <summary>
        /// 提供可显示与移除Flyout的容器
        /// </summary>
        public interface IFlyoutContainer
        {
                Flyout TopFlyout { get; }
                void ShowFlyout(Flyout flyout);
                void CloseFlyout(object content);
                void CloseFlyout();
        }
}
