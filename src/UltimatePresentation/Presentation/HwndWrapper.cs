using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UltimatePresentation.Native;
using UltimatePresentation.Native.Win32;

namespace UltimatePresentation.Presentation
{
        public abstract class HwndWrapper : DisposableObject
        {
                private IntPtr handle;
                private bool isHandleCreationAllowed = true;
                private ushort wndClassAtom;
                private Delegate wndProc;
                private static long failedDestroyWindows;
                private static int lastDestroyWindowError;
                protected ushort WindowClassAtom
                {
                        get
                        {
                                if (this.wndClassAtom == 0)
                                {
                                        this.wndClassAtom = this.CreateWindowClassCore();
                                }
                                return this.wndClassAtom;
                        }
                }
                public IntPtr Handle
                {
                        get
                        {
                                this.EnsureHandle();
                                return this.handle;
                        }
                }
                protected virtual bool IsWindowSubclassed
                {
                        get
                        {
                                return false;
                        }
                }
                protected virtual ushort CreateWindowClassCore()
                {
                        return this.RegisterClass(Guid.NewGuid().ToString());
                }
                protected virtual void DestroyWindowClassCore()
                {
                        if (this.wndClassAtom != 0)
                        {
                                IntPtr moduleHandle = NativeMethodsUltimate.GetModuleHandle(null);
                                NativeMethodsUltimate.UnregisterClass(new IntPtr((int)this.wndClassAtom), moduleHandle);
                                this.wndClassAtom = 0;
                        }
                }
                protected ushort RegisterClass(string className)
                {
                        WNDCLASS wNDCLASS = default(WNDCLASS);
                        wNDCLASS.cbClsExtra = 0;
                        wNDCLASS.cbWndExtra = 0;
                        wNDCLASS.hbrBackground = IntPtr.Zero;
                        wNDCLASS.hCursor = IntPtr.Zero;
                        wNDCLASS.hIcon = IntPtr.Zero;
                        wNDCLASS.lpfnWndProc = (this.wndProc = new NativeMethodsUltimate.WndProc(this.WndProc));
                        wNDCLASS.lpszClassName = className;
                        wNDCLASS.lpszMenuName = null;
                        wNDCLASS.style = 0u;
                        return NativeMethodsUltimate.RegisterClass(ref wNDCLASS);
                }
                private void SubclassWndProc()
                {
                        this.wndProc = new NativeMethodsUltimate.WndProc(this.WndProc);
                        NativeMethodsUltimate.SetWindowLong(this.handle, -4, Marshal.GetFunctionPointerForDelegate(this.wndProc));
                }
                protected abstract IntPtr CreateWindowCore();
                protected virtual void DestroyWindowCore()
                {
                        if (this.handle != IntPtr.Zero)
                        {
                                if (!NativeMethodsUltimate.DestroyWindow(this.handle))
                                {
                                        HwndWrapper.lastDestroyWindowError = Marshal.GetLastWin32Error();
                                        HwndWrapper.failedDestroyWindows += 1L;
                                }
                                this.handle = IntPtr.Zero;
                        }
                }
                protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
                {
                        return NativeMethodsUltimate.DefWindowProc(hwnd, msg, wParam, lParam);
                }
                public void EnsureHandle()
                {
                        if (this.handle == IntPtr.Zero)
                        {
                                if (!this.isHandleCreationAllowed)
                                {
                                        //VSDebug.Fail_Tagged("HWWR0001", "HwndWrapper.EnsureHandle should not be called when not allowed. Because of the current state, a new HWND will not be created", "f:\\dd\\env\\shell\\PackageFramework\\Current\\Shell\\UI\\Common\\HwndWrapper.cs", 152u);
                                        return;
                                }
                                this.isHandleCreationAllowed = false;
                                this.handle = this.CreateWindowCore();
                                if (this.IsWindowSubclassed)
                                {
                                        this.SubclassWndProc();
                                }
                        }
                }
                protected override void DisposeNativeResources()
                {
                        this.isHandleCreationAllowed = false;
                        this.DestroyWindowCore();
                        this.DestroyWindowClassCore();
                }
        }

}
