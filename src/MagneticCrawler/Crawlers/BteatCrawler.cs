using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class BteatCrawler : WebCrawler
    {
        public BteatCrawler()
        {
            this.WebUrl = "http://www.bteat.com";
            this.Name = "bt蚂蚁";
            this.SearchUrl = "http://www.bteat.com/search/{0}-first-asc-{1}";
            this.ItemMatchString = @"<div class=""search-item"">([\s\S]*?)迅雷链接</a>";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                MatchCollection itemmatchs = Regex.Matches(requeststr, this.ItemMatchString, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (itemmatchs.Count == 0)
                {
                    this.IsFinished = true;
                }

                foreach (Match item in itemmatchs)
                {
                    string value = item.Value.Replace("\n", "").Replace("\r", "").Replace("&", "&amp;") + "</div></div>";

                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(value);

                        if (xmlDoc.HasChildNodes)
                        {
                            XmlNodeList header = xmlDoc.SelectNodes("/div/div[1]/a");
                            if (header.Count > 0)
                            {
                                XmlElement element = header[0] as XmlElement;
                                resultItem.Title = element.InnerText;
                                resultItem.DetailUrl = this.WebUrl + element.GetAttribute("href");
                            }

                            resultItem.ResourceType = "未知";
                            resultItem.CreateTime = xmlDoc.GetInnerText("/div/div[3]/span[1]/b");
                            resultItem.Size = xmlDoc.GetInnerText("/div/div[3]/span[4]/b");
                            resultItem.HotLevel = xmlDoc.GetInnerText("/div/div[3]/span[2]/b");
                            resultItem.ActiveTime = xmlDoc.GetInnerText("/div/div[3]/span[3]/b");

                            resultItem.MagneticLink = xmlDoc.GetFirstAttrText("/div/div[3]/a[1]", "href");
                            resultItem.ThunderLink = xmlDoc.GetFirstAttrText("/div/div[3]/a[2]", "href");
                        }

                        this.OnResultItemGenerated(resultItem);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
