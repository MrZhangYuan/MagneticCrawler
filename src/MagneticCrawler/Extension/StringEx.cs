using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
        public static class StringEx
        {
                public static string RemoveStart(this string value, string endstr)
                {
                        if (!string.IsNullOrEmpty(endstr)
                                && value.StartsWith(endstr))
                        {
                                value = value.Remove(0, endstr.Length);
                        }

                        return value;
                }
                public static string RemoveEnd(this string value, string endstr)
                {
                        if (!string.IsNullOrEmpty(endstr)
                                && value.EndsWith(endstr))
                        {
                                value = value.Remove(value.Length - endstr.Length);
                        }

                        return value;
                }
                public static string SubStringUntil(this string value, int index, char ch)
                {
                        if (value.Length > index)
                        {
                                string temp = value.Substring(index);

                                if (temp.Contains(ch))
                                {
                                        return temp.Substring(0, temp.IndexOf(ch));
                                }

                                return value;
                        }

                        return string.Empty;
                }

                public static string SubStringInner(this string value, char start, char end)
                {
                        if (value.Contains(start)
                                && value.Contains(end))
                        {
                                return value.Substring(value.IndexOf(start) + 1, value.IndexOf(end) - 1);
                        }
                        else if (value.Contains(start))
                        {
                                return value.Substring(value.IndexOf(start));
                        }
                        else if (value.Contains(end))
                        {
                                return value.Substring(0, value.IndexOf(end));
                        }

                        return string.Empty;
                }

                public static string SubStringInner(this string value, string start, string end)
                {
                        if (value.Contains(start))
                        {
                                value = value.Substring(value.IndexOf(start));
                        }

                        if (value.Contains(start)
                                && value.Contains(end))
                        {
                                int startindex = value.IndexOf(start) + start.Length;
                                return value.Substring(startindex, value.IndexOf(end) - startindex);
                        }
                        else if (value.Contains(start))
                        {
                                return value.Substring(value.IndexOf(start) + start.Length);
                        }
                        else if (value.Contains(end))
                        {
                                return value.Substring(0, value.IndexOf(end));
                        }

                        return string.Empty;
                }
        }
}
