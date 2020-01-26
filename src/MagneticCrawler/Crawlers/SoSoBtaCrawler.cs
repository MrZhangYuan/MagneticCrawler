using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class SoSoBtaCrawler : WebCrawler
    {
        public SoSoBtaCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://www.sosobta.com";
            this.Name = "搜搜BT";
            this.SearchUrl = "http://www.sosobta.com/s/{0}/{1}.html";
            this.ItemMatchString = @"<div class=""r"">([\s\S]*?)</div>";
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

                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(item.Value);
                        var head = document.DocumentNode.SelectNodes("/div/a");
                        if (head != null)
                        {
                            if (head.Count > 0)
                            {
                                HtmlNode first = head[0];
                                resultItem.Title = HttpUtility.UrlDecode(first.InnerText.Replace("document.write(decodeURIComponent(", "").Replace("));", "").Replace("+", "").Replace("\"", "")).Replace("<fontcolor=\"red\">", "").Replace("</font><fontcolor=\"red\">", "").Replace("</font>", "");
                                resultItem.DetailUrl = this.WebUrl + HttpUtility.UrlDecode(first.GetAttributeValue("href", ""));
                            }

                            if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                            {
                                continue;
                            }

                            resultItem.ResourceType = "未知";
                            resultItem.CreateTime = document.DocumentNode.GetInnerText("/div/span[1]/b");
                            resultItem.Size = document.DocumentNode.GetInnerText("/div/span[2]/b");

                            resultItem.MagneticLink = document.DocumentNode.GetFirstAttrText("/div/a[2]", "href");

                            this.OnResultItemGenerated(resultItem);
                        }
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
                    HtmlNode detailnode = node?.SelectSingleNode(node.XPath + "/div[1]/ul[1]");
                    if (detailnode != null)
                    {
                        item.UpdateTime = detailnode.GetInnerText(detailnode.XPath + "/li[3]/span[1]").Replace("更新时间：", "");
                        item.HotLevel = detailnode.GetInnerText(detailnode.XPath + "/li[6]/span[1]").Replace("访问热度：", "");

                        item.MagneticLink = detailnode.GetFirstAttrText(detailnode.XPath + "/li[9]/span[1]/a[1]", "href");
                        item.ThunderLink = detailnode.GetFirstAttrText(detailnode.XPath + "/li[10]/span[1]/a[1]", "href");
                    }

                    HtmlNodeCollection filelistnodes = document.GetElementbyId("filelist").ChildNodes;
                    if (filelistnodes != null
                            && filelistnodes.Count > 0)
                    {
                        for (int i = 0; i < filelistnodes.Count; i++)
                        {

                            string source = filelistnodes[i].GetInnerText(filelistnodes[i].XPath + "/span[1]/script[1]").SubStringInner("document.write(decodeURIComponent(", "));").Replace("+", "").Replace("\"", "");
                            string text = HttpUtility.UrlDecode(source);
                            string size = null;

                            size = filelistnodes[i].GetInnerText(filelistnodes[i].XPath + "/span[2]");
                            text = text.RemoveEnd(size);

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
