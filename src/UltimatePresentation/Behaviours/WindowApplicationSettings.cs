using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UltimatePresentation.Native.Win32;

namespace UltimatePresentation.Behaviours
{
	/// <summary>
	/// this settings class is the default way to save the placement of the window
	/// </summary>
	public class WindowApplicationSettings : ApplicationSettingsBase, IWindowPlacementSettings
	{
		public WindowApplicationSettings(Window window)
			: base(window.GetType().FullName)
		{
		}

		[UserScopedSetting]
		public WINDOWPLACEMENT Placement
		{
			get
			{
				if (this["Placement"] != null)
				{
					return ((WINDOWPLACEMENT)this["Placement"]);
				}
				return null;
			}
			set
			{
				this["Placement"] = value;
			}
		}
	}

}
