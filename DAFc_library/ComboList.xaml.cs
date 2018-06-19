using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DAFc_library
{
    /// <summary>
    /// Logica di interazione per ComboList.xaml
    /// </summary>
    public partial class ComboList : UserControl
    {
        //SelectedValuePath

        //DisplayMemberPath


        public ComboList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<Object>), typeof(MultiSelectComboBox),
            new FrameworkPropertyMetadata(null, new PropertyChangedCallback(ComboList.OnItemsSourceChanged)));

        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems",
           typeof(ObservableCollection<Object>), typeof(MultiSelectComboBox), new UIPropertyMetadata(null, new PropertyChangedCallback(ComboList.OnSelectedItemsChanged)));

        //public static readonly DependencyProperty SelectedValuePathProperty = DependencyProperty.Register("SelectedValuePath", typeof())
        public ObservableCollection<Object> ItemsSource
        {
            get { return (ObservableCollection<Object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public ObservableCollection<Object> SelectedItems
        {
            get { return (ObservableCollection<Object>)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }


        private void SelectionIsChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox LB = (ListBox)sender;
            foreach (var items in LB.SelectedItems)
            {
                
            }

            //if ((string)clickedBox.Content == "All")
            //{
            //    if (clickedBox.IsChecked.Value)
            //    {
            //        foreach (Node node in _nodeList)
            //        {
            //            node.IsSelected = true;
            //        }
            //    }
            //    else
            //    {
            //        foreach (Node node in _nodeList)
            //        {
            //            node.IsSelected = false;
            //        }
            //    }
            //}
            //else
            //{
            //    int _selectedCount = 0;
            //    foreach (Node s in _nodeList)
            //    {
            //        if (s.IsSelected && s.Title != "All")
            //            _selectedCount++;
            //    }
            //    if (_selectedCount == _nodeList.Count - 1)
            //        _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = true;
            //    else
            //        _nodeList.FirstOrDefault(i => i.Title == "All").IsSelected = false;
            //}
            SetSelectedItems();
            SetText();
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboList control = (ComboList)d;
            //control.DisplayInControl();
        }

        private void SetSelectedItems()
        {
            //if (SelectedItems == null)
            //    SelectedItems = new Dictionary<string, object>();
            //SelectedItems.Clear();
            //foreach (Node node in _nodeList)
            //{
            //    if (node.IsSelected && node.Title != "All")
            //    {
            //        if (this.ItemsSource.Count > 0)
            //            SelectedItems.Add(node.Title, this.ItemsSource[node.Title]);
            //    }
            //}
        }

        private void SetText()
        {
            if (this.SelectedItems != null)
            {
                StringBuilder displayText = new StringBuilder();
                //foreach (Node s in _nodeList)
                //{
                //    if (s.IsSelected == true && s.Title == "All")
                //    {
                //        displayText = new StringBuilder();
                //        displayText.Append("All");
                //        break;
                //    }
                //    else if (s.IsSelected == true && s.Title != "All")
                //    {
                //        displayText.Append(s.Title);
                //        displayText.Append(',');
                //    }
                //}
                //this.Text = displayText.ToString().TrimEnd(new char[] { ',' });
            }
            // set DefaultText if nothing else selected
            //if (string.IsNullOrEmpty(this.Text))
            //{
            //    this.Text = this.DefaultText;
            //}
        }

        private void SelectNodes()
        {
            //foreach (KeyValuePair<string, object> kV in SelectedItems)
            //{
            //    Node node = _nodeList.FirstOrDefault(i => i.Title == kV.Key);
            //    if (node != null)
            //        node.IsSelected = true;
            //}
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ComboList control = (ComboList)d;
            control.SelectNodes();
            control.SetText();
        }
    }
}
