using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Linq;
using System.ComponentModel.Composition;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public class CiliFanHaoCrawler : WebCrawler
    {
        public CiliFanHaoCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://cilifanhaoba.me";
            this.Name = "磁力番号";
            this.SearchUrl = "http://cilifanhaoba.me/q/{0}/{1}/0/0.html";
            this.ItemMatchString = @"<dl class='item'>([\s\S]*?)</dl>";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            string result = HttpUtility.HtmlDecode(requeststr);

            if (result.Contains("内部服务器错误:服务器负载过高"))
            {
                this.IsFinished = true;
            }

            if (!string.IsNullOrEmpty(requeststr))
            {
                MatchCollection itemmatchs = Regex.Matches(requeststr, this.ItemMatchString, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (itemmatchs.Count == 0)
                {
                    this.IsFinished = true;
                }

                foreach (Match item in itemmatchs)
                {
                    string temp = HttpUtility.HtmlDecode(item.Value);

                    string value = temp;

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
                            XmlNodeList header = xmlDoc.SelectNodes("/dl/dt/a");
                            if (header.Count > 0)
                            {
                                XmlElement element = header[0] as XmlElement;
                                resultItem.Title = element.InnerText;
                                resultItem.DetailUrl = "http:" + element.GetAttribute("href");
                            }

                            if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                            {
                                continue;
                            }

                            resultItem.ResourceType = "未知";

                            resultItem.CreateTime = xmlDoc.GetInnerText("/dl/dd/span[1]/b");
                            resultItem.Size = xmlDoc.GetInnerText("/dl/dd/span[2]/b");
                            resultItem.DownloadSpeed = xmlDoc.GetInnerText("/dl/dd/span[4]/b");
                            resultItem.HotLevel = xmlDoc.GetInnerText("/dl/dd/span[5]/b");
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
                requeststr = HttpUtility.HtmlDecode(requeststr);

                if (!string.IsNullOrEmpty(requeststr)
                        && item != null)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(requeststr);

                    HtmlNode node = document.DocumentNode.SelectSingleNode("//div[@class='content']");

                    if (node != null)
                    {
                        item.ActiveTime = node.GetInnerText(node.XPath + "/div[1]/div[1]/p[2]/b[8]");
                        item.MagneticLink = node.GetFirstAttrText(node.XPath + "/div[1]/div[1]/p[6]/a[1]", "href");

                        HtmlNodeCollection filelistnodes1 = node.SelectNodes("//div[@class='dd filelist']/p");
                        HtmlNodeCollection filelistnodes2 = document.GetElementbyId("filelist_hidden").ChildNodes;

                        var filelist = filelistnodes1 == null
                                ?
                                filelistnodes2 == null ? null : filelistnodes2
                                :
                                filelistnodes2 == null ? filelistnodes1 : filelistnodes1.Concat(filelistnodes2);

                        if (filelist != null
                                && filelist.Any())
                        {
                            int i = 1;
                            foreach (var filenode in filelist)
                            {
                                string text = filenode.InnerText;
                                string size = null;

                                if (filenode.HasChildNodes)
                                {
                                    HtmlNode sizenode = filenode.ChildNodes[2];
                                    if (sizenode != null)
                                    {
                                        size = sizenode.InnerText;
                                        text = text.RemoveEnd(size);
                                    }
                                }

                                item.AddFileItem(new FileItem
                                {
                                    Index = i++,
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
