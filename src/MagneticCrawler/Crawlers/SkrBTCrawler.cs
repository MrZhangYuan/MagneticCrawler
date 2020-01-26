using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class SkrBTCrawler : WebCrawler
    {
        public SkrBTCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "https://skrbt4.xyz";
            this.Name = "SkrBT";
            this.SearchUrl = "https://skrbt4.xyz/search?keyword={0}&s=relevance&p={1}";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(requeststr);

                HtmlNode listnode = doc.DocumentNode.SelectSingleNode("/html/body/div/div[5]/div[2]");

                var itemnodes = listnode.ChildNodes.Where(_p => _p.NodeType == HtmlNodeType.Element && _p.Name == "ul").ToList();

                if (itemnodes.Count == 0)
                {
                    this.IsFinished = true;
                    return;
                }

                foreach (HtmlNode itemnode in itemnodes)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = itemnode.GetInnerText(itemnode.XPath + "/li[1]/a");
                        resultItem.DetailUrl = this.WebUrl + HttpUtility.UrlDecode(itemnode.GetFirstAttrText(itemnode.XPath + "/li[1]/a", "href"));

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
                        }

                        resultItem.CreateTime = itemnode.GetInnerText(itemnode.XPath + "/li[2]/span[3]");
                        resultItem.Size = itemnode.GetInnerText(itemnode.XPath + "/li[2]/span[1]");
                        resultItem.HotLevel = itemnode.GetInnerText(itemnode.XPath + "/li[2]/span[4]");

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

                    HtmlNode linknode = document.DocumentNode.SelectSingleNode("/html/body/div/div[3]/div[2]/table[1]/tr[5]/td[2]/a");
                    item.MagneticLink = linknode.GetAttributeValue("href", "");

                    var filelistnodes = document.DocumentNode.SelectSingleNode("/html/body/div/div[3]/div[2]/table[2]")
                                                        .ChildNodes
                                                        .Where(
                                                            _p => _p.NodeType == HtmlNodeType.Element
                                                                    && _p.Name == "tr"
                                                        )
                                                        .ToList();

                    if (filelistnodes != null
                            && filelistnodes.Count > 0)
                    {
                        int index = 1;
                        foreach (HtmlNode filenode in filelistnodes)
                        {
                            if (filenode.NodeType == HtmlNodeType.Element)
                            {
                                item.AddFileItem(new FileItem
                                {
                                    Index = index++,
                                    FileName = filenode.GetInnerText(filenode.XPath + "/td[2]").RemoveStart("&nbsp"),
                                    Size = filenode.GetInnerText(filenode.XPath + "/td[1]")
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
