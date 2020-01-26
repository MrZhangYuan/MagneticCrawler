using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UltimatePresentation.Native;

namespace UltimatePresentation.Presentation
{
        public static class DpiHelper
        {
                private delegate void PixelProcessor(ref byte alpha, ref byte red, ref byte green, ref byte blue);
                private const double LogicalDpi = 96.0;
                private static MatrixTransform transformFromDevice;
                private static MatrixTransform transformToDevice;
                public static MatrixTransform TransformFromDevice
                {
                        get
                        {
                                return DpiHelper.transformFromDevice;
                        }
                }
                public static MatrixTransform TransformToDevice
                {
                        get
                        {
                                return DpiHelper.transformToDevice;
                        }
                }
                public static double DeviceDpiX
                {
                        get;
                        private set;
                }
                public static double DeviceDpiY
                {
                        get;
                        private set;
                }
                public static double LogicalToDeviceUnitsScalingFactorX
                {
                        get
                        {
                                return DpiHelper.TransformToDevice.Matrix.M11;
                        }
                }
                public static double LogicalToDeviceUnitsScalingFactorY
                {
                        get
                        {
                                return DpiHelper.TransformToDevice.Matrix.M22;
                        }
                }
                static DpiHelper()
                {
                        IntPtr dC = NativeMethodsUltimate.GetDC(IntPtr.Zero);
                        if (dC != IntPtr.Zero)
                        {
                                DpiHelper.DeviceDpiX = (double)NativeMethodsUltimate.GetDeviceCaps(dC, 88);
                                DpiHelper.DeviceDpiY = (double)NativeMethodsUltimate.GetDeviceCaps(dC, 90);
                                NativeMethodsUltimate.ReleaseDC(IntPtr.Zero, dC);
                        }
                        else
                        {
                                DpiHelper.DeviceDpiX = 96.0;
                                DpiHelper.DeviceDpiY = 96.0;
                        }
                        System.Windows.Media.Matrix identity = System.Windows.Media.Matrix.Identity;
                        System.Windows.Media.Matrix identity2 = System.Windows.Media.Matrix.Identity;
                        identity.Scale(DpiHelper.DeviceDpiX / 96.0, DpiHelper.DeviceDpiY / 96.0);
                        identity2.Scale(96.0 / DpiHelper.DeviceDpiX, 96.0 / DpiHelper.DeviceDpiY);
                        DpiHelper.transformFromDevice = new MatrixTransform(identity2);
                        DpiHelper.transformFromDevice.Freeze();
                        DpiHelper.transformToDevice = new MatrixTransform(identity);
                        DpiHelper.transformToDevice.Freeze();
                }
                public static Rect LogicalToDeviceUnits(this Rect logicalRect)
                {
                        Rect result = logicalRect;
                        result.Transform(DpiHelper.TransformToDevice.Matrix);
                        return result;
                }
                public static Rect DeviceToLogicalUnits(this Rect deviceRect)
                {
                        Rect result = deviceRect;
                        result.Transform(DpiHelper.TransformFromDevice.Matrix);
                        return result;
                }
                public static double DeviceToLogicalUnitsScalingFactorX
                {
                        get
                        {
                                return DpiHelper.TransformFromDevice.Matrix.M11;
                        }
                }
                public static double DeviceToLogicalUnitsScalingFactorY
                {
                        get
                        {
                                return DpiHelper.TransformFromDevice.Matrix.M22;
                        }
                }
                public static System.Windows.Point DeviceToLogicalUnits(this System.Windows.Point devicePoint)
                {
                        return devicePoint;
                        //return DpiHelper.Instance.DeviceToLogicalUnits(devicePoint);
                }

                public static System.Windows.Size LogicalToDeviceUnits(this System.Windows.Size logicalSize)
                {
                        return logicalSize;
                        //return DpiHelper.Instance.LogicalToDeviceUnits(logicalSize);
                }
        }

}
