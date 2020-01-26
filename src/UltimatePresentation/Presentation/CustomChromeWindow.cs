using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using UltimatePresentation.Native;
using UltimatePresentation.Native.Win32;

namespace UltimatePresentation.Presentation
{
        public class CustomChromeWindow : Window
        {
                /// <summary>
                /// 剪裁区域更改类型
                /// </summary>
                protected enum ClipRegionChangeType
                {
                        FromSize,
                        FromPosition,
                        FromPropertyChange,
                        FromUndockSingleTab
                }
                /// <summary>
                /// 变化范围
                /// </summary>
                private class ChangeScope : DisposableObject
                {
                        private readonly CustomChromeWindow _window;
                        public ChangeScope(CustomChromeWindow window)
                        {
                                this._window = window;
                                this._window._deferGlowChangesCount++;
                        }
                        protected override void DisposeManagedResources()
                        {
                                this._window._deferGlowChangesCount--;
                                if (this._window._deferGlowChangesCount == 0)
                                {
                                        this._window.EndDeferGlowChanges();
                                }
                        }
                }
                /// <summary>
                /// 阴影位图
                /// </summary>
                private sealed class GlowBitmap : DisposableObject
                {
                        private static readonly string _bitmapRootPath = "Resources/Bitmaps/Window/";
                        /// <summary>
                        /// 缓存的位图信息
                        /// </summary>
                        private sealed class CachedBitmapInfo
                        {
                                public readonly int Width;
                                public readonly int Height;
                                public readonly byte[] DIBits;
                                public CachedBitmapInfo(byte[] diBits, int width, int height)
                                {
                                        this.Width = width;
                                        this.Height = height;
                                        this.DIBits = diBits;
                                }
                        }
                        public const int GlowBitmapPartCount = 16;
                        private const int BytesPerPixelBgra32 = 4;

                        private static readonly CustomChromeWindow.
                                        GlowBitmap.
                                        CachedBitmapInfo[] _transparencyMasks =
                                        new CustomChromeWindow.
                                        GlowBitmap.
                                        CachedBitmapInfo[16];

