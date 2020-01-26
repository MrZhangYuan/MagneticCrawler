using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class CLBCrawler : WebCrawler
    {
        public CLBCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "https://www.clb8.net";
            this.Name = "磁力吧";
            this.SearchUrl = "https://www.clb8.net/s/{0}_rel_{1}.html";
            this.ItemMatchString = @"<div class=""search-item"">([\s\S]*?)</span></div></div>";
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
                        string value = item.Value.Replace("\n", "").Replace("\r", "").Replace("<em>", "").Replace("</em>", "").Replace("<h3>", "").Replace("</h3>", "").Replace("&", "&amp;");

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

                            if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                            {
                                continue;
                            }

                            resultItem.ResourceType = xmlDoc.GetInnerText("/div/div[3]/span[1]");
                            resultItem.CreateTime = xmlDoc.GetInnerText("/div/div[3]/span[2]/b");
                            resultItem.Size = xmlDoc.GetInnerText("/div/div[3]/span[3]/b");
                            resultItem.HotLevel = xmlDoc.GetInnerText("/div/div[3]/span[4]/b");
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

                    HtmlNode node = document.GetElementbyId("content");

                    if (node != null)
                    {
                        item.Title = node.GetInnerText(node.XPath + "/div[1]/h1[1]");
                        item.CreateTime = node.GetInnerText(node.XPath + "/div[1]/div[1]/p[3]").Replace("收录时间：", "");
                        item.MagneticLink = node.GetFirstAttrText(node.XPath + "/div[1]/div[1]/p[6]/a[1]", "href");

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
