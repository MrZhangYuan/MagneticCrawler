using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MagneticCrawler.Crawlers;
using System.Windows;
using System.IO;
using System.Xml.Serialization;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.ComponentModel.Composition.Hosting;

namespace MagneticCrawler
{

    public class MainViewModel : ObservableObject
    {
        public static MainViewModel Instance
        {
            get;
        }

        static MainViewModel()
        {
            Instance = new MainViewModel();
        }
        private MainViewModel()
        {
            this.SearchHistorys = new ObservableCollection<SearchSuggestionItem>();
            this.SearchSuggestions = new ObservableCollection<SearchSuggestionItem>();
            Task.Factory.StartNew(this.LoadHistory);
            ViewHelper.ViewAllClosed += ViewHelper_ViewAllClosed;
        }

        private void ViewHelper_ViewAllClosed(object obj)
        {
            this.SearchPageVisibility = Visibility.Visible;
            this.MainPageVisibility = Visibility.Hidden;
        }

        private Visibility _searchPageVisibility = Visibility.Visible;
        public Visibility SearchPageVisibility
        {
            get
            {
                return _searchPageVisibility;
            }
            set
            {
                this._searchPageVisibility = value;
                this.RaisePropertyChanged(nameof(SearchPageVisibility));
            }
        }

        private Visibility _mainPageVisibility = Visibility.Hidden;
        public Visibility MainPageVisibility
        {
            get
            {
                return _mainPageVisibility;
            }
            set
            {
                this._mainPageVisibility = value;
                this.RaisePropertyChanged(nameof(MainPageVisibility));
            }
        }

        private bool _isDropDownOpen;
        public bool IsDropDownOpen
        {
            get
            {
                return _isDropDownOpen;
            }
            set
            {
                this._isDropDownOpen = value;
                this.RaisePropertyChanged(nameof(IsDropDownOpen));
            }
        }

        private ObservableCollection<SearchSuggestionItem> SearchHistorys
        {
            get;
        }
        public ObservableCollection<SearchSuggestionItem> SearchSuggestions
        {
            get;
        }

        public IEnumerable<WebCrawler> NewWebCrawlers()
        {
            //var crawlers = new CompositionContainer(
            //                                    new AggregateCatalog(
            //                                            new AssemblyCatalog(typeof(WebCrawler).Assembly))
            //                                        )
            //                                        .GetExportedValues<WebCrawler>(
            //                                            WebCrawler.CrawlerContractString
            //                                        )
            //                                        .Where(
            //                                            _p => _p.GetType().GetCustomAttribute(typeof(ObsoleteAttribute)) == null
            //                                        );

            //return crawlers;

            //yield return new DefaultCrawler();

            ////磁力链&百度云
            //yield return new CiliMaoCrawler();

            ////磁力链
            //yield return new BtRabbitCrawler();
            //yield return new CLBCrawler();
            //yield return new BtUrlCrawler();
            //yield return new BteatCrawler();
            //yield return new CiliGouCrawler();
            //yield return new CiliFanHaoCrawler();
            //yield return new SoSoBtaCrawler();
            //yield return new BtSoSoCrawler();
            //yield return new Fox_SpCrawler();

            ////百度云
            //yield return new PanSoSoCrawler();
            //////yield return new PanSoCrawler();



            yield return new SoSoCiliCrawler();
            yield return new SkrBTCrawler();
            yield return new CiliwangCrawler();
            yield return new CiLiGuoCrawler();
            yield return new CiLiNingMengCrawler();

            //无明细
            yield return new MAGCrawler();
            yield return new ZhongZiSoCrawler();
        }

