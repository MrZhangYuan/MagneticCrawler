using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace UltimateCore
{
        public static class LayoutDoubleUtil
        {
                private const double eps = 1.53E-06;

                /// <summary>Determines whether the absolute value of the difference between the specified values is less than the double value 0.00000153. </summary>
                /// <param name="value1">The first value for comparison.</param>
                /// <param name="value2">The second value for comparison.</param>
                /// <returns>Returns true if the difference between the values is less than 0.00000153; otherwise returns false.</returns>
                public static bool AreClose(this double value1, double value2)
                {
                        if (value1.IsNonreal() || value2.IsNonreal())
                                return value1.CompareTo(value2) == 0;
                        if (value1 == value2)
                                return true;
                        double num = value1 - value2;
                        if (num < 1.53E-06)
                                return num > -1.53E-06;
                        return false;
                }

                /// <summary>Determines whether the absolute values of the differences between the left positions, top positions, heights and widths of the specified rectangles are less than the double value 0.00000153.</summary>
                /// <param name="rect1">The first rectangle for comparison.</param>
                /// <param name="rect2">The second rectangle for comparison.</param>
                /// <returns>Returns true if the differences between the values are less than 0.00000153; otherwise returns false.</returns>
                public static bool AreClose(Rect rect1, Rect rect2)
                {
                        return rect1.Location.AreClose(rect2.Location) && rect1.Size.AreClose(rect2.Size);
                }

                /// <summary />
                /// <param name="size1" />
                /// <param name="size2" />
                /// <returns />
                public static bool AreClose(this Size size1, Size size2)
                {
                        return size1.Width.AreClose(size2.Width) && size1.Height.AreClose(size2.Height);
                }

                /// <summary />
                /// <param name="size1" />
                /// <param name="size2" />
                /// <returns />
                public static bool AreClose(this Point size1, Point size2)
                {
                        return size1.X.AreClose(size2.X) && size1.Y.AreClose(size2.Y);
                }

                /// <summary>Determines whether the first specified value is less than the second specified value and the values are not within 0.00000153 of each other.</summary>
                /// <param name="value1">The first value for comparison.</param>
                /// <param name="value2">The second value for comparison.</param>
                /// <returns>Returns true if the first value is less than the second value and the values are not within 0.00000153 of each other; otherwise returns false.</returns>
                public static bool LessThan(this double value1, double value2)
                {
                        if (value1 < value2)
                                return !value1.AreClose(value2);
                        return false;
                }

                /// <summary />
                /// <param name="value1" />
                /// <param name="value2" />
                /// <returns />
                public static bool LessThanOrClose(this double value1, double value2)
                {
                        if (value1 >= value2)
                                return value1.AreClose(value2);
                        return true;
                }

                /// <summary>Determines whether the first specified value is greater than the second specified value and the values are not within 0.00000153 of each other.</summary>
                /// <param name="value1">The first value for comparison.</param>
                /// <param name="value2">The second value for comparison.</param>
                /// <returns>Returns true if the first value is greater than the second value and the values are not within 0.00000153 of each other; otherwise returns false.</returns>
                public static bool GreaterThan(this double value1, double value2)
                {
                        if (value1 > value2)
                                return !value1.AreClose(value2);
                        return false;
                }

                /// <summary />
                /// <param name="value1" />
                /// <param name="value2" />
                /// <returns />
                public static bool GreaterThanOrClose(this double value1, double value2)
                {
                        if (value1 <= value2)
                                return value1.AreClose(value2);
                        return true;
                }

                /// <summary />
                /// <param name="value" />
                /// <returns />
                public static bool IsNonreal(this double value)
                {
                        if (!double.IsNaN(value))
                                return double.IsInfinity(value);
                        return true;
                }
        }
}