                        private readonly IntPtr _hBitmap;
                        private readonly IntPtr _pbits;
                        private readonly NativeMethodsUltimate.BITMAPINFO _bitmapInfo;
                        public IntPtr Handle
                        {
                                get
                                {
                                        return this._hBitmap;
                                }
                        }
                        public IntPtr DIBits
                        {
                                get
                                {
                                        return this._pbits;
                                }
                        }
                        public int Width
                        {
                                get
                                {
                                        return this._bitmapInfo.biWidth;
                                }
                        }
                        public int Height
                        {
                                get
                                {
                                        return -this._bitmapInfo.biHeight;
                                }
                        }
                        public GlowBitmap(IntPtr hdcScreen, int width, int height)
                        {
                                this._bitmapInfo.biSize = Marshal.SizeOf(typeof(NativeMethodsUltimate.BITMAPINFOHEADER));
                                this._bitmapInfo.biPlanes = 1;
                                this._bitmapInfo.biBitCount = 32;
                                this._bitmapInfo.biCompression = 0;
                                this._bitmapInfo.biXPelsPerMeter = 0;
                                this._bitmapInfo.biYPelsPerMeter = 0;
                                this._bitmapInfo.biWidth = width;
                                this._bitmapInfo.biHeight = -height;

                                this._hBitmap = NativeMethodsUltimate.CreateDIBSection(
                                        hdcScreen,
                                        ref this._bitmapInfo,
                                        0u,
                                        out this._pbits,
                                        IntPtr.Zero,
                                        0u);
                        }
                        protected override void DisposeNativeResources()
                        {
                                NativeMethodsUltimate.DeleteObject(this._hBitmap);
                        }
                        private static byte PremultiplyAlpha(byte channel, byte alpha)
                        {
                                return (byte)((double)(channel * alpha) / 255.0);
                        }
                        public static CustomChromeWindow.GlowBitmap Create(
                                CustomChromeWindow.GlowDrawingContext drawingContext,
                                CustomChromeWindow.GlowBitmapPart bitmapPart,
                                System.Windows.Media.Color color)
                        {
                                CustomChromeWindow.
                                        GlowBitmap.
                                        CachedBitmapInfo orCreateAlphaMask =
                                        CustomChromeWindow.
                                        GlowBitmap.GetOrCreateAlphaMask(bitmapPart);

                                CustomChromeWindow.
                                        GlowBitmap glowBitmap =
                                        new CustomChromeWindow.
                                                GlowBitmap(
                                                drawingContext.ScreenDC,
                                                orCreateAlphaMask.Width,
                                                orCreateAlphaMask.Height);

                                for (int i = 0; i < orCreateAlphaMask.DIBits.Length; i += 4)
                                {
                                        byte b = orCreateAlphaMask.DIBits[i + 3];
                                        byte val = CustomChromeWindow.GlowBitmap.PremultiplyAlpha(color.R, b);
                                        byte val2 = CustomChromeWindow.GlowBitmap.PremultiplyAlpha(color.G, b);
                                        byte val3 = CustomChromeWindow.GlowBitmap.PremultiplyAlpha(color.B, b);
                                        Marshal.WriteByte(glowBitmap.DIBits, i, val3);
                                        Marshal.WriteByte(glowBitmap.DIBits, i + 1, val2);
                                        Marshal.WriteByte(glowBitmap.DIBits, i + 2, val);
                                        Marshal.WriteByte(glowBitmap.DIBits, i + 3, b);
                                }
                                return glowBitmap;
                        }
                        private static CustomChromeWindow.GlowBitmap.CachedBitmapInfo GetOrCreateAlphaMask(
                                CustomChromeWindow.GlowBitmapPart bitmapPart)
                        {
                                if (CustomChromeWindow.GlowBitmap._transparencyMasks[(int)bitmapPart] == null)
                                {
                                        BitmapImage bitmapImage = new BitmapImage(
                                                GlowBitmap.MakePackUri(
                                                        typeof(CustomChromeWindow.GlowBitmap).Assembly,
                                                        CustomChromeWindow.GlowBitmap._bitmapRootPath + bitmapPart.ToString() + ".png"));

                                        byte[] array = new byte[4 * bitmapImage.PixelWidth * bitmapImage.PixelHeight];
                                        int stride = 4 * bitmapImage.PixelWidth;
                                        bitmapImage.CopyPixels(array, stride, 0);

                                        CustomChromeWindow.
                                                GlowBitmap._transparencyMasks[(int)bitmapPart] =
                                                new CustomChromeWindow.
                                                        GlowBitmap.
                                                        CachedBitmapInfo(
                                                        array,
                                                        bitmapImage.PixelWidth,
                                                        bitmapImage.PixelHeight);
                                }
                                return CustomChromeWindow.GlowBitmap._transparencyMasks[(int)bitmapPart];
                        }
                        public static Uri MakePackUri(Assembly assembly, string path)
                        {
                                string name = assembly.GetName().Name;
                                return new Uri(string.Format("pack://application:,,,/{0};component/{1}", name, path), UriKind.Absolute);
                        }
                }
                /// <summary>
                /// 阴影位图位置
                /// </summary>
                private enum GlowBitmapPart
                {
                        CornerTopLeft,
                        CornerTopRight,
                        CornerBottomLeft,
                        CornerBottomRight,
                        TopLeft,
                        Top,
                        TopRight,
                        LeftTop,
                        Left,
                        LeftBottom,
                        BottomLeft,
                        Bottom,
                        BottomRight,
                        RightTop,
                        Right,
                        RightBottom
                }
                /// <summary>
                /// 阴影绘图上下文
                /// </summary>
                private sealed class GlowDrawingContext : DisposableObject
                {
                        public NativeMethodsUltimate.BLENDFUNCTION Blend;
                        private readonly IntPtr hdcScreen;
                        private readonly IntPtr hdcWindow;
                        private readonly CustomChromeWindow.GlowBitmap windowBitmap;
                        private readonly IntPtr hdcBackground;
                        public bool IsInitialized
                        {
                                get
                                {
                                        return this.hdcScreen != IntPtr.Zero &&
                                                this.hdcWindow != IntPtr.Zero &&
                                                this.hdcBackground != IntPtr.Zero &&
                                                this.windowBitmap != null;
                                }
                        }
                        public IntPtr ScreenDC
                        {
                                get
                                {
                                        return this.hdcScreen;
                                }
                        }
                        public IntPtr WindowDC
                        {
                                get
                                {
                                        return this.hdcWindow;
                                }
                        }
                        public IntPtr BackgroundDC
                        {
                                get
                                {
                                        return this.hdcBackground;
                                }
                        }
                        public int Width
                        {
                                get
                                {
                                        return this.windowBitmap.Width;
                                }
                        }
                        public int Height
                        {
                                get
                                {
                                        return this.windowBitmap.Height;
                                }
                        }
                        public GlowDrawingContext(int width, int height)
                        {
                                this.hdcScreen = NativeMethodsUltimate.GetDC(IntPtr.Zero);
                                if (this.hdcScreen == IntPtr.Zero)
                                {
                                        return;
                                }
                                this.hdcWindow = NativeMethodsUltimate.CreateCompatibleDC(this.hdcScreen);
                                if (this.hdcWindow == IntPtr.Zero)
                                {
                                        return;
                                }
                                this.hdcBackground = NativeMethodsUltimate.CreateCompatibleDC(this.hdcScreen);
                                if (this.hdcBackground == IntPtr.Zero)
                                {
                                        return;
                                }
                                this.Blend.BlendOp = 0;
                                this.Blend.BlendFlags = 0;
                                this.Blend.SourceConstantAlpha = 255;
                                this.Blend.AlphaFormat = 1;
                                this.windowBitmap = new CustomChromeWindow.GlowBitmap(this.ScreenDC, width, height);
                                NativeMethodsUltimate.SelectObject(this.hdcWindow, this.windowBitmap.Handle);
                        }
                        protected override void DisposeManagedResources()
                        {
                                this.windowBitmap.Dispose();
                        }
                        protected override void DisposeNativeResources()
                        {
                                if (this.hdcScreen != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.ReleaseDC(IntPtr.Zero, this.hdcScreen);
                                }
                                if (this.hdcWindow != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteDC(this.hdcWindow);
                                }
                                if (this.hdcBackground != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteDC(this.hdcBackground);
                                }
                        }
                }
                /// <summary>
                /// 阴影窗口
                /// </summary>
                private sealed class GlowWindow : HwndWrapper
                {
                        [Flags]
                        private enum FieldInvalidationTypes
                        {
                                None = 0,
                                Location = 1,
                                Size = 2,
                                ActiveColor = 4,
                                InactiveColor = 8,
                                Render = 16,
                                Visibility = 32
                        }
                        private const string GlowWindowClassName = "VisualStudioGlowWindow";
                        private const int GlowDepth = 9;
                        private const int CornerGripThickness = 18;
                        private readonly CustomChromeWindow _targetWindow;
                        private readonly Dock _orientation;
                        private readonly CustomChromeWindow.GlowBitmap[] _activeGlowBitmaps = new CustomChromeWindow.GlowBitmap[16];
                        private readonly CustomChromeWindow.GlowBitmap[] _inactiveGlowBitmaps = new CustomChromeWindow.GlowBitmap[16];
                        private static ushort _sharedWindowClassAtom;
                        private static NativeMethodsUltimate.WndProc _sharedWndProc;
                        private static long _createdGlowWindows = 0L;
                        private static long _disposedGlowWindows = 0L;
                        private int _left;
                        private int _top;
                        private int _width;
                        private int _height;
                        private bool _isVisible;
                        private bool _isActive;
                        private System.Windows.Media.Color _activeGlowColor = Colors.Transparent;
                        private System.Windows.Media.Color _inactiveGlowColor = Colors.Transparent;
                        private CustomChromeWindow.GlowWindow.FieldInvalidationTypes _invalidatedValues;
                        private bool _pendingDelayRender;
                        private bool IsDeferringChanges
                        {
                                get
                                {
                                        return this._targetWindow._deferGlowChangesCount > 0;
                                }
                        }
                        private static ushort SharedWindowClassAtom
                        {
                                get
                                {
                                        if (CustomChromeWindow.GlowWindow._sharedWindowClassAtom == 0)
                                        {
                                                WNDCLASS wNDCLASS = default(WNDCLASS);
                                                wNDCLASS.cbClsExtra = 0;
                                                wNDCLASS.cbWndExtra = 0;
                                                wNDCLASS.hbrBackground = IntPtr.Zero;
                                                wNDCLASS.hCursor = IntPtr.Zero;
                                                wNDCLASS.hIcon = IntPtr.Zero;

                                                wNDCLASS.lpfnWndProc = (CustomChromeWindow.GlowWindow._sharedWndProc =
                                                        new NativeMethodsUltimate.WndProc(NativeMethodsUltimate.DefWindowProc));

                                                wNDCLASS.lpszClassName = "VisualStudioGlowWindow";
                                                wNDCLASS.lpszMenuName = null;
                                                wNDCLASS.style = 0u;

                                                CustomChromeWindow.GlowWindow._sharedWindowClassAtom =
                                                        NativeMethodsUltimate.RegisterClass(ref wNDCLASS);
                                        }
                                        return CustomChromeWindow.GlowWindow._sharedWindowClassAtom;
                                }
                        }
                        public bool IsVisible
                        {
                                get
                                {
                                        return this._isVisible;
                                }
                                set
                                {
                                        this.UpdateProperty<bool>(
                                                ref this._isVisible,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render | CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Visibility);
                                }
                        }
                        public int Left
                        {
                                get
                                {
                                        return this._left;
                                }
                                set
                                {
                                        this.UpdateProperty<int>(
                                                ref this._left,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Location);
                                }
                        }
                        public int Top
                        {
                                get
                                {
                                        return this._top;
                                }
                                set
                                {
                                        this.UpdateProperty<int>(
                                                ref this._top,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Location);
                                }
                        }
                        public int Width
                        {
                                get
                                {
                                        return this._width;
                                }
                                set
                                {
                                        this.UpdateProperty<int>(
                                                ref this._width,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Size | CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render);
                                }
                        }
                        public int Height
                        {
                                get
                                {
                                        return this._height;
                                }
                                set
                                {
                                        this.UpdateProperty<int>(
                                                ref this._height,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Size | CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render);
                                }
                        }
                        public bool IsActive
                        {
                                get
                                {
                                        return this._isActive;
                                }
                                set
                                {
                                        this.UpdateProperty<bool>(
                                                ref this._isActive,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render);
                                }
                        }
                        public System.Windows.Media.Color ActiveGlowColor
                        {
                                get
                                {
                                        return this._activeGlowColor;
                                }
                                set
                                {
                                        this.UpdateProperty<System.Windows.Media.Color>(
                                                ref this._activeGlowColor,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.ActiveColor | CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render);
                                }
                        }
                        public System.Windows.Media.Color InactiveGlowColor
                        {
                                get
                                {
                                        return this._inactiveGlowColor;
                                }
                                set
                                {
                                        this.UpdateProperty<System.Windows.Media.Color>(
                                                ref this._inactiveGlowColor,
                                                value,
                                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes.InactiveColor | CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render);
                                }
                        }
                        private IntPtr TargetWindowHandle
                        {
                                get
                                {
                                        return new WindowInteropHelper(this._targetWindow).Handle;
                                }
                        }
                        protected override bool IsWindowSubclassed
                        {
                                get
                                {
                                        return true;
                                }
                        }
                        private bool IsPositionValid
                        {
                                get
                                {
                                        return (this._invalidatedValues &
                                                (CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Location |
                                                        CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Size |
                                                        CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Visibility)) ==
                                                        CustomChromeWindow.GlowWindow.FieldInvalidationTypes.None;
                                }
                        }
                        public GlowWindow(CustomChromeWindow owner, Dock orientation)
                        {
                                if (owner == null)
                                {
                                        throw new ArgumentNullException("owner");
                                }
                                this._targetWindow = owner;
                                this._orientation = orientation;
                                CustomChromeWindow.GlowWindow._createdGlowWindows += 1L;
                        }
                        private void UpdateProperty<T>(ref T field,
                                T value,
                                CustomChromeWindow.GlowWindow.FieldInvalidationTypes invalidatedValues) where T : struct
                        {
                                if (!field.Equals(value))
                                {
                                        field = value;
                                        this._invalidatedValues |= invalidatedValues;
                                        if (!this.IsDeferringChanges)
                                        {
                                                this.CommitChanges();
                                        }
                                }
                        }
                        protected override ushort CreateWindowClassCore()
                        {
                                return CustomChromeWindow.GlowWindow.SharedWindowClassAtom;
                        }
                        protected override void DestroyWindowClassCore()
                        {
                        }
                        protected override IntPtr CreateWindowCore()
                        {
                                return NativeMethodsUltimate.CreateWindowEx(
                                        524416,
                                        new IntPtr((int)base.WindowClassAtom),
                                        string.Empty,
                                        -2046820352,
                                        0,
                                        0,
                                        0,
                                        0,
                                        new WindowInteropHelper(this._targetWindow).Owner,
                                        IntPtr.Zero,
                                        IntPtr.Zero,
                                        IntPtr.Zero);
                        }
                        public void ChangeOwner(IntPtr newOwner)
                        {
                                NativeMethodsUltimate.SetWindowLongPtr(
                                        base.Handle,
                                        NativeMethodsUltimate.GWLP.HWNDPARENT,
                                        newOwner);
                        }
                        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
                        {
                                if (msg <= 70)
                                {
                                        if (msg == 6)
                                        {
                                                return IntPtr.Zero;
                                        }
                                        if (msg == 70)
                                        {
                                                WINDOWPOS wINDOWPOS = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                                                wINDOWPOS.flags |= 16u;
                                                Marshal.StructureToPtr(wINDOWPOS, lParam, true);
                                        }
                                }
                                else
                                {
                                        if (msg != 126)
                                        {
                                                if (msg == 132)
                                                {
                                                        return new IntPtr(this.WmNcHitTest(lParam));
                                                }
                                                switch (msg)
                                                {
                                                        case 161:
                                                        case 163:
                                                        case 164:
                                                        case 166:
                                                        case 167:
                                                        case 169:
                                                        case 171:
                                                        case 173:
                                                                {
                                                                        IntPtr targetWindowHandle = this.TargetWindowHandle;
                                                                        NativeMethodsUltimate.SendMessage(targetWindowHandle, 6, new IntPtr(2), IntPtr.Zero);
                                                                        NativeMethodsUltimate.SendMessage(targetWindowHandle, msg, wParam, IntPtr.Zero);
                                                                        return IntPtr.Zero;
                                                                }
                                                }
                                        }
                                        else
                                        {
                                                if (this.IsVisible)
                                                {
                                                        this.RenderLayeredWindow();
                                                }
                                        }
                                }
                                return base.WndProc(hwnd, msg, wParam, lParam);
                        }
                        private int WmNcHitTest(IntPtr lParam)
                        {
                                int xLParam = NativeMethodsUltimate.GetXLParam(lParam.ToInt32());
                                int yLParam = NativeMethodsUltimate.GetYLParam(lParam.ToInt32());
                                RECT rECT;
                                NativeMethodsUltimate.GetWindowRect(base.Handle, out rECT);
                                switch (this._orientation)
                                {
                                        case Dock.Left:
                                                if (yLParam - 18 < rECT.Top)
                                                {
                                                        return 13;
                                                }
                                                if (yLParam + 18 > rECT.Bottom)
                                                {
                                                        return 16;
                                                }
                                                return 10;
                                        case Dock.Top:
                                                if (xLParam - 18 < rECT.Left)
                                                {
                                                        return 13;
                                                }
                                                if (xLParam + 18 > rECT.Right)
                                                {
                                                        return 14;
                                                }
                                                return 12;
                                        case Dock.Right:
                                                if (yLParam - 18 < rECT.Top)
                                                {
                                                        return 14;
                                                }
                                                if (yLParam + 18 > rECT.Bottom)
                                                {
                                                        return 17;
                                                }
                                                return 11;
                                        default:
                                                if (xLParam - 18 < rECT.Left)
                                                {
                                                        return 16;
                                                }
                                                if (xLParam + 18 > rECT.Right)
                                                {
                                                        return 17;
                                                }
                                                return 15;
                                }
                        }
                        public void CommitChanges()
                        {
                                this.InvalidateCachedBitmaps();
                                this.UpdateWindowPosCore();
                                this.UpdateLayeredWindowCore();
                                this._invalidatedValues = CustomChromeWindow.GlowWindow.FieldInvalidationTypes.None;
                        }
                        private void InvalidateCachedBitmaps()
                        {
                                if (this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.ActiveColor))
                                {
                                        this.ClearCache(this._activeGlowBitmaps);
                                }
                                if (this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.InactiveColor))
                                {
                                        this.ClearCache(this._inactiveGlowBitmaps);
                                }
                        }
                        private void UpdateWindowPosCore()
                        {
                                if (this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Location) ||
                                        this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Size) ||
                                        this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Visibility))
                                {
                                        int num = 532;
                                        if (this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Visibility))
                                        {
                                                if (this.IsVisible)
                                                {
                                                        num |= 64;
                                                }
                                                else
                                                {
                                                        num |= 131;
                                                }
                                        }
                                        if (!this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Location))
                                        {
                                                num |= 2;
                                        }
                                        if (!this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Size))
                                        {
                                                num |= 1;
                                        }
                                        NativeMethodsUltimate.SetWindowPos(base.Handle, IntPtr.Zero, this.Left, this.Top, this.Width, this.Height, num);
                                }
                        }
                        private void UpdateLayeredWindowCore()
                        {
                                if (this.IsVisible && this._invalidatedValues.HasFlag(CustomChromeWindow.GlowWindow.FieldInvalidationTypes.Render))
                                {
                                        if (this.IsPositionValid)
                                        {
                                                this.BeginDelayedRender();
                                                return;
                                        }
                                        this.CancelDelayedRender();
                                        this.RenderLayeredWindow();
                                }
                        }
                        private void BeginDelayedRender()
                        {
                                if (!this._pendingDelayRender)
                                {
                                        this._pendingDelayRender = true;
                                        CompositionTarget.Rendering += new EventHandler(this.CommitDelayedRender);
                                }
                        }
                        private void CancelDelayedRender()
                        {
                                if (this._pendingDelayRender)
                                {
                                        this._pendingDelayRender = false;
                                        CompositionTarget.Rendering -= new EventHandler(this.CommitDelayedRender);
                                }
                        }
                        private void CommitDelayedRender(object sender, EventArgs e)
                        {
                                this.CancelDelayedRender();
                                if (this.IsVisible)
                                {
                                        this.RenderLayeredWindow();
                                }
                        }
                        private void RenderLayeredWindow()
                        {
                                using (CustomChromeWindow.GlowDrawingContext glowDrawingContext =
                                        new CustomChromeWindow.GlowDrawingContext(this.Width, this.Height))
                                {
                                        if (glowDrawingContext.IsInitialized)
                                        {
                                                switch (this._orientation)
                                                {
                                                        case Dock.Left:
                                                                this.DrawLeft(glowDrawingContext);
                                                                break;
                                                        case Dock.Top:
                                                                this.DrawTop(glowDrawingContext);
                                                                break;
                                                        case Dock.Right:
                                                                this.DrawRight(glowDrawingContext);
                                                                break;
                                                        default:
                                                                this.DrawBottom(glowDrawingContext);
                                                                break;
                                                }
                                                POINT pOINT = new POINT
                                                {
                                                        X = this.Left,
                                                        Y = this.Top
                                                };
                                                SIZE sIZE = new SIZE
                                                {
                                                        cx = this.Width,
                                                        cy = this.Height
                                                };
                                                POINT pOINT2 = new POINT
                                                {
                                                        X = 0,
                                                        Y = 0
                                                };

                                                NativeMethodsUltimate.UpdateLayeredWindow(
                                                        base.Handle,
                                                        glowDrawingContext.ScreenDC,
                                                        ref pOINT,
                                                        ref sIZE,
                                                        glowDrawingContext.WindowDC,
                                                        ref pOINT2,
                                                        0u,
                                                        ref glowDrawingContext.Blend,
                                                        2u);
                                        }
                                }
                        }
                        private CustomChromeWindow.GlowBitmap GetOrCreateBitmap(CustomChromeWindow.GlowDrawingContext drawingContext, CustomChromeWindow.GlowBitmapPart bitmapPart)
                        {
                                CustomChromeWindow.GlowBitmap[] array;
                                System.Windows.Media.Color color;
                                if (this.IsActive)
                                {
                                        array = this._activeGlowBitmaps;
                                        color = this.ActiveGlowColor;
                                }
                                else
                                {
                                        array = this._inactiveGlowBitmaps;
                                        color = this.InactiveGlowColor;
                                }
                                if (array[(int)bitmapPart] == null)
                                {
                                        array[(int)bitmapPart] = CustomChromeWindow.GlowBitmap.Create(drawingContext, bitmapPart, color);
                                }
                                return array[(int)bitmapPart];
                        }
                        private void ClearCache(CustomChromeWindow.GlowBitmap[] cache)
                        {
                                for (int i = 0; i < cache.Length; i++)
                                {
                                        using (cache[i])
                                        {
                                                cache[i] = null;
                                        }
                                }
                        }
                        protected override void DisposeManagedResources()
                        {
                                this.ClearCache(this._activeGlowBitmaps);
                                this.ClearCache(this._inactiveGlowBitmaps);
                        }
                        protected override void DisposeNativeResources()
                        {
                                base.DisposeNativeResources();
                                CustomChromeWindow.GlowWindow._disposedGlowWindows += 1L;
                        }
                        private void DrawLeft(CustomChromeWindow.GlowDrawingContext drawingContext)
                        {
                                CustomChromeWindow.GlowBitmap orCreateBitmap = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.CornerTopLeft);
                                CustomChromeWindow.GlowBitmap orCreateBitmap2 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.LeftTop);
                                CustomChromeWindow.GlowBitmap orCreateBitmap3 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.Left);
                                CustomChromeWindow.GlowBitmap orCreateBitmap4 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.LeftBottom);
                                CustomChromeWindow.GlowBitmap orCreateBitmap5 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.CornerBottomLeft);
                                int height = orCreateBitmap.Height;
                                int num = height + orCreateBitmap2.Height;
                                int num2 = drawingContext.Height - orCreateBitmap5.Height;
                                int num3 = num2 - orCreateBitmap4.Height;
                                int num4 = num3 - num;
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.Blend);
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap2.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, height, orCreateBitmap2.Width, orCreateBitmap2.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap2.Width, orCreateBitmap2.Height, drawingContext.Blend);
                                if (num4 > 0)
                                {
                                        NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap3.Handle);
                                        NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, num, orCreateBitmap3.Width, num4, drawingContext.BackgroundDC, 0, 0, orCreateBitmap3.Width, orCreateBitmap3.Height, drawingContext.Blend);
                                }
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap4.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, num3, orCreateBitmap4.Width, orCreateBitmap4.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap4.Width, orCreateBitmap4.Height, drawingContext.Blend);
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap5.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, num2, orCreateBitmap5.Width, orCreateBitmap5.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap5.Width, orCreateBitmap5.Height, drawingContext.Blend);
                        }
                        private void DrawRight(CustomChromeWindow.GlowDrawingContext drawingContext)
                        {
                                CustomChromeWindow.GlowBitmap orCreateBitmap = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.CornerTopRight);
                                CustomChromeWindow.GlowBitmap orCreateBitmap2 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.RightTop);
                                CustomChromeWindow.GlowBitmap orCreateBitmap3 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.Right);
                                CustomChromeWindow.GlowBitmap orCreateBitmap4 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.RightBottom);
                                CustomChromeWindow.GlowBitmap orCreateBitmap5 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.CornerBottomRight);
                                int height = orCreateBitmap.Height;
                                int num = height + orCreateBitmap2.Height;
                                int num2 = drawingContext.Height - orCreateBitmap5.Height;
                                int num3 = num2 - orCreateBitmap4.Height;
                                int num4 = num3 - num;
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.Blend);
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap2.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, height, orCreateBitmap2.Width, orCreateBitmap2.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap2.Width, orCreateBitmap2.Height, drawingContext.Blend);
                                if (num4 > 0)
                                {
                                        NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap3.Handle);
                                        NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, num, orCreateBitmap3.Width, num4, drawingContext.BackgroundDC, 0, 0, orCreateBitmap3.Width, orCreateBitmap3.Height, drawingContext.Blend);
                                }
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap4.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, num3, orCreateBitmap4.Width, orCreateBitmap4.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap4.Width, orCreateBitmap4.Height, drawingContext.Blend);
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap5.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, 0, num2, orCreateBitmap5.Width, orCreateBitmap5.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap5.Width, orCreateBitmap5.Height, drawingContext.Blend);
                        }
                        private void DrawTop(CustomChromeWindow.GlowDrawingContext drawingContext)
                        {
                                CustomChromeWindow.GlowBitmap orCreateBitmap = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.TopLeft);
                                CustomChromeWindow.GlowBitmap orCreateBitmap2 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.Top);
                                CustomChromeWindow.GlowBitmap orCreateBitmap3 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.TopRight);
                                int num = 9;
                                int num2 = num + orCreateBitmap.Width;
                                int num3 = drawingContext.Width - 9 - orCreateBitmap3.Width;
                                int num4 = num3 - num2;
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, num, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.Blend);
                                if (num4 > 0)
                                {
                                        NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap2.Handle);
                                        NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, num2, 0, num4, orCreateBitmap2.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap2.Width, orCreateBitmap2.Height, drawingContext.Blend);
                                }
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap3.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, num3, 0, orCreateBitmap3.Width, orCreateBitmap3.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap3.Width, orCreateBitmap3.Height, drawingContext.Blend);
                        }
                        private void DrawBottom(CustomChromeWindow.GlowDrawingContext drawingContext)
                        {
                                CustomChromeWindow.GlowBitmap orCreateBitmap = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.BottomLeft);
                                CustomChromeWindow.GlowBitmap orCreateBitmap2 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.Bottom);
                                CustomChromeWindow.GlowBitmap orCreateBitmap3 = this.GetOrCreateBitmap(drawingContext, CustomChromeWindow.GlowBitmapPart.BottomRight);
                                int num = 9;
                                int num2 = num + orCreateBitmap.Width;
                                int num3 = drawingContext.Width - 9 - orCreateBitmap3.Width;
                                int num4 = num3 - num2;
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, num, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap.Width, orCreateBitmap.Height, drawingContext.Blend);
                                if (num4 > 0)
                                {
                                        NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap2.Handle);
                                        NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, num2, 0, num4, orCreateBitmap2.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap2.Width, orCreateBitmap2.Height, drawingContext.Blend);
                                }
                                NativeMethodsUltimate.SelectObject(drawingContext.BackgroundDC, orCreateBitmap3.Handle);
                                NativeMethodsUltimate.AlphaBlend(drawingContext.WindowDC, num3, 0, orCreateBitmap3.Width, orCreateBitmap3.Height, drawingContext.BackgroundDC, 0, 0, orCreateBitmap3.Width, orCreateBitmap3.Height, drawingContext.Blend);
                        }
                        public void UpdateWindowPos()
                        {
                                IntPtr targetWindowHandle = this.TargetWindowHandle;
                                RECT rECT;
                                NativeMethodsUltimate.GetWindowRect(targetWindowHandle, out rECT);
                                NativeMethodsUltimate.GetWindowPlacement(targetWindowHandle);
                                if (this.IsVisible)
                                {
                                        int range = 9;
                                        switch (this._orientation)
                                        {
                                                case Dock.Left:
                                                        this.Left = rECT.Left - range;
                                                        this.Top = rECT.Top - range;
                                                        this.Width = range;
                                                        this.Height = rECT.Height + range * 2;
                                                        return;
                                                case Dock.Top:
                                                        this.Left = rECT.Left - range;
                                                        this.Top = rECT.Top - range;
                                                        this.Width = rECT.Width + range * 2;
                                                        this.Height = range;
                                                        return;
                                                case Dock.Right:
                                                        this.Left = rECT.Right;
                                                        this.Top = rECT.Top - range;
                                                        this.Width = range;
                                                        this.Height = rECT.Height + range * 2;
                                                        return;
                                                default:
                                                        this.Left = rECT.Left - range;
                                                        this.Top = rECT.Bottom;
                                                        this.Width = rECT.Width + range * 2;
                                                        this.Height = range;
                                                        break;
                                        }
                                }
                        }
                }

                #region 用于DLL之间数据共享
                public int DefaultResolutionX
                {
                        get;
                        set;
                } = 1024;

                public int DefaultResolutionY
                {
                        get;
                        set;
                } = 768;

                public int TablesViewRows
                {
                        get;
                        set;
                } = 5;

                public int TablesViewColumns
                {
                        get;
                        set;
                } = 5;

                public double TablesViewHeightReduce
                {
                        get;
                        set;
                } = 0.0;

                public double TablesViewWidthReduce
                {
                        get;
                        set;
                } = 0.0;
                #endregion

                private const int MinimizeAnimationDurationMilliseconds = 200;
                public static readonly DependencyProperty CornerRadiusProperty;
                private int lastWindowPlacement;
                private int _deferGlowChangesCount;
                private bool _isGlowVisible;
                private DispatcherTimer _makeGlowVisibleTimer;
                private bool _isNonClientStripVisible;
                private readonly CustomChromeWindow.GlowWindow[] _glowWindows = new CustomChromeWindow.GlowWindow[4];
                private IntPtr ownerForActivate;
                public static readonly DependencyProperty ActiveGlowColorProperty;
                public static readonly DependencyProperty InactiveGlowColorProperty;
                public static readonly DependencyProperty NonClientFillColorProperty;
                private Rect logicalSizeForRestore = Rect.Empty;
                private bool useLogicalSizeForRestore;
                private bool updatingZOrder;
                public int CornerRadius
                {
                        get
                        {
                                return (int)base.GetValue(CustomChromeWindow.CornerRadiusProperty);
                        }
                        set
                        {
                                base.SetValue(CustomChromeWindow.CornerRadiusProperty, value);
                        }
                }
                public System.Windows.Media.Color ActiveGlowColor
                {
                        get
                        {
                                return (System.Windows.Media.Color)base.GetValue(CustomChromeWindow.ActiveGlowColorProperty);
                        }
                        set
                        {
                                base.SetValue(CustomChromeWindow.ActiveGlowColorProperty, value);
                        }
                }
                public System.Windows.Media.Color InactiveGlowColor
                {
                        get
                        {
                                return (System.Windows.Media.Color)base.GetValue(CustomChromeWindow.InactiveGlowColorProperty);
                        }
                        set
                        {
                                base.SetValue(CustomChromeWindow.InactiveGlowColorProperty, value);
                        }
                }
                public System.Windows.Media.Color NonClientFillColor
                {
                        get
                        {
                                return (System.Windows.Media.Color)base.GetValue(CustomChromeWindow.NonClientFillColorProperty);
                        }
                        set
                        {
                                base.SetValue(CustomChromeWindow.NonClientFillColorProperty, value);
                        }
                }
                private static int PressedMouseButtons
                {
                        get
                        {
                                int num = 0;
                                if (NativeMethodsUltimate.IsKeyPressed(1))
                                {
                                        num |= 1;
                                }
                                if (NativeMethodsUltimate.IsKeyPressed(2))
                                {
                                        num |= 2;
                                }
                                if (NativeMethodsUltimate.IsKeyPressed(4))
                                {
                                        num |= 16;
                                }
                                if (NativeMethodsUltimate.IsKeyPressed(5))
                                {
                                        num |= 32;
                                }
                                if (NativeMethodsUltimate.IsKeyPressed(6))
                                {
                                        num |= 64;
                                }
                                return num;
                        }
                }
                private bool IsGlowVisible
                {
                        get
                        {
                                return this._isGlowVisible;
                        }
                        set
                        {
                                if (this._isGlowVisible != value)
                                {
                                        this._isGlowVisible = value;
                                        for (int i = 0; i < this._glowWindows.Length; i++)
                                        {
                                                this.GetOrCreateGlowWindow(i).IsVisible = value;
                                        }
                                }
                        }
                }
                private IEnumerable<CustomChromeWindow.GlowWindow> LoadedGlowWindows
                {
                        get
                        {
                                return
                                        from w in this._glowWindows
                                        where w != null
                                        select w;
                        }
                }
                protected virtual bool ShouldShowGlow
                {
                        get
                        {
                                IntPtr handle = new WindowInteropHelper(this).Handle;
                                return NativeMethodsUltimate.IsWindowVisible(handle) && !NativeMethodsUltimate.IsIconic(handle) && !NativeMethodsUltimate.IsZoomed(handle) && base.ResizeMode != ResizeMode.NoResize;
                        }
                }
                static CustomChromeWindow()
                {
                        CustomChromeWindow.CornerRadiusProperty = DependencyProperty.Register(
                                "CornerRadius",
                                typeof(int),
                                typeof(CustomChromeWindow),
                                new FrameworkPropertyMetadata(
                                        0,
                                        new PropertyChangedCallback(
                                                CustomChromeWindow.OnCornerRadiusChanged)));

                        CustomChromeWindow.ActiveGlowColorProperty = DependencyProperty.Register(
                                "ActiveGlowColor",
                                typeof(System.Windows.Media.Color),
                                typeof(CustomChromeWindow),
                                new FrameworkPropertyMetadata(
                                        Colors.Transparent,
                                        new PropertyChangedCallback(
                                                CustomChromeWindow.OnGlowColorChanged)));

                        CustomChromeWindow.InactiveGlowColorProperty = DependencyProperty.Register(
                                "InactiveGlowColor",
                                typeof(System.Windows.Media.Color),
                                typeof(CustomChromeWindow),
                                new FrameworkPropertyMetadata(
                                        Colors.Transparent,
                                        new PropertyChangedCallback(
                                                CustomChromeWindow.OnGlowColorChanged)));

                        CustomChromeWindow.NonClientFillColorProperty = DependencyProperty.Register(
                                "NonClientFillColor",
                                typeof(System.Windows.Media.Color),
                                typeof(CustomChromeWindow),
                                new FrameworkPropertyMetadata(Colors.Black));

                        Window.ResizeModeProperty.OverrideMetadata(
                                typeof(CustomChromeWindow),
                                new FrameworkPropertyMetadata(
                                        new PropertyChangedCallback(
                                                CustomChromeWindow.OnResizeModeChanged)));
                }
                protected override void OnActivated(EventArgs e)
                {
                        this.UpdateGlowActiveState();
                        base.OnActivated(e);
                }
                protected override void OnDeactivated(EventArgs e)
                {
                        this.UpdateGlowActiveState();
                        base.OnDeactivated(e);
                }
                private static void OnResizeModeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
                {
                        CustomChromeWindow customChromeWindow = (CustomChromeWindow)obj;
                        customChromeWindow.UpdateGlowVisibility(false);
                }
                private static void OnCornerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
                {
                        ((CustomChromeWindow)obj).UpdateClipRegion(CustomChromeWindow.ClipRegionChangeType.FromPropertyChange);
                }
                private static void OnGlowColorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
                {
                        ((CustomChromeWindow)obj).UpdateGlowColors();
                }
                protected override void OnSourceInitialized(EventArgs e)
                {
                        HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
                        hwndSource.AddHook(new HwndSourceHook(this.HwndSourceHook));
                        this.CreateGlowWindowHandles();
                        base.OnSourceInitialized(e);
                }
                private void CreateGlowWindowHandles()
                {
                        for (int i = 0; i < this._glowWindows.Length; i++)
                        {
                                this.GetOrCreateGlowWindow(i).EnsureHandle();
                        }
                }
                protected virtual IntPtr HwndSourceHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                        if (msg <= 71)
                        {
                                if (msg == 6)
                                {
                                        this.WmActivate(wParam, lParam);
                                        goto IL_11C;
                                }
                                if (msg != 12)
                                {
                                        switch (msg)
                                        {
                                                case 70:
                                                        this.WmWindowPosChanging(hWnd, lParam);
                                                        goto IL_11C;
                                                case 71:
                                                        this.WmWindowPosChanged(hWnd, lParam);
                                                        goto IL_11C;
                                                default:
                                                        goto IL_11C;
                                        }
                                }
                        }
                        else
                        {
                                if (msg <= 166)
                                {
                                        switch (msg)
                                        {
                                                case 128:
                                                        break;
                                                case 129:
                                                case 130:
                                                        goto IL_11C;
                                                case 131:
                                                        return this.WmNcCalcSize(hWnd, wParam, lParam, ref handled);
                                                case 132:
                                                        return this.WmNcHitTest(hWnd, lParam, ref handled);
                                                case 133:
                                                        return this.WmNcPaint(hWnd, wParam, lParam, ref handled);
                                                case 134:
                                                        return this.WmNcActivate(hWnd, wParam, lParam, ref handled);
                                                default:
                                                        switch (msg)
                                                        {
                                                                case 164:
                                                                case 165:
                                                                case 166:
                                                                        CustomChromeWindow.RaiseNonClientMouseMessageAsClient(hWnd, msg, wParam, lParam);
                                                                        handled = true;
                                                                        goto IL_11C;
                                                                default:
                                                                        goto IL_11C;
                                                        }
                                        }
                                }
                                else
                                {
                                        switch (msg)
                                        {
                                                case 174:
                                                case 175:
                                                        handled = true;
                                                        goto IL_11C;
                                                default:
                                                        if (msg != 274)
                                                        {
                                                                goto IL_11C;
                                                        }
                                                        this.WmSysCommand(hWnd, wParam, lParam);
                                                        goto IL_11C;
                                        }
                                }
                        }
                        return this.CallDefWindowProcWithoutVisibleStyle(hWnd, msg, wParam, lParam, ref handled);
                IL_11C:
                        return IntPtr.Zero;
                }
                private static void RaiseNonClientMouseMessageAsClient(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
                {
                        POINT pOINT = new POINT
                        {
                                X = NativeMethodsUltimate.GetXLParam(lParam.ToInt32()),
                                Y = NativeMethodsUltimate.GetYLParam(lParam.ToInt32())
                        };
                        NativeMethodsUltimate.ScreenToClient(hWnd, ref pOINT);
                        NativeMethodsUltimate.SendMessage(hWnd, msg + 513 - 161, new IntPtr(CustomChromeWindow.PressedMouseButtons), NativeMethodsUltimate.MakeParam(pOINT.X, pOINT.Y));
                }
                private IntPtr CallDefWindowProcWithoutVisibleStyle(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                        bool flag = UtilityMethods.ModifyStyle(hWnd, 268435456, 0);
                        IntPtr result = NativeMethodsUltimate.DefWindowProc(hWnd, msg, wParam, lParam);
                        if (flag)
                        {
                                UtilityMethods.ModifyStyle(hWnd, 0, 268435456);
                        }
                        handled = true;
                        return result;
                }
                private void WmActivate(IntPtr wParam, IntPtr lParam)
                {
                        if (this.ownerForActivate != IntPtr.Zero)
                        {
                                NativeMethodsUltimate.SendMessage(this.ownerForActivate, NativeMethodsUltimate.NOTIFYOWNERACTIVATE, wParam, lParam);
                        }
                }
                private IntPtr WmNcActivate(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                        handled = true;
                        return NativeMethodsUltimate.DefWindowProc(hWnd, 134, wParam, NativeMethodsUltimate.HRGN_NONE);
                }
                private IntPtr WmNcPaint(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                        if (this._isNonClientStripVisible)
                        {
                                IntPtr hrgnClip = (wParam == new IntPtr(1)) ? IntPtr.Zero : wParam;
                                IntPtr dCEx = NativeMethodsUltimate.GetDCEx(hWnd, hrgnClip, 155);
                                if (dCEx != IntPtr.Zero)
                                {
                                        try
                                        {
                                                System.Windows.Media.Color nonClientFillColor = this.NonClientFillColor;
                                                int colorref = (int)nonClientFillColor.B << 16 | (int)nonClientFillColor.G << 8 | (int)nonClientFillColor.R;
                                                IntPtr intPtr = NativeMethodsUltimate.CreateSolidBrush(colorref);
                                                try
                                                {
                                                        RECT clientRectRelativeToWindowRect = CustomChromeWindow.GetClientRectRelativeToWindowRect(hWnd);
                                                        clientRectRelativeToWindowRect.Top = clientRectRelativeToWindowRect.Bottom;
                                                        clientRectRelativeToWindowRect.Bottom = clientRectRelativeToWindowRect.Top + 1;
                                                        NativeMethodsUltimate.FillRect(dCEx, ref clientRectRelativeToWindowRect, intPtr);
                                                }
                                                finally
                                                {
                                                        NativeMethodsUltimate.DeleteObject(intPtr);
                                                }
                                        }
                                        finally
                                        {
                                                NativeMethodsUltimate.ReleaseDC(hWnd, dCEx);
                                        }
                                }
                        }
                        handled = true;
                        return IntPtr.Zero;
                }
                private static RECT GetClientRectRelativeToWindowRect(IntPtr hWnd)
                {
                        RECT rECT;
                        NativeMethodsUltimate.GetWindowRect(hWnd, out rECT);
                        RECT result;
                        NativeMethodsUltimate.GetClientRect(hWnd, out result);
                        POINT pOINT = new POINT
                        {
                                X = 0,
                                Y = 0
                        };
                        NativeMethodsUltimate.ClientToScreen(hWnd, ref pOINT);
                        result.Offset(pOINT.X - rECT.Left, pOINT.Y - rECT.Top);
                        return result;
                }
                private IntPtr WmNcCalcSize(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
                {
                        this._isNonClientStripVisible = false;
                        WINDOWPLACEMENT windowPlacement = NativeMethodsUltimate.GetWindowPlacement(hWnd);
                        bool flag = windowPlacement.showCmd == 3;
                        if (flag)
                        {
                                RECT rECT = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                                NativeMethodsUltimate.DefWindowProc(hWnd, 131, wParam, lParam);
                                RECT rECT2 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                                MONITORINFO mONITORINFO = CustomChromeWindow.MonitorInfoFromWindow(hWnd);
                                if (mONITORINFO.rcMonitor.Height == mONITORINFO.rcWork.Height && mONITORINFO.rcMonitor.Width == mONITORINFO.rcWork.Width)
                                {
                                        this._isNonClientStripVisible = true;
                                        rECT2.Bottom--;
                                }
                                rECT2.Top = rECT.Top + (int)this.GetWindowInfo(hWnd).cyWindowBorders;
                                Marshal.StructureToPtr(rECT2, lParam, true);
                        }
                        handled = true;
                        return IntPtr.Zero;
                }
                private IntPtr WmNcHitTest(IntPtr hWnd, IntPtr lParam, ref bool handled)
                {
                        if (!this.IsConnectedToPresentationSource())
                        {
                                return new IntPtr(0);
                        }
                        System.Windows.Point point = new System.Windows.Point((double)NativeMethodsUltimate.GetXLParam(lParam.ToInt32()), (double)NativeMethodsUltimate.GetYLParam(lParam.ToInt32()));
                        System.Windows.Point point2 = base.PointFromScreen(point);
                        DependencyObject visualHit = null;
                        UtilityMethods.HitTestVisibleElements(this, delegate (HitTestResult target)
                        {
                                visualHit = target.VisualHit;
                                return HitTestResultBehavior.Stop;
                        }, new PointHitTestParameters(point2));
                        int num = 0;
                        while (visualHit != null)
                        {
                                INonClientArea nonClientArea = visualHit as INonClientArea;
                                if (nonClientArea != null)
                                {
                                        num = nonClientArea.HitTest(point);
                                        if (num != 0)
                                        {
                                                break;
                                        }
                                }
                                visualHit = visualHit.GetVisualOrLogicalParent();
                        }
                        if (num == 0)
                        {
                                num = 1;
                        }
                        handled = true;
                        return new IntPtr(num);
                }
                private void WmSysCommand(IntPtr hWnd, IntPtr wParam, IntPtr lParam)
                {
                        int num = NativeMethodsUltimate.GET_SC_WPARAM(wParam);
                        if (num == 61456)
                        {
                                NativeMethodsUltimate.RedrawWindow(hWnd, IntPtr.Zero, IntPtr.Zero, NativeMethodsUltimate.RedrawWindowFlags.Invalidate | NativeMethodsUltimate.RedrawWindowFlags.NoChildren | NativeMethodsUltimate.RedrawWindowFlags.UpdateNow | NativeMethodsUltimate.RedrawWindowFlags.Frame);
                        }
                        if ((num == 61488 || num == 61472 || num == 61456 || num == 61440) && base.WindowState == WindowState.Normal && !this.IsAeroSnappedToMonitor(hWnd))
                        {
                                this.logicalSizeForRestore = new Rect(base.Left, base.Top, base.Width, base.Height);
                        }
                        if (num == 61456 && base.WindowState == WindowState.Maximized && this.logicalSizeForRestore == Rect.Empty)
                        {
                                this.logicalSizeForRestore = new Rect(base.Left, base.Top, base.Width, base.Height);
                        }
                        if (num == 61728 && base.WindowState != WindowState.Minimized && this.logicalSizeForRestore.Width > 0.0 && this.logicalSizeForRestore.Height > 0.0)
                        {
                                base.Left = this.logicalSizeForRestore.Left;
                                base.Top = this.logicalSizeForRestore.Top;
                                base.Width = this.logicalSizeForRestore.Width;
                                base.Height = this.logicalSizeForRestore.Height;
                                this.useLogicalSizeForRestore = true;
                        }
                }
                private bool IsAeroSnappedToMonitor(IntPtr hWnd)
                {
                        MONITORINFO mONITORINFO = CustomChromeWindow.MonitorInfoFromWindow(hWnd);
                        Rect logicalRect = new Rect(base.Left, base.Top, base.Width, base.Height);
                        logicalRect = logicalRect.LogicalToDeviceUnits();
                        return (double)mONITORINFO.rcWork.Height == logicalRect.Height && (double)mONITORINFO.rcWork.Top == logicalRect.Top;
                }
                internal Rect GetOnScreenPosition(Rect floatRect)
                {
                        Rect result = floatRect;
                        floatRect = floatRect.LogicalToDeviceUnits();
                        Rect rect;
                        Rect rect2;
                        Screen.FindMaximumSingleMonitorRectangle(floatRect, out rect, out rect2);
                        if (!floatRect.IntersectsWith(rect2))
                        {
                                Screen.FindMonitorRectsFromPoint(NativeMethodsUltimate.GetCursorPos(), out rect, out rect2);
                                rect2 = rect2.DeviceToLogicalUnits();
                                if (result.Width > rect2.Width)
                                {
                                        result.Width = rect2.Width;
                                }
                                if (result.Height > rect2.Height)
                                {
                                        result.Height = rect2.Height;
                                }
                                if (rect2.Right <= result.X)
                                {
                                        result.X = rect2.Right - result.Width;
                                }
                                if (rect2.Left > result.X + result.Width)
                                {
                                        result.X = rect2.Left;
                                }
                                if (rect2.Bottom <= result.Y)
                                {
                                        result.Y = rect2.Bottom - result.Height;
                                }
                                if (rect2.Top > result.Y + result.Height)
                                {
                                        result.Y = rect2.Top;
                                }
                        }
                        return result;
                }
                private void WmWindowPosChanging(IntPtr hwnd, IntPtr lParam)
                {
                        WINDOWPOS wINDOWPOS = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                        if ((wINDOWPOS.flags & 2u) == 0u && (wINDOWPOS.flags & 1u) == 0u && wINDOWPOS.cx > 0 && wINDOWPOS.cy > 0)
                        {
                                Rect rect = new Rect((double)wINDOWPOS.x, (double)wINDOWPOS.y, (double)wINDOWPOS.cx, (double)wINDOWPOS.cy);
                                rect = rect.DeviceToLogicalUnits();
                                if (this.useLogicalSizeForRestore)
                                {
                                        rect = this.logicalSizeForRestore;
                                        this.logicalSizeForRestore = Rect.Empty;
                                        this.useLogicalSizeForRestore = false;
                                }
                                Rect logicalRect = this.GetOnScreenPosition(rect);
                                logicalRect = logicalRect.LogicalToDeviceUnits();
                                wINDOWPOS.x = (int)logicalRect.X;
                                wINDOWPOS.y = (int)logicalRect.Y;
                                Marshal.StructureToPtr(wINDOWPOS, lParam, true);
                        }
                }
                private void WmWindowPosChanged(IntPtr hWnd, IntPtr lParam)
                {
                        try
                        {
                                WINDOWPOS wINDOWPOS = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                                WINDOWPLACEMENT windowPlacement = NativeMethodsUltimate.GetWindowPlacement(hWnd);
                                RECT currentBounds = new RECT(wINDOWPOS.x, wINDOWPOS.y, wINDOWPOS.x + wINDOWPOS.cx, wINDOWPOS.y + wINDOWPOS.cy);
                                if ((wINDOWPOS.flags & 1u) != 1u)
                                {
                                        this.UpdateClipRegion(hWnd, windowPlacement, CustomChromeWindow.ClipRegionChangeType.FromSize, currentBounds);
                                }
                                else
                                {
                                        if ((wINDOWPOS.flags & 2u) != 2u)
                                        {
                                                this.UpdateClipRegion(hWnd, windowPlacement, CustomChromeWindow.ClipRegionChangeType.FromPosition, currentBounds);
                                        }
                                }
                                this.OnWindowPosChanged(hWnd, windowPlacement.showCmd, windowPlacement.rcNormalPosition.ToInt32Rect());
                                this.UpdateGlowWindowPositions((wINDOWPOS.flags & 64u) == 0u);
                                this.UpdateZOrderOfThisAndOwner();
                        }
                        catch (Win32Exception)
                        {
                        }
                }
                private void UpdateZOrderOfThisAndOwner()
                {
                        if (this.updatingZOrder)
                        {
                                return;
                        }
                        try
                        {
                                this.updatingZOrder = true;
                                WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
                                IntPtr handle = windowInteropHelper.Handle;
                                foreach (CustomChromeWindow.GlowWindow current in this.LoadedGlowWindows)
                                {
                                        IntPtr window = NativeMethodsUltimate.GetWindow(current.Handle, 3);
                                        if (window != handle)
                                        {
                                                NativeMethodsUltimate.SetWindowPos(current.Handle, handle, 0, 0, 0, 0, 19);
                                        }
                                        handle = current.Handle;
                                }
                                IntPtr owner = windowInteropHelper.Owner;
                                if (owner != IntPtr.Zero)
                                {
                                        this.UpdateZOrderOfOwner(owner);
                                }
                        }
                        finally
                        {
                                this.updatingZOrder = false;
                        }
                }
                private void UpdateZOrderOfOwner(IntPtr hwndOwner)
                {
                        IntPtr lastOwnedWindow = IntPtr.Zero;
                        NativeMethodsUltimate.EnumThreadWindows(NativeMethodsUltimate.GetCurrentThreadId(), delegate (IntPtr hwnd, IntPtr lParam)
                        {
                                if (NativeMethodsUltimate.GetWindow(hwnd, 4) == hwndOwner)
                                {
                                        lastOwnedWindow = hwnd;
                                }
                                return true;
                        }, IntPtr.Zero);
                        if (lastOwnedWindow != IntPtr.Zero && NativeMethodsUltimate.GetWindow(hwndOwner, 3) != lastOwnedWindow)
                        {
                                NativeMethodsUltimate.SetWindowPos(hwndOwner, lastOwnedWindow, 0, 0, 0, 0, 19);
                        }
                }
                protected virtual void OnWindowPosChanged(IntPtr hWnd, int showCmd, Int32Rect rcNormalPosition)
                {
                }
                /// <summary>
                /// 更新剪裁区域
                /// </summary>
                /// <param name="regionChangeType"></param>
                protected void UpdateClipRegion(CustomChromeWindow.ClipRegionChangeType regionChangeType = CustomChromeWindow.ClipRegionChangeType.FromPropertyChange)
                {
                        HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(this);
                        if (hwndSource != null)
                        {
                                RECT currentBounds;
                                NativeMethodsUltimate.GetWindowRect(hwndSource.Handle, out currentBounds);
                                WINDOWPLACEMENT windowPlacement = NativeMethodsUltimate.GetWindowPlacement(hwndSource.Handle);
                                this.UpdateClipRegion(hwndSource.Handle, windowPlacement, regionChangeType, currentBounds);
                        }
                }
                private void UpdateClipRegion(IntPtr hWnd, WINDOWPLACEMENT placement, CustomChromeWindow.ClipRegionChangeType changeType, RECT currentBounds)
                {
                        this.UpdateClipRegionCore(hWnd, placement.showCmd, changeType, currentBounds.ToInt32Rect());
                        this.lastWindowPlacement = placement.showCmd;
                }
                protected virtual bool UpdateClipRegionCore(IntPtr hWnd, int showCmd, CustomChromeWindow.ClipRegionChangeType changeType, Int32Rect currentBounds)
                {
                        if (showCmd == 3)
                        {
                                this.UpdateMaximizedClipRegion(hWnd);
                                return true;
                        }
                        if (changeType == CustomChromeWindow.ClipRegionChangeType.FromSize || changeType == CustomChromeWindow.ClipRegionChangeType.FromPropertyChange || this.lastWindowPlacement != showCmd)
                        {
                                if (this.CornerRadius < 0)
                                {
                                        this.ClearClipRegion(hWnd);
                                }
                                else
                                {
                                        this.SetRoundRect(hWnd, currentBounds.Width, currentBounds.Height);
                                }
                                return true;
                        }
                        return false;
                }
                private WINDOWINFO GetWindowInfo(IntPtr hWnd)
                {
                        WINDOWINFO wINDOWINFO = default(WINDOWINFO);
                        wINDOWINFO.cbSize = Marshal.SizeOf(wINDOWINFO);
                        NativeMethodsUltimate.GetWindowInfo(hWnd, ref wINDOWINFO);
                        return wINDOWINFO;
                }
                private void UpdateMaximizedClipRegion(IntPtr hWnd)
                {
                        RECT clientRectRelativeToWindowRect = CustomChromeWindow.GetClientRectRelativeToWindowRect(hWnd);
                        if (this._isNonClientStripVisible)
                        {
                                clientRectRelativeToWindowRect.Bottom++;
                        }

                        IntPtr hRgn = NativeMethodsUltimate.CreateRectRgnIndirect(ref clientRectRelativeToWindowRect);
                        NativeMethodsUltimate.SetWindowRgn(hWnd, hRgn, NativeMethodsUltimate.IsWindowVisible(hWnd));
                }
                private static MONITORINFO MonitorInfoFromWindow(IntPtr hWnd)
                {
                        IntPtr hMonitor = NativeMethodsUltimate.MonitorFromWindow(hWnd, 2);
                        MONITORINFO result = default(MONITORINFO);
                        result.cbSize = (uint)Marshal.SizeOf(typeof(MONITORINFO));
                        NativeMethodsUltimate.GetMonitorInfo(hMonitor, ref result);
                        return result;
                }
                private void ClearClipRegion(IntPtr hWnd)
                {
                        NativeMethodsUltimate.SetWindowRgn(hWnd, IntPtr.Zero, NativeMethodsUltimate.IsWindowVisible(hWnd));
                }
                protected void SetRoundRect(IntPtr hWnd, int width, int height)
                {
                        IntPtr hRgn = this.ComputeRoundRectRegion(0, 0, width, height, this.CornerRadius);
                        NativeMethodsUltimate.SetWindowRgn(hWnd, hRgn, NativeMethodsUltimate.IsWindowVisible(hWnd));
                }
                private IntPtr ComputeRoundRectRegion(int left, int top, int width, int height, int cornerRadius)
                {
                        int nWidthEllipse = (int)((double)(2 * cornerRadius) * DpiHelper.LogicalToDeviceUnitsScalingFactorX);
                        int nHeightEllipse = (int)((double)(2 * cornerRadius) * DpiHelper.LogicalToDeviceUnitsScalingFactorY);
                        return NativeMethodsUltimate.CreateRoundRectRgn(left, top, left + width + 1, top + height + 1, nWidthEllipse, nHeightEllipse);
                }
                protected IntPtr ComputeCornerRadiusRectRegion(Int32Rect rect, CornerRadius cornerRadius)
                {
                        if (cornerRadius.TopLeft == cornerRadius.TopRight && cornerRadius.TopLeft == cornerRadius.BottomLeft && cornerRadius.BottomLeft == cornerRadius.BottomRight)
                        {
                                return this.ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int)cornerRadius.TopLeft);
                        }
                        IntPtr intPtr = IntPtr.Zero;
                        IntPtr intPtr2 = IntPtr.Zero;
                        IntPtr intPtr3 = IntPtr.Zero;
                        IntPtr intPtr4 = IntPtr.Zero;
                        IntPtr intPtr5 = IntPtr.Zero;
                        IntPtr intPtr6 = IntPtr.Zero;
                        IntPtr intPtr7 = IntPtr.Zero;
                        IntPtr intPtr8 = IntPtr.Zero;
                        IntPtr intPtr9 = IntPtr.Zero;
                        IntPtr intPtr10 = IntPtr.Zero;
                        try
                        {
                                intPtr = this.ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int)cornerRadius.TopLeft);
                                intPtr2 = this.ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int)cornerRadius.TopRight);
                                intPtr3 = this.ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int)cornerRadius.BottomLeft);
                                intPtr4 = this.ComputeRoundRectRegion(rect.X, rect.Y, rect.Width, rect.Height, (int)cornerRadius.BottomRight);
                                POINT pOINT = new POINT
                                {
                                        X = rect.X + rect.Width / 2,
                                        Y = rect.Y + rect.Height / 2
                                };
                                intPtr5 = NativeMethodsUltimate.CreateRectRgn(rect.X, rect.Y, pOINT.X + 1, pOINT.Y + 1);
                                intPtr6 = NativeMethodsUltimate.CreateRectRgn(pOINT.X - 1, rect.Y, rect.X + rect.Width, pOINT.Y + 1);
                                intPtr7 = NativeMethodsUltimate.CreateRectRgn(rect.X, pOINT.Y - 1, pOINT.X + 1, rect.Y + rect.Height);
                                intPtr8 = NativeMethodsUltimate.CreateRectRgn(pOINT.X - 1, pOINT.Y - 1, rect.X + rect.Width, rect.Y + rect.Height);
                                intPtr9 = NativeMethodsUltimate.CreateRectRgn(0, 0, 1, 1);
                                intPtr10 = NativeMethodsUltimate.CreateRectRgn(0, 0, 1, 1);
                                NativeMethodsUltimate.CombineRgn(intPtr10, intPtr, intPtr5, NativeMethodsUltimate.CombineMode.RGN_AND);
                                NativeMethodsUltimate.CombineRgn(intPtr9, intPtr2, intPtr6, NativeMethodsUltimate.CombineMode.RGN_AND);
                                NativeMethodsUltimate.CombineRgn(intPtr10, intPtr10, intPtr9, NativeMethodsUltimate.CombineMode.RGN_OR);
                                NativeMethodsUltimate.CombineRgn(intPtr9, intPtr3, intPtr7, NativeMethodsUltimate.CombineMode.RGN_AND);
                                NativeMethodsUltimate.CombineRgn(intPtr10, intPtr10, intPtr9, NativeMethodsUltimate.CombineMode.RGN_OR);
                                NativeMethodsUltimate.CombineRgn(intPtr9, intPtr4, intPtr8, NativeMethodsUltimate.CombineMode.RGN_AND);
                                NativeMethodsUltimate.CombineRgn(intPtr10, intPtr10, intPtr9, NativeMethodsUltimate.CombineMode.RGN_OR);
                        }
                        finally
                        {
                                if (intPtr != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr);
                                }
                                if (intPtr2 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr2);
                                }
                                if (intPtr3 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr3);
                                }
                                if (intPtr4 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr4);
                                }
                                if (intPtr5 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr5);
                                }
                                if (intPtr6 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr6);
                                }
                                if (intPtr7 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr7);
                                }
                                if (intPtr8 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr8);
                                }
                                if (intPtr9 != IntPtr.Zero)
                                {
                                        NativeMethodsUltimate.DeleteObject(intPtr9);
                                }
                        }
                        return intPtr10;
                }
                public static void ShowWindowMenu(HwndSource source, Visual element, System.Windows.Point elementPoint, System.Windows.Size elementSize)
                {
                        if (elementPoint.X >= 0.0 && elementPoint.X <= elementSize.Width && elementPoint.Y >= 0.0 && elementPoint.Y <= elementSize.Height)
                        {
                                System.Windows.Point screenPoint = element.PointToScreen(elementPoint);
                                CustomChromeWindow.ShowWindowMenu(source, screenPoint, true);
                        }
                }
                protected static void ShowWindowMenu(HwndSource source, System.Windows.Point screenPoint, bool canMinimize)
                {
                        int systemMetrics = NativeMethodsUltimate.GetSystemMetrics(40);
                        IntPtr systemMenu = NativeMethodsUltimate.GetSystemMenu(source.Handle, false);
                        WINDOWPLACEMENT windowPlacement = NativeMethodsUltimate.GetWindowPlacement(source.Handle);
                        bool flag = UtilityMethods.ModifyStyle(source.Handle, 268435456, 0);
                        uint uEnable = canMinimize ? 0u : 1u;
                        if (windowPlacement.showCmd == 1)
                        {
                                NativeMethodsUltimate.EnableMenuItem(systemMenu, 61728u, 1u);
                                NativeMethodsUltimate.EnableMenuItem(systemMenu, 61456u, 0u);
                                NativeMethodsUltimate.EnableMenuItem(systemMenu, 61440u, 0u);
                                NativeMethodsUltimate.EnableMenuItem(systemMenu, 61488u, 0u);
                                NativeMethodsUltimate.EnableMenuItem(systemMenu, 61472u, uEnable);
                                NativeMethodsUltimate.EnableMenuItem(systemMenu, 61536u, 0u);
                        }
                        else
                        {
                                if (windowPlacement.showCmd == 3)
                                {
                                        NativeMethodsUltimate.EnableMenuItem(systemMenu, 61728u, 0u);
                                        NativeMethodsUltimate.EnableMenuItem(systemMenu, 61456u, 1u);
                                        NativeMethodsUltimate.EnableMenuItem(systemMenu, 61440u, 1u);
                                        NativeMethodsUltimate.EnableMenuItem(systemMenu, 61488u, 1u);
                                        NativeMethodsUltimate.EnableMenuItem(systemMenu, 61472u, uEnable);
                                        NativeMethodsUltimate.EnableMenuItem(systemMenu, 61536u, 0u);
                                }
                        }
                        if (flag)
                        {
                                UtilityMethods.ModifyStyle(source.Handle, 0, 268435456);
                        }
                        uint fuFlags = (uint)(systemMetrics | 256 | 128 | 2);
                        int num = NativeMethodsUltimate.TrackPopupMenuEx(systemMenu, fuFlags, (int)screenPoint.X, (int)screenPoint.Y, source.Handle, IntPtr.Zero);
                        if (num != 0)
                        {
                                NativeMethodsUltimate.PostMessage(source.Handle, 274, new IntPtr(num), IntPtr.Zero);
                        }
                }
                protected override void OnClosed(EventArgs e)
                {
                        this.StopTimer();
                        this.DestroyGlowWindows();
                        base.OnClosed(e);
                }
                private CustomChromeWindow.GlowWindow GetOrCreateGlowWindow(int direction)
                {
                        if (this._glowWindows[direction] == null)
                        {
                                this._glowWindows[direction] = new CustomChromeWindow.GlowWindow(this, (Dock)direction);
                                this._glowWindows[direction].ActiveGlowColor = this.ActiveGlowColor;
                                this._glowWindows[direction].InactiveGlowColor = this.InactiveGlowColor;
                                this._glowWindows[direction].IsActive = base.IsActive;
                        }
                        return this._glowWindows[direction];
                }
                private void DestroyGlowWindows()
                {
                        for (int i = 0; i < this._glowWindows.Length; i++)
                        {
                                using (this._glowWindows[i])
                                {
                                        this._glowWindows[i] = null;
                                }
                        }
                }
                private void UpdateGlowWindowPositions(bool delayIfNecessary)
                {
                        using (this.DeferGlowChanges())
                        {
                                this.UpdateGlowVisibility(delayIfNecessary);
                                foreach (CustomChromeWindow.GlowWindow current in this.LoadedGlowWindows)
                                {
                                        current.UpdateWindowPos();
                                }
                        }
                }
                private void UpdateGlowActiveState()
                {
                        using (this.DeferGlowChanges())
                        {
                                foreach (CustomChromeWindow.GlowWindow current in this.LoadedGlowWindows)
                                {
                                        current.IsActive = base.IsActive;
                                }
                        }
                }
                public void ChangeOwnerForActivate(IntPtr newOwner)
                {
                        this.ownerForActivate = newOwner;
                }
                public void ChangeOwner(IntPtr newOwner)
                {
                        WindowInteropHelper windowInteropHelper = new WindowInteropHelper(this);
                        windowInteropHelper.Owner = newOwner;
                        foreach (CustomChromeWindow.GlowWindow current in this.LoadedGlowWindows)
                        {
                                current.ChangeOwner(newOwner);
                        }
                        this.UpdateZOrderOfThisAndOwner();
                }
                private void UpdateGlowVisibility(bool delayIfNecessary)
                {
                        bool shouldShowGlow = this.ShouldShowGlow;
                        if (shouldShowGlow != this.IsGlowVisible)
                        {
                                if (SystemParameters.MinimizeAnimation && shouldShowGlow && delayIfNecessary)
                                {
                                        if (this._makeGlowVisibleTimer != null)
                                        {
                                                this._makeGlowVisibleTimer.Stop();
                                        }
                                        else
                                        {
                                                this._makeGlowVisibleTimer = new DispatcherTimer();
                                                this._makeGlowVisibleTimer.Interval = TimeSpan.FromMilliseconds(200.0);
                                                this._makeGlowVisibleTimer.Tick += new EventHandler(this.OnDelayedVisibilityTimerTick);
                                        }
                                        this._makeGlowVisibleTimer.Start();
                                        return;
                                }
                                this.StopTimer();
                                this.IsGlowVisible = shouldShowGlow;
                        }
                }
                private void StopTimer()
                {
                        if (this._makeGlowVisibleTimer != null)
                        {
                                this._makeGlowVisibleTimer.Stop();
                                this._makeGlowVisibleTimer.Tick -= new EventHandler(this.OnDelayedVisibilityTimerTick);
                                this._makeGlowVisibleTimer = null;
                        }
                }
                private void OnDelayedVisibilityTimerTick(object sender, EventArgs e)
                {
                        this.StopTimer();
                        this.UpdateGlowWindowPositions(false);
                }
                private void UpdateGlowColors()
                {
                        using (this.DeferGlowChanges())
                        {
                                foreach (CustomChromeWindow.GlowWindow current in this.LoadedGlowWindows)
                                {
                                        current.ActiveGlowColor = this.ActiveGlowColor;
                                        current.InactiveGlowColor = this.InactiveGlowColor;
                                }
                        }
                }
                private IDisposable DeferGlowChanges()
                {
                        return new CustomChromeWindow.ChangeScope(this);
                }
                private void EndDeferGlowChanges()
                {
                        foreach (CustomChromeWindow.GlowWindow current in this.LoadedGlowWindows)
                        {
                                current.CommitChanges();
                        }
                }
        }
}