        public async void EditTextChanged(string text)
        {
            this.SearchSuggestions.Clear();

            if (string.IsNullOrEmpty(text))
            {
                foreach (var item in this.SearchHistorys)
                {
                    this.SearchSuggestions.Add(item);
                }
            }
            else
            {
                try
                {
                    var result = await Task.Factory.StartNew(() =>
                    {
                        return WebRequestHelper.Request(new RequestParameter
                        {
                            Encoding = Encoding.GetEncoding("GBK"),
                            Url = $"http://suggestion.baidu.com/su?wd={text}"
                            //Url = $"http://nssug.baidu.com/su?wd={text}&prod=top"
                        });
                    });

                    if (!string.IsNullOrEmpty(result))
                    {
                        result = result.RemoveStart("window.baidu.sug(").RemoveEnd(");");
                    }

                    var list = JsonConvert.DeserializeAnonymousType(result, new
                    {
                        q = "",
                        p = false,
                        s = new string[0]
                    });

                    if (list != null)
                    {
                        this.SearchSuggestions.Add(new SearchSuggestionItem
                        {
                            Order = 0,
                            Text = list.q
                        });

                        if (list.s != null)
                        {
                            for (int i = 0; i < list.s.Length; i++)
                            {
                                this.SearchSuggestions.Add(new SearchSuggestionItem
                                {
                                    Order = i + 1,
                                    Text = list.s[i]
                                });
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }

            this.IsDropDownOpen = true;
        }

        public void Search(string paramstr)
        {
            if (!string.IsNullOrEmpty(paramstr)
                    && !string.IsNullOrWhiteSpace(paramstr))
            {
                SearchSuggestionItem history = this.SearchHistorys.FirstOrDefault(_p => _p.Text == paramstr);
                if (history == null)
                {
                    history = new SearchSuggestionItem(paramstr)
                    {
                        Order = this.SearchHistorys.Count == 0 ? 1 : this.SearchHistorys.Max(_p => _p.Order),
                        IsHistory = true
                    };
                    this.SearchHistorys.Insert(0, history);
                }

                this.Search(history);
            }
        }

        public void Search(SearchSuggestionItem history)
        {
            if (!this.SearchHistorys.Contains(history))
            {
                this.Search(history.Text);
                return;
            }

            history.Order = this.SearchHistorys.Max(_p => _p.Order) + 1;

            ResultPage newpage = new ResultPage(1, history.Text, this.NewWebCrawlers());
            ViewHelper.ShowPage(newpage);
            newpage.Start();
            this.SearchPageVisibility = Visibility.Hidden;
            this.MainPageVisibility = Visibility.Visible;

            Task.Factory.StartNew(this.SaveHostory);
        }

        string _historyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "History.xml");
        private readonly object _lockobj = new object();
        public void SaveHostory()
        {
            var list = this.SearchHistorys.ToList();
            int min = list.Count == 0 ? 0 : list.Min(_p => _p.Order);
            foreach (var item in list)
            {
                item.Order = item.Order - min;
            }

            try
            {
                lock (this._lockobj)
                {
                    using (FileStream fileStream = new FileStream(_historyPath, FileMode.Create))
                    {
                        XmlSerializer xml = new XmlSerializer(list.GetType());
                        xml.Serialize(fileStream, list);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void LoadHistory()
        {
            try
            {
                this.SearchSuggestions.Clear();
                if (File.Exists(_historyPath))
                {
                    lock (this._lockobj)
                    {
                        using (FileStream fileStream = new FileStream(_historyPath, FileMode.Open))
                        {
                            XmlSerializer xml = new XmlSerializer(typeof(List<SearchSuggestionItem>));
                            List<SearchSuggestionItem> histories = xml.Deserialize(fileStream) as List<SearchSuggestionItem>;
                            if (histories != null && histories.Count > 0)
                            {
                                foreach (var item in histories.OrderByDescending(_p => _p.Order))
                                {
                                    item.IsHistory = true;
                                    this.SearchHistorys.Add(item);
                                    this.SearchSuggestions.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void DeleteHistory(SearchSuggestionItem history, bool ischear = false)
        {
            if (ischear)
            {
                this.SearchHistorys.Clear();
                this.SearchSuggestions.Clear();
            }
            else if (history != null)
            {
                this.SearchHistorys.Remove(history);
                this.SearchSuggestions.Remove(history);
            }

            this.SaveHostory();
        }
    }
}
