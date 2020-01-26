using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class CiliGouCrawler : WebCrawler
    {
        public CiliGouCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "https://www.ciligou.me";
            this.Name = "磁力狗";
            this.SearchUrl = "https://www.ciligou.me/search?word={0}&sort=rele&p={1}";
            this.ItemMatchString = @"<li>([\s\S]*?)</div></li>";
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
                    string value = item.Value.Replace("\n", "").Replace("\r", "").Replace("&", "&amp;");

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
                            XmlNodeList header = xmlDoc.SelectNodes("/li/div[1]/div/a");
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

                            resultItem.HotLevel = xmlDoc.GetInnerText("/li/div[2]/span");

                            //29文件大小：2.31 GB创建时间：2017-08-28文件格式：.mp4
                            string tempvalue = xmlDoc.GetInnerText("/li/div[2]");

                            resultItem.ResourceType = tempvalue.Substring(tempvalue.IndexOf("文件格式：") + 5);

                            resultItem.CreateTime = tempvalue.SubStringInner("创建时间：", "文件格式：");
                            resultItem.Size = tempvalue.SubStringInner("文件大小：", "创建时间：");
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

                    HtmlNode node = document.GetElementbyId("Information_container");

                    if (node != null)
                    {
                        item.MagneticLink = node.GetFirstAttrText(node.XPath + "/div[2]/div[1]/div[2]/a[1]", "href");

                        HtmlNodeCollection filelistnodes = node.SelectNodes("//ul/li");
                        if (filelistnodes != null
                                && filelistnodes.Count > 0)
                        {
                            for (int i = 0; i < filelistnodes.Count; i++)
                            {
                                string text = filelistnodes[i].InnerText;
                                string size = null;

                                if (filelistnodes[i].HasChildNodes)
                                {
                                    HtmlNode sizenode = filelistnodes[i].ChildNodes.FindFirst("div")?.ChildNodes?.FindFirst("div");
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
