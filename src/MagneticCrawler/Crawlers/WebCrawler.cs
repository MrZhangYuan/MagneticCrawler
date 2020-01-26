using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MagneticCrawler.Crawlers
{
    public class ItemRequestParam
    {
        public string Text
        {
            get; set;
        }
        public string[] Keys
        {
            get;
            set;
        }
    }

    public enum ItemType
    {
        All,
        Magnet,
        BaiduShare,
        Magnet_Baidu,
        Subtitle
    }

    public class CrawlerInfo
    {
        public string HttpMethod
        {
            get;
            set;
        }
        public string UserAgent
        {
            get;
            set;
        }
        public Encoding Encoding
        {
            get;
            set;
        }

        public CrawlerInfo()
        {
            HttpMethod = "GET";
            Encoding = Encoding.UTF8;
        }
    }

    public abstract class WebCrawler
    {
        public const string CrawlerContractString = "D27C636F-B490-4A73-AA1F-1B3B9D3EFFA9";

        public static int TakePageCount { get; } = 1;

        private int _requestCounter = 0;

        public string WebUrl
        {
            get;
            protected set;
        }

        public ItemType ItemType
        {
            get;
            protected set;
        }

        public CrawlerInfo CrawlerInfo
        {
            get;
            protected set;
        }

        public string Name
        {
            get;
            protected set;
        }

        public int CurrentPageIndex
        {
            get;
            protected set;
        }

        public string SearchUrl
        {
            get;
            protected set;
        }

        public string ItemMatchString
        {
            get;
            protected set;
        }

        public string ItemDetailMatchString
        {
            get;
            protected set;
        }


        private bool _isFinished;
        public bool IsFinished
        {
            get
            {
                return this._isFinished;
            }
            protected set
            {
                this._isFinished = value;
                if (value)
                {
                    this.OnSearchFinished(this);
                }
            }
        }


        private bool _isComplated;
        public bool IsComplated
        {
            get
            {
                return _isComplated;
            }
            protected set
            {
                this._isComplated = value;

                if (value)
                {
                    this.OnSearchComplated(this);
                }
            }
        }

        public event Action<ResultItem> ResultItemGenerated;
        protected void OnResultItemGenerated(ResultItem resultItem)
        {
            this.ResultItemGenerated?.Invoke(resultItem);
        }

        public event Action<TitleItem> TitleItemGenerated;
        protected void OnTitleItemGenerated(TitleItem resultItem)
        {
            this.TitleItemGenerated?.Invoke(resultItem);
        }

        public event Action<object> SearchComplated;
        protected void OnSearchComplated(object obj)
        {
            this.SearchComplated?.Invoke(obj);
        }

        public event Action<object> SearchFinished;
        protected void OnSearchFinished(object obj)
        {
            this.SearchFinished?.Invoke(obj);
        }

        public WebCrawler()
        {
            this.CurrentPageIndex = 1;
            this.ItemType = ItemType.All;
            this.CrawlerInfo = new CrawlerInfo();
        }

        public void ReSetPage()
        {
            this.CurrentPageIndex = 1;
        }

        public virtual void Start(string searchtext)
        {
            if (this.CheckIfStartNecessary())
            {
                ItemRequestParam param = new ItemRequestParam
                {
                    Text = searchtext,
                    Keys = this.ConvertSearchString(searchtext)
                };

                int index = this.CurrentPageIndex;
                this.IsComplated = false;

                for (; this.CurrentPageIndex < TakePageCount + index; this.CurrentPageIndex++)
                {
                    WebRequestHelper.RequestAsync(string.Format(this.SearchUrl, HttpUtility.UrlEncode(searchtext), this.CurrentPageIndex), this.CrawlerInfo.UserAgent, this.OnRequestSuccess, this.OnRequestError, param);
                    Interlocked.Increment(ref this._requestCounter);
                }
            }
        }

        public virtual bool CheckItemNeedCollect(ResultItem resultItem, ItemRequestParam param)
        {
            //启用搜索引擎筛选
            return true;

            //启用软件筛选
            if (resultItem != null
                    && param != null
                    && param.Keys != null
                    && param.Keys.Length > 1)
            {
                string title = resultItem.Title.ToUpper();

                foreach (var key in param.Keys.Skip(1))
                {
                    if (!title.Contains(key.ToUpper()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public virtual string[] ConvertSearchString(string searchtext)
        {
            if (!string.IsNullOrEmpty(searchtext)
                    && searchtext.Contains(" "))
            {
                return searchtext.Split(' ');
            }

            return new string[] { searchtext };
        }

        public virtual void Stop()
        {

        }

        public virtual bool CheckIfStartNecessary()
        {
            return this._requestCounter == 0
                            && !this.IsFinished;
        }


        protected virtual void OnRequestError(object param, string requeststr)
        {
            Interlocked.Decrement(ref this._requestCounter);

            if (this._requestCounter == 0)
            {
                this.IsComplated = true;
            }
        }
        protected virtual void OnRequestSuccess(object param, string requeststr)
        {
            Interlocked.Decrement(ref this._requestCounter);

            if (this._requestCounter == 0)
            {
                this.IsComplated = true;
            }
        }
        protected virtual void OnRequestDetailSuccess(object param, string requeststr)
        {

        }

        protected virtual void OnRequestDetailError(object param, string requeststr)
        {

        }

        public virtual void RequestDetail(ResultItem resultItem)
        {
            if (resultItem != null)
            {
                WebRequestHelper.RequestAsync(resultItem.DetailUrl, this.CrawlerInfo.UserAgent, this.OnRequestDetailSuccess, this.OnRequestDetailError, resultItem);
            }
        }
    }
}
