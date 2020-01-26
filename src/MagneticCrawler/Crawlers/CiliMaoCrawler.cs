using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MagneticCrawler.Crawlers
{
    [Obsolete("引擎失效")]
    [Export(WebCrawler.CrawlerContractString, typeof(WebCrawler))]
    public sealed class CiliMaoCrawler : WebCrawler
    {
        class ContentWrapper
        {
            public Content search
            {
                get; set;
            }
        }
        class Content
        {
            public ContentResult result
            {
                get; set;
            }
            public ContentCard card
            {
                get; set;
            }
        }
        class ContentResult
        {
            public List<object> content
            {
                get; set;
            }
            public int total_pages
            {
                get; set;
            }
            public bool last
            {
                get; set;
            }
            public int total_elements
            {
                get; set;
            }
            public int number_of_elements
            {
                get; set;
            }
            public bool first
            {
                get; set;
            }
            public object sort
            {
                get; set;
            }
            public int size
            {
                get; set;
            }
            public int number
            {
                get; set;
            }
        }
        class ContentCard
        {
            public bool movieTag
            {
                get; set;
            }
            public Movie movie
            {
                get; set;
            }
            public Actress actress
            {
                get; set;
            }
        }
        public class Movie
        {
            public int id
            {
                get; set;
            }
            public int douban_id
            {
                get; set;
            }
            public string title
            {
                get; set;
            }
            public string countries
            {
                get; set;
            }
            public int current_season
            {
                get; set;
            }
            public int seasons_count
            {
                get; set;
            }
            public int current_episode
            {
                get; set;
            }
            public string cover_medium
            {
                get; set;
            }
            public string cover_large
            {
                get; set;
            }
            public string upyun_cover_large
            {
                get; set;
            }
            public double rate
            {
                get; set;
            }
            public string year
            {
                get; set;
            }
            public string release_date
            {
                get; set;
            }
            public string languages
            {
                get; set;
            }
            public string genres
            {
                get; set;
            }
            public string summary
            {
                get; set;
            }
            public int online_play_status
            {
                get; set;
            }
            public string subtype
            {
                get; set;
            }
            public int playable
            {
                get; set;
            }
            public bool downloadable
            {
                get; set;
            }
            public int priority
            {
                get; set;
            }
            public string original_title
            {
                get; set;
            }
            public string en_title
            {
                get; set;
            }
            public string aka
            {
                get; set;
            }
            public int ratings_count
            {
                get; set;
            }
            public int status
            {
                get; set;
            }
            public bool favorite
            {
                get; set;
            }
            public string encodeId
            {
                get; set;
            }
        }

        //艺人
        class Actress
        {

        }
        class ContentItem
        {
            public string title
            {
                get; set;
            }
            public string shareid
            {
                get; set;
            }
            public string uk
            {
                get; set;
            }
            public string shorturl
            {
                get; set;
            }

            public string infohash
            {
                get; set;
            }

            public bool favorite
            {
                get; set;
            }
            public int file_count
            {
                get; set;
            }
            public long content_size
            {
                get; set;
            }
            public string created_time
            {
                get; set;
            }
            public string feed_time
            {
                get; set;
            }
        }

        public CiliMaoCrawler()
        {
            this.ItemType = ItemType.Magnet_Baidu;
            //this.WebUrl = "https://www.cilimao.cc";
            this.WebUrl = "https://www.cilimao.info";
            this.Name = "磁力猫";
            this.SearchUrl = "https://www.cilimao.info/search?word={0}&page={1}";
            this.ItemMatchString = @"var __data =([\s\S]*?)</script>";
            this.ItemDetailMatchString = @"var __data =([\s\S]*?)</script>";

            this.CrawlerInfo.UserAgent = "Mozilla/5.0";
        }

        protected override void OnRequestSuccess(object param, string requeststr)
        {
            base.OnRequestSuccess(param, requeststr);

            if (!string.IsNullOrEmpty(requeststr))
            {
                MatchCollection itemmatchs = Regex.Matches(requeststr, this.ItemMatchString, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                if (itemmatchs.Count > 0)
                {
                    int first = itemmatchs[0].Value.IndexOf("{");
                    int last = itemmatchs[0].Value.LastIndexOf(";");
                    string value = itemmatchs[0].Value.Substring(first, last - first);

                    try
                    {
                        ContentWrapper content = JsonConvert.DeserializeObject<ContentWrapper>(value);

                        if (content != null
                                && content.search != null
                                && content.search.card != null
                                && content.search.card.movie != null)
                        {
                            TitleItem titleItem = new TitleItem
                            {
                                OwnerWebCrawler = this,
                                Title = content.search.card.movie.title,
                                ResourceType = content.search.card.movie.subtype,
                                DetailUrl = "https://www.cilimao.cc/baike/movie/" + content.search.card.movie.encodeId,
                                MovieTitle = content.search.card.movie,
                            };
                            this.OnTitleItemGenerated(titleItem);
                        }

                        if (content != null
                                && content.search != null
                                && content.search.result != null
                                && content.search.result.content != null
                                && content.search.result.content.Count > 0)
                        {
                            foreach (var item in content.search.result.content)
                            {
                                JObject jobj = item as JObject;
                                if (jobj != null)
                                {
                                    ContentItem contentItem = JsonConvert.DeserializeObject<ContentItem>(jobj.ToString());
                                    if (contentItem != null)
                                    {
                                        ResultItem resultItem = null;
                                        if (!string.IsNullOrEmpty(contentItem.shorturl))
                                        {
                                            resultItem = new BaiduNetDiskItem
                                            {
                                                OwnerWebCrawler = this,
                                                ResourceType = "未知",
                                                Title = contentItem.title,
                                                DetailUrl = string.IsNullOrEmpty(contentItem.infohash) ? string.Empty : "https://www.cilimao.cc/information/" + contentItem.infohash,
                                                BaiduNetDiskLink = string.IsNullOrEmpty(contentItem.shorturl) ? string.Empty : "https://pan.baidu.com/s/" + contentItem.shorturl,
                                                CreateTime = contentItem.created_time,
                                                Size = ((double)contentItem.content_size / 1024 / 1024 / 1024).ToString("0.00") + " GB",
                                            };
                                        }
                                        else
                                        {
                                            resultItem = new MagnetItem()
                                            {
                                                OwnerWebCrawler = this,
                                                ResourceType = "未知",
                                                Title = contentItem.title,
                                                DetailUrl = string.IsNullOrEmpty(contentItem.infohash) ? string.Empty : "https://www.cilimao.cc/information/" + contentItem.infohash,
                                                MagneticLink = string.IsNullOrEmpty(contentItem.infohash) ? string.Empty : "magnet:?xt=urn:btih:" + contentItem.infohash,
                                                CreateTime = contentItem.created_time,
                                                Size = ((double)contentItem.content_size / 1024 / 1024 / 1024).ToString("0.00") + " GB",
                                            };
                                        }

                                        if (!this.CheckItemNeedCollect(resultItem, param as ItemRequestParam))
                                        {
                                            continue;
                                        }

                                        this.OnResultItemGenerated(resultItem);
                                    }
                                }
                            }
                        }
                        else
                        {
                            this.IsFinished = true;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                else
                {
                    this.IsFinished = true;
                }
            }
        }

        class ContentInfomationWrapper
        {
            public ContentInfomation information
            {
                get; set;
            }
        }
        class ContentInfomation
        {
            public List<FileListItem> magnet_file_list
            {
                get; set;
            }
            public string updated_time
            {
                get; set;
            }
            public int download_count
            {
                get; set;
            }
        }
        class FileListItem
        {
            public string name
            {
                get; set;
            }
            public long length
            {
                get; set;
            }
        }
        protected override void OnRequestDetailSuccess(object param, string requeststr)
        {
            base.OnRequestDetailSuccess(param, requeststr);

            MagnetItem item = param as MagnetItem;

            if (item != null
                    && !string.IsNullOrEmpty(requeststr))
            {
                try
                {
                    MatchCollection itemmatchs = Regex.Matches(requeststr, this.ItemMatchString, RegexOptions.IgnoreCase | RegexOptions.Multiline);

                    if (itemmatchs.Count > 0)
                    {
                        int first = itemmatchs[0].Value.IndexOf("{");
                        int last = itemmatchs[0].Value.LastIndexOf(";");
                        string value = itemmatchs[0].Value.Substring(first, last - first);

                        ContentInfomationWrapper infomationWrapper = JsonConvert.DeserializeObject<ContentInfomationWrapper>(value);

                        if (infomationWrapper != null)
                        {
                            item.UpdateTime = infomationWrapper.information.updated_time;
                            item.HotLevel = infomationWrapper.information.download_count + "";

                            if (infomationWrapper.information.magnet_file_list != null)
                            {
                                int i = 1;
                                foreach (var fileitem in infomationWrapper.information.magnet_file_list)
                                {
                                    item.AddFileItem(new FileItem
                                    {
                                        Index = i++,
                                        FileName = fileitem.name,
                                        Size = ((double)fileitem.length / 1024 / 1024 / 1024).ToString("0.00") + " GB"
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
}
