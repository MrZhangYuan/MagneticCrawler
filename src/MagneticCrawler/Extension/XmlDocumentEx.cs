using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace System.Xml
{
        public static class DomEx
        {
                public static string GetInnerText(this HtmlNode xmlDoc, string xpath)
                {
                        return xmlDoc.SelectSingleNode(xpath)?.InnerText;
                }

                public static string GetFirstAttrText(this HtmlNode xmlDoc, string xpath, string attrname)
                {
                        return xmlDoc.SelectSingleNode(xpath)?.GetAttributeValue(attrname, "");
                }

                public static string GetInnerText(this XmlNode xmlDoc, string xpath)
                {
                        return xmlDoc.SelectSingleNode(xpath)?.InnerText;
                }

                public static string GetFirstAttrText(this XmlNode xmlDoc, string xpath, string attrname)
                {
                        return (xmlDoc.SelectSingleNode(xpath) as XmlElement)?.GetAttribute(attrname);
                }
        }
}
