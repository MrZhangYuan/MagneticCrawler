using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using System.Linq;
using System.ComponentModel.Composition;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class BtRabbitCrawler : WebCrawler
    {
        public BtRabbitCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://www.btrabbit.xyz/";
            this.Name = "BT兔子";
            this.SearchUrl = "http://www.btrabbit.xyz/search/{0}/default-{1}.html";
            this.ItemMatchString = @"<div class=""search-item detail-width"">([\s\S]*?)迅雷链接</a>";
            this.ItemDetailMatchString = @"<div class=""fileDetail"">([\s\S]*?)</div></div></div></div>";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                {
                    MatchCollection itemmatchs = Regex.Matches(requeststr, this.ItemMatchString, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    if (itemmatchs.Count == 0)
                    {
                        this.IsFinished = true;
                    }

                    foreach (Match item in itemmatchs)
                    {
                        string value = item.Value.Replace("\n", "").Replace("\r", "").Replace("&", "&amp;") + "--></div></div>";

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
                                XmlNodeList header = xmlDoc.SelectNodes("/div/div[1]/h3/a");
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

                                XmlNodeList comm = xmlDoc.SelectNodes("/div/div[3]");
                                if (comm.Count > 0)
                                {
                                    XmlElement element = comm[0] as XmlElement;
                                    XmlComment comment = element.LastChild as XmlComment;

                                    XmlDocument tempdoc = new XmlDocument();
                                    tempdoc.LoadXml("<TT>" + comment.Value + "</TT>");

                                    resultItem.MagneticLink = tempdoc.GetFirstAttrText("/TT/a[1]", "href");
                                    resultItem.ThunderLink = tempdoc.GetFirstAttrText("/TT/a[2]", "href");
                                }
                            }

                            this.OnResultItemGenerated(resultItem);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }


                //{
                //        HtmlDocument htmlDocument = new HtmlDocument();
                //        htmlDocument.LoadHtml(requeststr);

                //        HtmlNodeCollection itemnodes = htmlDocument.DocumentNode.SelectNodes("//div[@class='search-item detail-width']");

                //        if (itemnodes == null
                //                || itemnodes.Count == 0)
                //        {
                //                this.IsFinished = true;
                //        }

                //        var nodes = itemnodes.Select(_p => HtmlNode.CreateNode(_p.OuterHtml));

                //        ResultItem resultItem = new ResultItem()
                //        {
                //                OwnerWebCrawler = this
                //        };

                //        foreach (var itemnode in nodes)
                //        {
                //                HtmlNode head = itemnode.SelectSingleNode("/div[1]/div[1]/h3[1]/a[1]");
                //                resultItem.Title = head.InnerText;
                //                resultItem.DetailUrl = this.WebUrl + head.GetAttributeValue("href", "");


                //                resultItem.ResourceType = itemnode.SelectSingleNode("/div[1]/div[3]/span[1]").InnerText;
                //                resultItem.CreateTime = itemnode.SelectSingleNode("/div[1]/div[3]/span[2]/b").InnerText;
                //                resultItem.Size = itemnode.SelectSingleNode("/div[1]/div[3]/span[3]/b").InnerText;
                //                resultItem.HotLevel = itemnode.SelectSingleNode("/div[1]/div[3]/span[4]/b").InnerText;
                //                resultItem.ActiveTime = itemnode.SelectSingleNode("/div[1]/div[3]/span[5]/b").InnerText;


                //                HtmlNode comment = itemnode.SelectSingleNode("/div[1]/div[3]").Element("#comment");

                //                if (comment != null)
                //                {
                //                        comment = HtmlNode.CreateNode("<div>" + comment.OuterHtml.RemoveStart("<!--").RemoveEnd("-->") + "</div>");

                //                        resultItem.MagneticLink = comment.SelectSingleNode("/div[1]/a[1]").GetAttributeValue("href", "");
                //                        resultItem.ThunderLink = comment.SelectSingleNode("/div[1]/a[2]").GetAttributeValue("href", "");
                //                }
                //        }
                //}
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
                        item.ResourceType = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/table[1]/tr[2]/td[1]");
                        item.CreateTime = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/table[1]/tr[2]/td[2]");
                        item.ActiveTime = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/table[1]/tr[2]/td[3]");
                        item.HotLevel = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/table[1]/tr[2]/td[4]");
                        item.Size = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/table[1]/tr[2]/td[5]");

                        item.MagneticLink = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/div[1]/div[2]/a[1]");
                        item.ThunderLink = node.GetInnerText(node.XPath + "/div[1]/div[1]/div[1]/div[2]/div[2]/a[1]");

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
