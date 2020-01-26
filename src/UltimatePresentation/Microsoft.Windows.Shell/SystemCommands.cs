
namespace Microsoft.Windows.Shell
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;
    using Standard;

    internal static class SystemCommands
    {
        public static RoutedCommand CloseWindowCommand { get; private set; }
        public static RoutedCommand MaximizeWindowCommand { get; private set; }
        public static RoutedCommand MinimizeWindowCommand { get; private set; }
        public static RoutedCommand RestoreWindowCommand { get; private set; }
        public static RoutedCommand ShowSystemMenuCommand { get; private set; }

        //后期添加
        public static RoutedCommand MoveWindowCommand { get; private set; }
        public static RoutedCommand SizeWindowCommand { get; private set; }

        static SystemCommands()
        {
            CloseWindowCommand = new RoutedCommand("CloseWindow", typeof(SystemCommands));
            MaximizeWindowCommand = new RoutedCommand("MaximizeWindow", typeof(SystemCommands));
            MinimizeWindowCommand = new RoutedCommand("MinimizeWindow", typeof(SystemCommands));
            RestoreWindowCommand = new RoutedCommand("RestoreWindow", typeof(SystemCommands));
            ShowSystemMenuCommand = new RoutedCommand("ShowSystemMenu", typeof(SystemCommands));

            MoveWindowCommand = new RoutedCommand("MoveWindow", typeof(SystemCommands));
            SizeWindowCommand = new RoutedCommand("SizeWindow", typeof(SystemCommands));       
        }

        private static void _PostSystemCommand(Window window, SC command)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !NativeMethodsShell.IsWindow(hwnd))
            {
                return;
            }

            NativeMethodsShell.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr((int)command), IntPtr.Zero);
        }

        public static void CloseWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.CLOSE);
        }

        public static void MaximizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MAXIMIZE);
        }

        public static void MinimizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MINIMIZE);
        }

        public static void RestoreWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.RESTORE);
        }

        public static void MoveWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.MOVE);
        }
        public static void SizeWindow(Window window)
        {
            Verify.IsNotNull(window, "window");
            _PostSystemCommand(window, SC.SIZE);
        }

        /// <summary>Display the system menu at a specified location.</summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="screenLocation">The location to display the system menu, in logical screen coordinates.</param>
        public static void ShowSystemMenu(Window window, Point screenLocation)
        {
            Verify.IsNotNull(window, "window");
            ShowSystemMenuPhysicalCoordinates(window, DpiHelper.LogicalPixelsToDevice(screenLocation));
        }

        internal static void ShowSystemMenuPhysicalCoordinates(Window window, Point physicalScreenLocation)
        {
            const uint TPM_RETURNCMD = 0x0100;
            const uint TPM_LEFTBUTTON = 0x0;

            Verify.IsNotNull(window, "window");
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero || !NativeMethodsShell.IsWindow(hwnd))
            {
                return;
            }

            IntPtr hmenu = NativeMethodsShell.GetSystemMenu(hwnd, false);

            uint cmd = NativeMethodsShell.TrackPopupMenuEx(hmenu, TPM_LEFTBUTTON | TPM_RETURNCMD, (int)physicalScreenLocation.X, (int)physicalScreenLocation.Y, hwnd, IntPtr.Zero);
            if (0 != cmd)
            {
                NativeMethodsShell.PostMessage(hwnd, WM.SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }
        }
    }
}
