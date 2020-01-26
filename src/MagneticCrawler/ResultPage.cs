using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using MagneticCrawler.Crawlers;
using System.Windows.Threading;
using System.Diagnostics;
using SmartCashier.Infrastructure.DataVirtualization;
using System.Collections.Concurrent;
using System.Threading;

namespace MagneticCrawler
{
        public class ResultPage : ObservableObject
        {
                public int PageID
                {
                        get;
                }
                public string SearchText
                {
                        get;
                }

                public Filter Filter
                {
                        get;
                }

                private TitleItem _titleItem;
                public TitleItem TitleItem
                {
                        get
                        {
                                return _titleItem;
                        }
                        set
                        {
                                this._titleItem = value;
                                this.RaisePropertyChanged(nameof(TitleItem));
                        }
                }

                public ObservableCollection<ResultItem> ResultItems
                {
                        get;
                }


                private bool _isComplated;
                public bool IsComplated
                {
                        get
                        {
                                return _isComplated;
                        }
                        set
                        {
                                this._isComplated = value;
                                this.RaisePropertyChanged(nameof(IsComplated));
                        }
                }


                private bool _isFinished;
                public bool IsFinished
                {
                        get
                        {
                                return _isFinished;
                        }
                        set
                        {
                                this._isFinished = value;
                                this.RaisePropertyChanged(nameof(IsFinished));
                        }
                }


                private int _searchCount = 1;
                /// <summary>
                /// 第几次检索
                /// </summary>
                public int SearchCount
                {
                        get
                        {
                                return _searchCount;
                        }
                        set
                        {
                                this._searchCount = value;
                                this.RaisePropertyChanged(nameof(SearchCount));
                        }
                }

                private readonly List<WebCrawler> _webCrawlers = null;

                private readonly HashSet<ResultItem> _items = null;
                public ResultPage(int pageID, string searchText, IEnumerable<WebCrawler> webCrawlers)
                {
                        this.Filter = new Filter
                        {

                        };

                        this.Filter.ConditionChanged += Filter_ConditionChanged;

                        this.PageID = PageID;
                        this.SearchText = searchText;
                        this._webCrawlers = webCrawlers.ToList();
                        for (int i = 0; i < this._webCrawlers.Count; i++)
                        {
                                this._webCrawlers[i].ResultItemGenerated += ResultPage_OnGenerateResultItem;
                                this._webCrawlers[i].SearchComplated += ResultPage_SearchComplated;
                                this._webCrawlers[i].SearchFinished += ResultPage_SearchFinished;
                                this._webCrawlers[i].TitleItemGenerated += ResultPage_TitleItemGenerated;
                        }

                        this.ResultItems = new ObservableCollection<ResultItem>();

                        this._items = new HashSet<ResultItem>();
                }

                private void Filter_ConditionChanged(object obj)
                {
                        this.IsFinished = false;
                        this.IsComplated = false;
                        this.SearchCount = 1;

                        this.ResultItems.Clear();
                        this._items.Clear();

                        foreach (var item in this._webCrawlers)
                        {
                                item.ReSetPage();
                        }

                        this.Start();
                }

