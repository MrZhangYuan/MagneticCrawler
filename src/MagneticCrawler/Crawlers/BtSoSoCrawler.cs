using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class BtSoSoCrawler : WebCrawler
    {
        public BtSoSoCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://www.btsoso.vip";
            this.Name = "BT搜搜";
            this.SearchUrl = "http://www.btsoso.vip/search/{0}_ctime_{1}.html";
            this.ItemMatchString = @"<li class=""media"">([\s\S]*?)</li>";
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
                        var head = document.DocumentNode.SelectNodes("/li/div/h4/a");
                        if (head.Count > 0)
                        {
                            HtmlNode first = head[0];

                            string title = first.GetAttributeValue("rel", "");
                            for (int i = 0; i < 3; i++)
                            {
                                title = Encoding.UTF8.GetString(Convert.FromBase64String(title));
                            }

                            resultItem.Title = title;

                            if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                            {
                                continue;
                            }

                            resultItem.MagneticLink = "magnet:?xt=urn:btih:" + first.GetInnerText("/li/div/h4/a");
                            resultItem.DetailUrl = this.WebUrl + first.GetAttributeValue("href", "");
                        }

                        resultItem.ResourceType = "未知";
                        resultItem.CreateTime = document.DocumentNode.GetInnerText("/li/div/div/span[1]");
                        resultItem.Size = document.DocumentNode.GetInnerText("/li/div/div/span[2]");
                        resultItem.HotLevel = document.DocumentNode.GetInnerText("/li/div/div/span[3]");

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
                //if (!string.IsNullOrEmpty(requeststr)
                //        && item != null)
                //{
                //        HtmlDocument document = new HtmlDocument();
                //        document.LoadHtml(requeststr);

                //        HtmlNode node = document.GetElementbyId("magnetLink");

                //        if (node != null)
                //        {
                //                item.MagneticLink = node.InnerText;
                //        }
                //}
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
