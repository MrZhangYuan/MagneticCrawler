using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace MagneticCrawler.Crawlers
{
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class CiliwangCrawler : WebCrawler
    {
        public CiliwangCrawler()
        {
            this.ItemType = ItemType.Magnet;
            this.WebUrl = "https://www.ciliwang.club";
            this.Name = "磁力王";
            this.SearchUrl = "https://www.ciliwang.club/search?keyword={0}&sort=hot&page={1}&mode=1";
            //this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }

        public class _CiliwangResult
        {
            public int status
            {
                get; set;
            }
            public string info
            {
                get; set;
            }
            public _CiliwangData data
            {
                get; set;
            }
        }

        public class _CiliwangData
        {
            public int page_size
            {
                get; set;
            }
            public int page
            {
                get; set;
            }
            public int time
            {
                get; set;
            }
            public int total
            {
                get; set;
            }
            public List<_CiliwangItem> data
            {
                get; set;
            }
        }

        public class _CiliwangItem
        {
            public int down_level
            {
                get; set;
            }
            public int is_new
            {
                get; set;
            }
            public int is_hot
            {
                get; set;
            }

            public string name
            {
                get; set;
            }
            public long id
            {
                get; set;
            }
            public string time
            {
                get; set;
            }
            public int hot
            {
                get; set;
            }
            public string uri
            {
                get; set;
            }
            public int is_fire
            {
                get; set;
            }
            public int down_num
            {
                get; set;
            }
            public int file_num
            {
                get; set;
            }
            public string file_size
            {
                get; set;
            }
            public object _score
            {
                get; set;
            }
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                _CiliwangResult content = JsonConvert.DeserializeObject<_CiliwangResult>(requeststr);

                if (content.status != 200
                    || content.data.data == null
                    || content.data.data.Count == 0)
                {
                    this.IsFinished = true;
                    return;
                }

                foreach (var item in content.data.data)
                {
                    try
                    {
                        MagnetItem resultItem = new MagnetItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = item.name;
                        resultItem.DetailUrl = string.Format("https://www.ciliwang.club/source/{0}/detail.html", item.id);

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
                        }

                        resultItem.CreateTime = item.time;
                        resultItem.Size = item.file_size;
                        resultItem.HotLevel = item.hot + "";
                        resultItem.MagneticLink = item.uri;

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

                    var filelistnodes = document.DocumentNode.SelectSingleNode("/html/body/div[2]/div[1]/div/ul[2]/li[5]/div/ol")
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
                            string size = filenode.GetInnerText(filenode.XPath + "/span");

                            item.AddFileItem(new FileItem
                            {
                                Index = index++,
                                FileName = filenode.GetInnerText(filenode.XPath).RemoveEnd(size),
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
