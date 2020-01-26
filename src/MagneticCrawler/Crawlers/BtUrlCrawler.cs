using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class BtUrlCrawler : WebCrawler
    {
        public BtUrlCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "https://www.bturl.pw";
            this.Name = "BT磁力链";
            this.SearchUrl = "https://www.bturl.pw/search/{0}_ctime_{1}.html";
            this.ItemMatchString = @"<li>([\s\S]*?)</li>";
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
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        string value = item.Value.Replace("\n", "").Replace("\r", "").Replace("&", "&amp;");

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(value);

                        if (xmlDoc.HasChildNodes)
                        {
                            XmlNodeList header = xmlDoc.SelectNodes("/li/h3/a");
                            if (header.Count > 0)
                            {
                                XmlElement element = header[0] as XmlElement;
                                resultItem.Title = element.InnerText;
                                resultItem.DetailUrl = this.WebUrl + element.GetAttribute("href");
                            }

                            if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                            {
                                continue;
                            }

                            resultItem.ResourceType = "未知";
                            resultItem.CreateTime = xmlDoc.GetInnerText("/li/dl/dt/span[1]");
                            resultItem.Size = xmlDoc.GetInnerText("/li/dl/dt/span[2]");
                            resultItem.HotLevel = xmlDoc.GetInnerText("/li/dl/dt/span[3]");
                        }

                        this.OnResultItemGenerated(resultItem);
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        protected override void OnRequestDetailSuccess(object param, string requeststr)
        {
            base.OnRequestDetailSuccess(param, requeststr);
            MagnetItem item = param as MagnetItem;

            try
            {
                if (!string.IsNullOrEmpty(requeststr)
                        && item != null)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(requeststr);

                    HtmlNode node = document.GetElementbyId("container");

                    if (node != null)
                    {
                        item.MagneticLink = node.GetFirstAttrText(node.XPath + "/div[1]/dl[1]/p[5]/a[1]", "href");

                        HtmlNodeCollection filelistnodes = node.SelectNodes("//ol/li");
                        if (filelistnodes != null
                                && filelistnodes.Count > 0)
                        {
                            for (int i = 0; i < filelistnodes.Count; i++)
                            {
                                string text = filelistnodes[i].InnerText;
                                string size = null;

                                if (filelistnodes[i].HasChildNodes)
                                {
                                    HtmlNode sizenode = filelistnodes[i].ChildNodes.FindFirst("span");
                                    if (sizenode != null)
                                    {
                                        size = sizenode.InnerText;
                                        text = text.RemoveEnd(size);
                                    }
                                }

                                item.AddFileItem(new FileItem
                                {
                                    Index = i + 1,
                                    FileName = text,
                                    Size = size
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                item.IsDetailLoaded = true;
            }
        }
    }
}
