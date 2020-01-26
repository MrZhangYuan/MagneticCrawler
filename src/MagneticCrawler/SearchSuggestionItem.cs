using System;

namespace MagneticCrawler
{
        [Serializable]
        public class SearchSuggestionItem
        {
                public bool IsHistory
                {
                        get;
                        set;
                }
                public string Text
                {
                        get;
                        set;
                }
                public int Order
                {
                        get;
                        set;
                }
                public SearchSuggestionItem()
                {

                }
                public SearchSuggestionItem(string text)
                {
                        Text = text;
                }

                public override string ToString()
                {
                        return this.Text;
                }
        }
}