                public bool FilterItem(ResultItem item)
                {
                        bool flag = true;

                        if (this.Filter.SizeStart != null
                                || this.Filter.SizeEnd != null)
                        {
                                int? size = item.GetSizeKB();

                                if (size != null)
                                {
                                        if (this.Filter.SizeStart != null
                                               && this.Filter.SizeEnd != null)
                                        {
                                                flag = size >= this.Filter.SizeStart
                                                        && size <= this.Filter.SizeEnd;
                                        }
                                        else if (this.Filter.SizeStart != null)
                                        {
                                                flag = size >= this.Filter.SizeStart;
                                        }
                                        else if (this.Filter.SizeEnd != null)
                                        {
                                                flag = size <= this.Filter.SizeEnd;
                                        }
                                }
                        }


                        if (flag)
                        {
                                if (this.Filter.DateStart != null
                                        || this.Filter.DateEnd != null)
                                {
                                        DateTime? date = item.GetDate();
                                        if (date != null)
                                        {
                                                if (this.Filter.DateStart != null
                                                        && this.Filter.DateEnd != null)
                                                {
                                                        flag = date >= this.Filter.DateStart
                                                                && date <= this.Filter.DateEnd;
                                                }
                                                else if (this.Filter.DateStart != null)
                                                {
                                                        flag = date >= this.Filter.DateStart;
                                                }
                                                else if (this.Filter.DateEnd != null)
                                                {
                                                        flag = date <= this.Filter.DateEnd;
                                                }
                                                else
                                                {
                                                        flag = true;
                                                }
                                        }
                                }
                        }


                        if (flag)
                        {
                                switch (this.Filter.ItemType)
                                {
                                        case ItemType.All:
                                                return true;

                                        case ItemType.Magnet:
                                                return item is MagnetItem
                                                        || item is TitleItem;

                                        case ItemType.BaiduShare:
                                                return item is BaiduNetDiskItem
                                                        || item is TitleItem;

                                        case ItemType.Magnet_Baidu:
                                                return item is MagnetItem
                                                        || item is BaiduNetDiskItem
                                                        || item is TitleItem;

                                        case ItemType.Subtitle:
                                                return item is SubTitleItem
                                                        || item is TitleItem;
                                }
                        }

                        return flag;
                }

                private void ResultPage_TitleItemGenerated(TitleItem obj)
                {
                        if (this.TitleItem == null
                                && obj != null)
                        {
                                this.TitleItem = obj;
                        }
                }

                private void ResultPage_SearchFinished(object obj)
                {
                        this.IsFinished = this._webCrawlers.All(_p => _p.IsFinished);
                }

                private void ResultPage_SearchComplated(object obj)
                {
                        WebCrawler crawler = obj as WebCrawler;
                        if (crawler != null)
                        {
                                this.IsComplated = this._webCrawlers.All(_p => _p.IsComplated);
                        }
                }

                private void ResultPage_OnGenerateResultItem(ResultItem result)
                {
                        this.AddResultItem(result);
                }

                private readonly object _syncRoot = new object();

                public void AddResultItem(ResultItem item)
                {
                        if (item != null)
                        {
                                lock (this._syncRoot)
                                {
                                        if (this._items.Add(item)
                                                && this.FilterItem(item))
                                        {
                                                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                                                {
                                                        this.ResultItems.Add(item);

                                                }), DispatcherPriority.Loaded);
                                        }
                                }
                        }
                }

                public void Start()
                {
                        if (this.IsFinished)
                        {
                                return;
                        }

                        if (this.IsComplated)
                        {
                                this.SearchCount++;
                        }

                        this.IsComplated = false;

                        foreach (var item in this._webCrawlers)
                        {
                                //bool flag = false;
                                //switch (this.Filter.ItemType)
                                //{
                                //        case ItemType.All:
                                //                flag = true;
                                //                break;

                                //        case ItemType.Magnet:
                                //                flag = item.ItemType == ItemType.Magnet 
                                //                        || item.ItemType == ItemType.All 
                                //                        || item.ItemType == ItemType.Magnet_Baidu;
                                //                break;

                                //        case ItemType.BaiduShare:
                                //                flag = item.ItemType == ItemType.BaiduShare
                                //                        || item.ItemType == ItemType.All
                                //                        || item.ItemType == ItemType.Magnet_Baidu;
                                //                break;

                                //        case ItemType.Magnet_Baidu:
                                //                flag = item.ItemType == ItemType.Magnet
                                //                        || item.ItemType == ItemType.BaiduShare
                                //                        || item.ItemType == ItemType.All
                                //                        || item.ItemType == ItemType.Magnet_Baidu;
                                //                break;

                                //        case ItemType.Subtitle:
                                //                flag = item.ItemType == ItemType.Subtitle;
                                //                break;
                                //}

                                if (true)
                                {
                                        try
                                        {
                                                item.Start(this.SearchText);
                                        }
                                        catch (Exception e)
                                        {
                                                Debug.WriteLine(e);
                                        }
                                }
                        }
                }

                public void Stop()
                {
                        for (int i = 0; i < this._webCrawlers.Count; i++)
                        {
                                this._webCrawlers[i].Stop();
                        }
                }
        }
}
