using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class PanSoCrawler : WebCrawler
    {
        class Item
        {
            public string title
            {
                get; set;
            }
            public string link
            {
                get; set;
            }
            public string des
            {
                get; set;
            }
            public string blink
            {
                get; set;
            }
            public string host
            {
                get; set;
            }
            public object more
            {
                get; set;
            }
        }
        public PanSoCrawler()
        {
            this.ItemType = ItemType.BaiduShare;
            this.WebUrl = "http://pansou.com/";
            this.Name = "盘搜";
            this.SearchUrl = "http://106.15.195.249:8011/search_new?q={0}&p={1}";
            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                var obj = JsonConvert.DeserializeAnonymousType(requeststr, new
                {
                    list = new
                    {
                        data = new List<Item>()
                    }
                });

                if (obj != null
                        && obj.list != null
                         && obj.list.data != null)
                {
                    foreach (var item in obj.list.data)
                    {
                        BaiduNetDiskItem resultItem = new BaiduNetDiskItem()
                        {
                            OwnerWebCrawler = this
                        };

                        resultItem.Title = item.title;
                        resultItem.BaiduNetDiskLink = item.link;

                        int i = resultItem.Title.LastIndexOf('.');
                        if (i > 0)
                        {
                            resultItem.ResourceType = resultItem.Title.Substring(i).SubStringUntil(0, ' ');
                        }

                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                        {
                            continue;
                        }

                        resultItem.Size = (item.des + ",").SubStringInner("文件大小:", ",");
                        resultItem.CreateTime = item.des.SubStringInner("分享时间:", ",");

                        this.OnResultItemGenerated(resultItem);
                    }
                }
                else
                {
                    this.IsFinished = true;
                }
            }
        }

        public override void RequestDetail(ResultItem resultItem)
        {
            if (resultItem != null)
            {
                resultItem.IsDetailLoaded = true;
            }
        }
    }
}
