using System;
using System.Diagnostics;
using System.Windows;

namespace MagneticCrawler
{
    public class MagnetItem : ResultItem
    {
        /// <summary>
        /// 磁力链
        /// </summary>
        private string _magneticLink;
        public string MagneticLink
        {
            get
            {
                return _magneticLink;
            }
            set
            {
                this._magneticLink = value;
                this.RaisePropertyChanged(nameof(MagneticLink));
            }
        }


        /// <summary>
        /// 迅雷连接
        /// </summary>
        private string _thunderLink;
        public string ThunderLink
        {
            get
            {
                return _thunderLink;
            }
            set
            {
                this._thunderLink = value;
                this.RaisePropertyChanged(nameof(ThunderLink));
            }
        }

        public void StartMagneticLink()
        {
            try
            {
                Clipboard.SetText(this.MagneticLink);
                Process.Start(this.MagneticLink);
            }
            catch (Exception)
            {
            }
        }

        public void StartThunderLink()
        {
            try
            {
                Clipboard.SetText(this.ThunderLink);
                Process.Start(this.ThunderLink);
            }
            catch (Exception)
            {
            }
        }

        public override bool Equals(object obj)
        {
            MagnetItem item = obj as MagnetItem;
            if (item != null)
            {
                if (!string.IsNullOrEmpty(this.MagneticLink)
                        && !string.IsNullOrWhiteSpace(this.MagneticLink))
                {
                    return string.Equals(this.MagneticLink, item.MagneticLink);
                }
                else if (!string.IsNullOrEmpty(this.ThunderLink)
                         && !string.IsNullOrWhiteSpace(this.ThunderLink))
                {
                    return string.Equals(this.ThunderLink, item.ThunderLink);
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
            if (!string.IsNullOrEmpty(this.MagneticLink)
                    && !string.IsNullOrWhiteSpace(this.MagneticLink))
            {
                return this.MagneticLink.GetHashCode();
            }
            else if (!string.IsNullOrEmpty(this.ThunderLink)
                    && !string.IsNullOrWhiteSpace(this.ThunderLink))
            {
                return this.ThunderLink.GetHashCode();
            }

            return base.GetHashCode();
        }
    }
}
