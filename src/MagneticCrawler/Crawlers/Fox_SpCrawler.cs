using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using System.Xml;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel.Composition;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class Fox_SpCrawler : WebCrawler
    {
        public Fox_SpCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://www.gzqnf.com";
            this.Name = "磁力库";
            this.SearchUrl = "http://www.gzqnf.com/main-search-kw-{0}-px-1-page-{1}.html";
            this.ItemMatchString = @"<div class=""sopanel"">([\s\S]*?)</div>";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                Stopwatch sw = Stopwatch.StartNew();

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(requeststr);
                var items = document.DocumentNode.SelectNodes("//div[@class='panel panel-default']").Where(_p => _p.HasChildNodes && _p.ChildNodes.Count == 3);

                if (!items.Any())
                {
                    this.IsFinished = true;
                }

                foreach (var item in items)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        var head = document.DocumentNode.SelectSingleNode(item.XPath + "/div[1]/h5[1]/a[1]");
                        if (head != null)
                        {
                            resultItem.Title = head.InnerText;
                            resultItem.DetailUrl = this.WebUrl + head.GetAttributeValue("href", "");
                        }

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
                        }

                        resultItem.ResourceType = "未知";
                        resultItem.CreateTime = document.DocumentNode.GetInnerText(item.XPath + "/div[1]/table[1]/tr[1]/td[1]");
                        resultItem.Size = document.DocumentNode.GetInnerText(item.XPath + "/div[1]/table[1]/tr[1]/td[2]");
                        resultItem.DownloadSpeed = document.DocumentNode.GetInnerText(item.XPath + "/div[1]/table[1]/tr[1]/td[3]");
                        resultItem.HotLevel = document.DocumentNode.GetInnerText(item.XPath + "/div[1]/table[1]/tr[1]/td[4]");

                        if (!string.IsNullOrEmpty(resultItem.HotLevel))
                        {
                            resultItem.HotLevel = System.Web.HttpUtility.HtmlDecode(resultItem.HotLevel);
                        }

                        this.OnResultItemGenerated(resultItem);
                    }
                    catch (Exception)
                    {

                    }
                }
                sw.Stop();
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

                    item.MagneticLink = document.GetElementbyId("MagnetLink")?.InnerText;

                    HtmlNodeCollection filelistnodes = document.DocumentNode.SelectNodes("//table[@class='table table-striped']/tr");
                    if (filelistnodes != null
                            && filelistnodes.Count > 0)
                    {
                        for (int i = 1; i < filelistnodes.Count; i++)
                        {
                            string text = filelistnodes[i].GetInnerText(filelistnodes[i].XPath + "/td[1]");
                            string size = filelistnodes[i].GetInnerText(filelistnodes[i].XPath + "/td[2]");

                            item.AddFileItem(new FileItem
                            {
                                Index = i,
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
