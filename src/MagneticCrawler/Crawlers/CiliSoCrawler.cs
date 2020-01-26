using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class CiLiNingMengCrawler : WebCrawler
    {
        public CiLiNingMengCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://bt.cywacg.com";
            this.Name = "磁力柠檬";
            this.SearchUrl = "http://bt.cywacg.com/api/search?source=磁力柠檬&keyword={0}&page={1}&sort=time";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }
        public class _Result
        {
            public bool success
            {
                get; set;
            }
            public _Data data
            {
                get; set;
            }
        }

        public class _Data
        {
            public List<_Item> results
            {
                get; set;
            }
        }

        public class _Item
        {
            public string magnet
            {
                get; set;
            }
            public string name
            {
                get; set;
            }
            public string formatSize
            {
                get; set;
            }
            public string date
            {
                get; set;
            }
            public string hot
            {
                get; set;
            }
            public string detailUrl
            {
                get; set;
            }
        }
        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                _Result content = JsonConvert.DeserializeObject<_Result>(requeststr);

                if (!content.success
                    || content.data.results == null
                    || content.data.results.Count == 0)
                {
                    this.IsFinished = true;
                    return;
                }

                foreach (var item in content.data.results)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = item.name;
                        resultItem.DetailUrl = item.detailUrl;
                        resultItem.MagneticLink = item.magnet;
                        resultItem.Size = item.formatSize;
                        resultItem.UpdateTime = item.date;
                        resultItem.HotLevel = item.hot;

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
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
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(requeststr);
                var filelistnodes = document.DocumentNode.SelectSingleNode("/html/body/div/div/div[2]/div[7]/div[2]/ul")
                                                    .ChildNodes
                                                    .Where(
                                                        _p => _p.NodeType == HtmlNodeType.Element
                                                                && _p.Name == "li"
                                                    )
                                                    .ToList();

                if (filelistnodes != null
                        && filelistnodes.Count > 0)
                {
                    int index = 1;
                    foreach (HtmlNode filenode in filelistnodes)
                    {
                        item.AddFileItem(new FileItem
                        {
                            Index = index++,
                            FileName = filenode.GetInnerText(filenode.XPath + "/span[1]").RemoveStart("&nbsp"),
                            Size = filenode.GetInnerText(filenode.XPath + "/span[2]")
                        });
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


    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class MAGCrawler : WebCrawler
    {
        public MAGCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://bt.cywacg.com";
            this.Name = "MAG磁力站";
            this.SearchUrl = "http://bt.cywacg.com/api/search?source=MAG磁力站&keyword={0}&page={1}&sort=default";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }
        public class _Result
        {
            public bool success
            {
                get; set;
            }
            public _Data data
            {
                get; set;
            }
        }

        public class _Data
        {
            public List<_Item> results
            {
                get; set;
            }
        }

        public class _Item
        {
            public string magnet
            {
                get; set;
            }
            public string name
            {
                get; set;
            }
            public string formatSize
            {
                get; set;
            }
            public string date
            {
                get; set;
            }
            public string hot
            {
                get; set;
            }
            public string detailUrl
            {
                get; set;
            }
        }
        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                _Result content = JsonConvert.DeserializeObject<_Result>(requeststr);

                if (!content.success
                    || content.data.results == null
                    || content.data.results.Count == 0)
                {
                    this.IsFinished = true;
                    return;
                }

                foreach (var item in content.data.results)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = item.name;
                        resultItem.DetailUrl = item.detailUrl;
                        resultItem.MagneticLink = item.magnet;
                        resultItem.Size = item.formatSize;
                        resultItem.UpdateTime = item.date;
                        resultItem.HotLevel = item.hot;

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
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
                item.AddFileItem(new FileItem
                {
                    FileName = "未知"
                });
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


    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class CiLiGuoCrawler : WebCrawler
    {
        public CiLiGuoCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://bt.cywacg.com";
            this.Name = "磁力果";
            this.SearchUrl = "http://bt.cywacg.com/api/search?source=磁力果&keyword={0}&page={1}&sort=default";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }
        public class _Result
        {
            public bool success
            {
                get; set;
            }
            public _Data data
            {
                get; set;
            }
        }

        public class _Data
        {
            public List<_Item> results
            {
                get; set;
            }
        }

        public class _Item
        {
            public string magnet
            {
                get; set;
            }
            public string name
            {
                get; set;
            }
            public string formatSize
            {
                get; set;
            }
            public string date
            {
                get; set;
            }
            public string hot
            {
                get; set;
            }
            public string detailUrl
            {
                get; set;
            }
        }
        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                _Result content = JsonConvert.DeserializeObject<_Result>(requeststr);

                if (!content.success
                    || content.data.results == null
                    || content.data.results.Count == 0)
                {
                    this.IsFinished = true;
                    return;
                }

                foreach (var item in content.data.results)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = item.name;
                        resultItem.DetailUrl = item.detailUrl;
                        resultItem.MagneticLink = item.magnet;
                        resultItem.Size = item.formatSize;
                        resultItem.UpdateTime = item.date;
                        resultItem.HotLevel = item.hot;

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
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
                    var filelistnodes = document.DocumentNode.SelectSingleNode(@"//*[@id=""__layout""]/div/div[1]/div[2]/div[1]/div/div[3]/div[2]")
                                                        .ChildNodes
                                                        .Where(
                                                            _p => _p.NodeType == HtmlNodeType.Element
                                                                    && _p.Name == "div"
                                                        )
                                                        .ToList();

                    if (filelistnodes != null
                            && filelistnodes.Count > 0)
                    {
                        int index = 1;
                        foreach (HtmlNode filenode in filelistnodes)
                        {
                            item.AddFileItem(new FileItem
                            {
                                Index = index++,
                                FileName = filenode.GetInnerText(filenode.XPath + "/div[1]/div[1]/span"),
                                Size = filenode.GetInnerText(filenode.XPath + "/div[1]/div[2]/span")
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


    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class ZhongZiSoCrawler : WebCrawler
    {
        public ZhongZiSoCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "http://bt.cywacg.com";
            this.Name = "种子搜";
            this.SearchUrl = "http://bt.cywacg.com/api/search?source=种子搜&keyword={0}&page={1}&sort=time";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }
        public class _Result
        {
            public bool success
            {
                get; set;
            }
            public _Data data
            {
                get; set;
            }
        }

        public class _Data
        {
            public List<_Item> results
            {
                get; set;
            }
        }

        public class _Item
        {
            public string magnet
            {
                get; set;
            }
            public string name
            {
                get; set;
            }
            public string formatSize
            {
                get; set;
            }
            public string date
            {
                get; set;
            }
            public string hot
            {
                get; set;
            }
            public string detailUrl
            {
                get; set;
            }
        }
        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                _Result content = JsonConvert.DeserializeObject<_Result>(requeststr);

                if (!content.success
                    || content.data.results == null
                    || content.data.results.Count == 0)
                {
                    this.IsFinished = true;
                    return;
                }

                foreach (var item in content.data.results)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = item.name;
                        resultItem.DetailUrl = item.detailUrl;
                        resultItem.MagneticLink = item.magnet;
                        resultItem.Size = item.formatSize;
                        resultItem.UpdateTime = item.date;
                        resultItem.HotLevel = item.hot;

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
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
                item.AddFileItem(new FileItem
                {
                    FileName = "未知"
                });
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
