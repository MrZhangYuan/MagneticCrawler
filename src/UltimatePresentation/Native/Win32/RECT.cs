using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UltimatePresentation.Native.Win32
{
        [Serializable]
	[StructLayout(LayoutKind.Sequential, Pack = 0)]
	public struct RECT
        {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
                public System.Windows.Point Position
                {
                        get
                        {
                                return new System.Windows.Point((double)this.Left, (double)this.Top);
                        }
                }
                public System.Windows.Size Size
                {
                        get
                        {
                                return new System.Windows.Size((double)this.Width, (double)this.Height);
                        }
                }
                public int Height
                {
                        get
                        {
                                return this.Bottom - this.Top;
                        }
                        set
                        {
                                this.Bottom = this.Top + value;
                        }
                }
                public int Width
                {
                        get
                        {
                                return this.Right - this.Left;
                        }
                        set
                        {
                                this.Right = this.Left + value;
                        }
                }
                public RECT(int left, int top, int right, int bottom)
                {
                        this.Left = left;
                        this.Top = top;
                        this.Right = right;
                        this.Bottom = bottom;
                }
                public RECT(Rect rect)
                {
                        this.Left = (int)rect.Left;
                        this.Top = (int)rect.Top;
                        this.Right = (int)rect.Right;
                        this.Bottom = (int)rect.Bottom;
                }
                public void Offset(int dx, int dy)
                {
                        this.Left += dx;
                        this.Right += dx;
                        this.Top += dy;
                        this.Bottom += dy;
                }
                public Int32Rect ToInt32Rect()
                {
                        return new Int32Rect(this.Left, this.Top, this.Width, this.Height);
                }
        }
}
