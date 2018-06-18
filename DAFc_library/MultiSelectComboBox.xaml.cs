using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace DAFc_library
{
    /// <summary>
    /// Logica di interazione per MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl
    {
        private ObservableCollection<Node> _nodeList;

        public MultiSelectComboBox()
        {
            InitializeComponent();
            _nodeList = new ObservableCollection<Node>();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(Dictionary<string, object>), typeof(MultiSelectComboBox),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems",
           typeof(Dictionary<string, object>), typeof(MultiSelectComboBox), new UIPropertyMetadata(null, new PropertyChangedCallback(MultiSelectComboBox.OnSelectedItemsChanged)));

        public static readonly DependencyProperty IdRowProperty = DependencyProperty.Register("IdRow", typeof(int), typeof(MultiSelectComboBox),
            new UIPropertyMetadata(null));

        public static readonly DependencyProperty NameFieldIdRowProperty = DependencyProperty.Register("NameFieldIdRow", typeof(string), typeof(MultiSelectComboBox),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox),
           new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty NameFieldTextProperty = DependencyProperty.Register("NameFieldText", typeof(string), typeof(MultiSelectComboBox),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty DefaultTextProperty = DependencyProperty.Register("DefaultText", typeof(string),
            typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

        public Dictionary<string, object> ItemsSource
        {
            get { return (Dictionary<string, object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public Dictionary<string, object> SelectedItems
        {
            get { return (Dictionary<string, object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        //SelectedValuePath
        public int IdRow
        {
            get { return (int)GetValue(IdRowProperty); }
            set { SetValue(IdRowProperty, value); }
        }

        public string NameFieldIdRow
        {
            get { return (string)GetValue(NameFieldIdRowProperty); }
            set { SetValue(NameFieldIdRowProperty, value); }
        }

        //DisplayMemberPath
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public string NameFieldText
        {
            get { return (string)GetValue(NameFieldTextProperty); }
            set { SetValue(NameFieldTextProperty, value); }
        }

        public string DefaultText
        {
            get { return (string)GetValue(DefaultTextProperty); }
            set { SetValue(DefaultTextProperty, value); }
        }

        private void DisplayInControl()
        {
            _nodeList.Clear();
            if (this.ItemsSource.Count > 1)
                _nodeList.Add(new Node("All"));
            foreach (KeyValuePair<string, object> kV in this.ItemsSource)
            {
                Node node = new Node(kV.Key);
               _nodeList.Add(node);
            }
            MultiSelectCombo.ItemsSource = _nodeList;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            control.DisplayInControl();
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox clickedBox = (CheckBox)sender;
            if ((string)clickedBox.Content == "All")
            {
                if (clickedBox.IsChecked.Value)
                {
                    foreach (Node node in _nodeList)
                    {
                        node.IsSelected = true;
                    }
                }
                else
                {
                    foreach (Node node in _nodeList)
                    {
                        node.IsSelected = false;
                    }
                }
            }
            else
            {
                int _selectedCount = 0;
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected && s.Title != "All")
                        _selectedCount++;
                }
                if (_selectedCount == _nodeList.Count - 1)
                    _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = true;
                else
                    _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = false;
            }
            SetSelectedItems();
            SetText();
        }
        private void SetSelectedItems()
        {
            if (SelectedItems == null)
                SelectedItems = new Dictionary<string, object>();
            SelectedItems.Clear();
            foreach (Node node in _nodeList)
            {
                if (node.IsSelected && node.Title != "All")
                {
                    if (this.ItemsSource.Count > 0)
                        SelectedItems.Add(node.Title, this.ItemsSource[node.Title]);
                }
            }
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                foreach (Node s in _nodeList)
                {
                    if (s.IsSelected == true && s.Title == "All")
                    {
                        displayText = new StringBuilder();
                        displayText.Append("All");
                        break;
                    }
                    else if (s.IsSelected == true && s.Title != "All")
                    {
                        displayText.Append(s.Title);
                        displayText.Append(',');
                    }
                }
                this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            }
            // set DefaultText if nothing else selected
            if (string.IsNullOrEmpty(this.Text))
            {
                this.Text = this.DefaultText;
            }
        }

        private void SelectNodes()
        {
            foreach (KeyValuePair<string, object> kV in SelectedItems)
            {
                Node node = _nodeList.FirstOrDefault(i => i.Title == kV.Key);
                if (node != null)
                    node.IsSelected = true;
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MultiSelectComboBox control = (MultiSelectComboBox)d;
            control.SelectNodes();
            control.SetText();
        }
    }
}
