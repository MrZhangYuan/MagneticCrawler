using System.ComponentModel;

namespace MagneticCrawler
{
        public class ObservableObject : INotifyPropertyChanged
        {
                public event PropertyChangedEventHandler PropertyChanged;
                public void RaisePropertyChanged(string propertyname)
                {
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
                }
        }
}
