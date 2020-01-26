using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using UltimatePresentation.Native;
using UltimatePresentation.Native.Win32;
using UltimatePresentation.Presentation;

namespace UltimatePresentation.Behaviours
{
	internal class WindowSettings
	{
		public static readonly DependencyProperty WindowPlacementSettingsProperty = DependencyProperty.RegisterAttached(
			"WindowPlacementSettings",
			typeof(IWindowPlacementSettings),
			typeof(WindowSettings),
			new FrameworkPropertyMetadata(OnWindowPlacementSettingsInvalidated));

		public static void SetSave(DependencyObject dependencyObject, IWindowPlacementSettings windowPlacementSettings)
		{
			dependencyObject.SetValue(WindowPlacementSettingsProperty, windowPlacementSettings);
		}

		private static void OnWindowPlacementSettingsInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			var window = dependencyObject as Window;
			if (window == null || !(e.NewValue is IWindowPlacementSettings))
				return;

			var windowSettings = new WindowSettings(window, (IWindowPlacementSettings)e.NewValue);
			windowSettings.Attach();
		}

		private Window _window;
		private IWindowPlacementSettings _settings;

		public WindowSettings(Window window, IWindowPlacementSettings windowPlacementSettings)
		{
			_window = window;
			_settings = windowPlacementSettings;
		}

		protected virtual void LoadWindowState()
		{
			if (_settings == null) return;
			_settings.Reload();

			if (_settings.Placement == null)
				return;

			try
			{
				var wp = _settings.Placement;

				wp.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
				wp.flags = 0;
				wp.showCmd = (wp.showCmd == Constants.SW_SHOWMINIMIZED ? Constants.SW_SHOWNORMAL : wp.showCmd);
				var hwnd = new WindowInteropHelper(_window).Handle;
				NativeMethodsUltimate.SetWindowPlacement(hwnd, wp);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Failed to load window state:\r\n{0}", ex);
			}
		}

		protected virtual void SaveWindowState()
		{
			if (_settings == null) return;
			WINDOWPLACEMENT wp;
			var hwnd = new WindowInteropHelper(_window).Handle;
			wp = NativeMethodsUltimate.GetWindowPlacement(hwnd);
			_settings.Placement = wp;
			_settings.Save();
		}

		private void Attach()
		{
			if (_window == null) return;
			_window.Closing += WindowClosing;
			//SourceInitialized事件会引起窗口轻微闪烁，不知为何
			_window.SourceInitialized += WindowSourceInitialized;
			//_window.Loaded += WindowSourceInitialized;
		}

		void WindowSourceInitialized(object sender, EventArgs e)
		{
			LoadWindowState();
		}

		private void WindowClosing(object sender, CancelEventArgs e)
		{
			SaveWindowState();
			_window.Closing -= WindowClosing;
			_window.SourceInitialized -= WindowSourceInitialized;
			_window = null;
			_settings = null;
		}
	}
}
