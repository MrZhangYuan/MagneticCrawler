using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class PanSoSoCrawler : WebCrawler
    {
        public PanSoSoCrawler()
        {
            this.ItemType = ItemType.BaiduShare;
            this.WebUrl = "http://www.pansoso.com";
            this.Name = "盘搜搜";
            this.SearchUrl = "http://www.pansoso.com/zh/{0}_{1}";
            this.ItemMatchString = @"<li>([\s\S]*?)</div></li>";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(requeststr);

                HtmlNode content = document.GetElementbyId("content");

                if (content != null)
                {
                    if (!content.HasChildNodes)
                    {
                        this.IsFinished = true;
                    }

                    foreach (var node in content.ChildNodes)
                    {
                        try
                        {
                            BaiduNetDiskItem resultItem = new BaiduNetDiskItem()
                            {
                                OwnerWebCrawler = this
                            };

                            var title = node.SelectSingleNode(node.XPath + "/h2[1]/a[1]");
                            if (title != null)
                            {
                                resultItem.DetailUrl = this.WebUrl + title.GetAttributeValue("href", "");
                                resultItem.Title = title.InnerText;

                                int i = resultItem.Title.LastIndexOf('.');
                                if (i > 0)
                                {
                                    resultItem.ResourceType = resultItem.Title.Substring(i).SubStringUntil(0, ' ');
                                }
                            }

                            if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                            {
                                continue;
                            }

                            var detail = node.SelectSingleNode(node.XPath + "/div[1]");
                            if (detail != null)
                            {
                                string value = detail.InnerText;
                                resultItem.Size = value.SubStringInner("文件大小:", ",");
                                resultItem.Author = value.SubStringInner("分享者:", ",");
                                resultItem.CreateTime = value.SubStringInner("分享时间:", ",");
                                resultItem.ActiveTimes = value.SubStringInner("浏览次数:", "<");
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

        protected override void OnRequestDetailSuccess(object param, string requeststr)
        {
            base.OnRequestDetailSuccess(param, requeststr);
            BaiduNetDiskItem item = param as BaiduNetDiskItem;

            try
            {
                if (!string.IsNullOrEmpty(requeststr)
                        && item != null)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(requeststr);

                    HtmlNode linknode = document.DocumentNode.SelectSingleNode("//a[@class='red']");

                    if (linknode != null)
                    {
                        string value = linknode.GetAttributeValue("href", "");
                        item.BaiduNetDiskLink = value;
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
