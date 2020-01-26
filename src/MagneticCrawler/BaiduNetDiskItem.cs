using System;
using System.Diagnostics;

namespace MagneticCrawler
{
        public class BaiduNetDiskItem : ResultItem
        {
                private string _author;
                /// <summary>
                /// 作者
                /// </summary>
                public string Author
                {
                        get
                        {
                                return _author;
                        }
                        set
                        {
                                this._author = value;
                                this.RaisePropertyChanged(nameof(Author));
                        }
                }

                private string _extractionCode;
                /// <summary>
                /// 提取码
                /// </summary>
                public string ExtractionCode
                {
                        get
                        {
                                return _extractionCode;
                        }
                        set
                        {
                                this._extractionCode = value;
                                this.RaisePropertyChanged(nameof(ExtractionCode));
                        }
                }


                private string _activeTimes;
                /// <summary>
                /// 浏览次数
                /// </summary>
                public string ActiveTimes
                {
                        get
                        {
                                return _activeTimes;
                        }
                        set
                        {
                                this._activeTimes = value;
                                this.RaisePropertyChanged(nameof(ActiveTimes));
                        }
                }



                private string _downloadTimes;
                /// <summary>
                /// 下载次数
                /// </summary>
                public string DownloadTimes
                {
                        get
                        {
                                return _downloadTimes;
                        }
                        set
                        {
                                this._downloadTimes = value;
                                this.RaisePropertyChanged(nameof(DownloadTimes));
                        }
                }



                /// <summary>
                /// 百度网盘分享链接
                /// </summary>
                private string _baiduNetDiskLink;
                public string BaiduNetDiskLink
                {
                        get
                        {
                                return _baiduNetDiskLink;
                        }
                        set
                        {
                                this._baiduNetDiskLink = value;
                                this.RaisePropertyChanged(nameof(BaiduNetDiskLink));
                        }
                }

                public void StartBaiduLink()
                {
                        try
                        {
                                Process.Start(this.BaiduNetDiskLink);
                        }
                        catch (Exception)
                        {
                        }
                }

                public override bool Equals(object obj)
                {
                        BaiduNetDiskItem item = obj as BaiduNetDiskItem;
                        if (item != null)
                        {
                                if (!string.IsNullOrEmpty(this.BaiduNetDiskLink)
                                        && !string.IsNullOrWhiteSpace(this.BaiduNetDiskLink))
                                {
                                        return string.Equals(this.BaiduNetDiskLink, item.BaiduNetDiskLink);
                                }
                                else
                                {
                                        return object.ReferenceEquals(this, item);
                                }
                        }

                        return false;
                }

                public override int GetHashCode()
                {
                        if (!string.IsNullOrEmpty(this.BaiduNetDiskLink)
                                && !string.IsNullOrWhiteSpace(this.BaiduNetDiskLink))
                        {
                                return this.BaiduNetDiskLink.GetHashCode();
                        }

                        return base.GetHashCode();
                }
        }
}
