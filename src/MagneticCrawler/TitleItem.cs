using MagneticCrawler.Crawlers;

namespace MagneticCrawler
{
        public class TitleItem : ResultItem
        {
                private CiliMaoCrawler.Movie _movieTitle;
                public CiliMaoCrawler.Movie MovieTitle
                {
                        get
                        {
                                return _movieTitle;
                        }
                        set
                        {
                                this._movieTitle = value;
                                this.RaisePropertyChanged(nameof(MovieTitle));
                        }
                }
        }
}
