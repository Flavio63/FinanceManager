using System.ComponentModel;

namespace DAFc_library
{
    public class Node : INotifyPropertyChanged
    {
        private string _Title;
        private bool _isSelected;
        
        public Node(string title)
        {
            Title = title;
        }

        public string Title
        {
            get { return _Title; }
            set { _Title = value; NotifyPropertyChanged("Title"); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; NotifyPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
