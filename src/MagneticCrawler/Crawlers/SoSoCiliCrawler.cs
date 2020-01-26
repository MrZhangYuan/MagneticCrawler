using HtmlAgilityPack;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace MagneticCrawler.Crawlers
{

    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class SoSoCiliCrawler : WebCrawler
    {
        public SoSoCiliCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "https://www.sscili.com";
            this.Name = "搜搜磁力";
            this.SearchUrl = "https://www.sscili.com/search-{0}-0-0-{1}.html";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(requeststr);

                HtmlNode listnode = doc.GetElementbyId("content");

                var itemnodes = listnode.ChildNodes[1].ChildNodes.Where(_p => _p.NodeType == HtmlNodeType.Element).ToList();

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

                        resultItem.Title = itemnode.GetInnerText(itemnode.XPath + "/dt/a");
                        resultItem.DetailUrl = this.WebUrl + HttpUtility.UrlDecode(itemnode.GetFirstAttrText(itemnode.XPath + "/dt/a", "href")); ;

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
                        }

                        resultItem.ResourceType = itemnode.GetInnerText(itemnode.XPath + "/dt/span");
                        resultItem.CreateTime = itemnode.GetInnerText(itemnode.XPath + "/dd/span/b");
                        resultItem.Size = itemnode.GetInnerText(itemnode.XPath + "/dd/span[3]").Replace("文档大小:", "");
                        resultItem.MagneticLink = itemnode.GetFirstAttrText(itemnode.XPath + "/dd/span[6]/a", "href");

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

                    HtmlNode linknode = document.DocumentNode.SelectSingleNode("/html/body/div[2]/div[3]/div/div[1]/div[3]/div[2]/div/a[3]");

                    //item.MagneticLink = linknode.GetFirstAttrText(linknode.XPath + "/li[9]/span[1]/a[1]", "href");
                    item.ThunderLink = linknode.GetAttributeValue("href","");

                    HtmlNodeCollection filelistnodes = document.DocumentNode.SelectSingleNode("/html[1]/body[1]/div[2]/div[3]/div[1]/div[1]/div[5]/div[2]/ul").ChildNodes;

                    if (filelistnodes != null
                            && filelistnodes.Count > 0)
                    {
                        int index = 1;
                        foreach (HtmlNode filenode in filelistnodes)
                        {
                            if (filenode.NodeType == HtmlNodeType.Element)
                            {
                                string size = filenode.GetInnerText(filenode.XPath + "/span");
                                item.AddFileItem(new FileItem
                                {
                                    Index = index++,
                                    FileName = filenode.GetInnerText(filenode.XPath).RemoveEnd(size).RemoveEnd("&nbsp;"),
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
