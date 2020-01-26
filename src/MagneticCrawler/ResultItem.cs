using MagneticCrawler.Crawlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace MagneticCrawler
{
        public class FileItem
        {
                public int Index
                {
                        get;
                        set;
                }
                public string FileName
                {
                        get;
                        set;
                }
                public string Size
                {
                        get;
                        set;
                }
        }

        public abstract class ResultItem : ObservableObject
        {
                public Guid Guid
                {
                        get;
                }

                public string WebIconUrl
                {
                        get;
                        set;
                }

                /// <summary>
                /// 详情URL
                /// </summary>
                private string _detailUrl;
                public string DetailUrl
                {
                        get
                        {
                                return _detailUrl;
                        }
                        set
                        {
                                this._detailUrl = value;
                                this.RaisePropertyChanged(nameof(DetailUrl));
                        }
                }

                private WebCrawler _ownerWebCrawler;
                public WebCrawler OwnerWebCrawler
                {
                        get
                        {
                                return _ownerWebCrawler;
                        }
                        set
                        {
                                this._ownerWebCrawler = value;
                                this.RaisePropertyChanged(nameof(OwnerWebCrawler));
                        }
                }


                /// <summary>
                /// 标题
                /// </summary>
                private string _title;
                public string Title
                {
                        get
                        {
                                return _title;
                        }
                        set
                        {
                                this._title = value;
                                this.RaisePropertyChanged(nameof(Title));
                        }
                }

                /// <summary>
                /// 文件类型
                /// </summary>
                private string _resourceType;
                public string ResourceType
                {
                        get
                        {
                                return _resourceType;
                        }
                        set
                        {
                                this._resourceType = value;
                                this.RaisePropertyChanged(nameof(ResourceType));
                        }
                }


                /// <summary>
                /// 创建时间
                /// </summary>
                private string _createTime;
                public string CreateTime
                {
                        get
                        {
                                return _createTime;
                        }
                        set
                        {
                                this._createTime = value;
                                this.RaisePropertyChanged(nameof(CreateTime));
                        }
                }

                /// <summary>
                /// 更新时间
                /// </summary>
                private string _updateTime;
                public string UpdateTime
                {
                        get
                        {
                                return _updateTime;
                        }
                        set
                        {
                                this._updateTime = value;
                                this.RaisePropertyChanged(nameof(UpdateTime));
                        }
                }



                /// <summary>
                /// 最后活跃
                /// </summary>
                private string _activeTime;
                public string ActiveTime
                {
                        get
                        {
                                return _activeTime;
                        }
                        set
                        {
                                this._activeTime = value;
                                this.RaisePropertyChanged(nameof(ActiveTime));
                        }
                }


                /// <summary>
                /// 大小
                /// </summary>
                private string _size;
                public string Size
                {
                        get
                        {
                                return _size;
                        }
                        set
                        {
                                this._size = value;
                                this.RaisePropertyChanged(nameof(Size));
                        }
                }



                /// <summary>
                /// 热度
                /// </summary>
                private string _hotLevel;
                public string HotLevel
                {
                        get
                        {
                                return _hotLevel;
                        }
                        set
                        {
                                this._hotLevel = value;
                                this.RaisePropertyChanged(nameof(HotLevel));
                        }
                }

                /// <summary>
                /// 下载速度
                /// </summary>
                private string _downloadSpeed;
                public string DownloadSpeed
                {
                        get
                        {
                                return _downloadSpeed;
                        }
                        set
                        {
                                this._downloadSpeed = value;
                                this.RaisePropertyChanged(nameof(DownloadSpeed));
                        }
                }

                /// <summary>
                /// 文件列表
                /// </summary>
                public ObservableCollection<FileItem> FileList
                {
                        get;
                }


                private bool _isDetailLoaded = true;
                public bool IsDetailLoaded
                {
                        get
                        {
                                return _isDetailLoaded;
                        }
                        set
                        {
                                this._isDetailLoaded = value;
                                this.RaisePropertyChanged(nameof(IsDetailLoaded));
                        }
                }

                public ResultItem()
                {
                        this.Guid = Guid.NewGuid();
                        this.FileList = new ObservableCollection<FileItem>();
                        this.ResourceType = "未知";
                }

                public void AddFileItem(FileItem fileItem)
                {
                        if (fileItem != null)
                        {
                                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                        this.FileList.Add(fileItem);
                                }));
                        }
                }


                private bool _isDetailLoadingFlag = false;
                public void LoadDetail()
                {
                        if (!string.IsNullOrEmpty(this.DetailUrl))
                        {
                                if (!this._isDetailLoadingFlag)
                                {
                                        this._isDetailLoadingFlag = true;
                                        this.IsDetailLoaded = false;
                                        this.OwnerWebCrawler?.RequestDetail(this);
                                }
                        }
                }

                public int? GetSizeKB()
                {
                        if (!string.IsNullOrEmpty(this.Size))
                        {
                                string size = this.Size.ToUpper();
                                int? value = null;

                                if (size.Contains("GB")
                                        || size.Contains("G"))
                                {
                                        if (double.TryParse(size.Replace("GB", "").Replace("G", "").Trim(), out double temp))
                                        {
                                                value = (int)(temp * 1024 * 1024);
                                                return value;
                                        }
                                }
                                else if (size.Contains("MB")
                                        || size.Contains("M"))
                                {
                                        if (double.TryParse(size.Replace("MB", "").Replace("M", "").Trim(), out double temp))
                                        {
                                                value = (int)(temp * 1024);
                                                return value;
                                        }
                                }
                                else if (size.Contains("KB")
                                        || size.Contains("K"))
                                {
                                        if (double.TryParse(size.Replace("KB", "").Replace("K", "").Trim(), out double temp))
                                        {
                                                value = (int)temp;
                                                return value;
                                        }
                                }
                        }
                        return null;
                }

                public DateTime? GetDate()
                {
                        if (!string.IsNullOrEmpty(this.CreateTime)
                                && DateTime.TryParse(this.CreateTime, out DateTime date))
                        {
                                return date;
                        }

                        if (this.CreateTime.Contains("年前")
                                || this.CreateTime.Contains("年"))
                        {
                                if (int.TryParse(this.CreateTime.Replace("年前", "").Replace("年", ""), out int value))
                                {
                                        return DateTime.Now.Date.AddYears(-value);
                                }
                        }
                        else if (this.CreateTime.Contains("个月前")
                                || this.CreateTime.Contains("月前")
                                || this.CreateTime.Contains("月"))
                        {
                                if (int.TryParse(this.CreateTime.Replace("个月前", "").Replace("月前", "").Replace("月", ""), out int value))
                                {
                                        return DateTime.Now.Date.AddMonths(-value);
                                }
                        }
                        else if (this.CreateTime.Contains("周前")
                                || this.CreateTime.Contains("星期前")
                                || this.CreateTime.Contains("周")
                                || this.CreateTime.Contains("星期")
                                || this.CreateTime.Contains("礼拜前")
                                || this.CreateTime.Contains("礼拜"))
                        {
                                if (int.TryParse(this.CreateTime.Replace("周前", "").Replace("星期前", "").Replace("礼拜前", "").Replace("星期", "").Replace("礼拜", "").Replace("周", ""), out int value))
                                {
                                        return DateTime.Now.Date.AddDays(-value * 7);
                                }
                        }
                        else if (this.CreateTime.Contains("天前")
                                || this.CreateTime.Contains("日前")
                                || this.CreateTime.Contains("天")
                                || this.CreateTime.Contains("日"))
                        {
                                if (int.TryParse(this.CreateTime.Replace("天前", "").Replace("日前", "").Replace("日", "").Replace("日", ""), out int value))
                                {
                                        return DateTime.Now.Date.AddDays(-value);
                                }
                        }
                        else if (this.CreateTime.Contains("个小时前")
                                || this.CreateTime.Contains("小时前")
                                || this.CreateTime.Contains("小时"))
                        {
                                return DateTime.Now.Date;
                        }

                        return null;
                }
        }
}
