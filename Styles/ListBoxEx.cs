using Clipboards.Class;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Clipboards.Styles
{
    public class ListBoxEx : ListBox
    {
        public event Action UnHook;
        public event Action Hook;
        protected override DependencyObject GetContainerForItemOverride()
        {
            var listbox = new ListBoxItemEx();
            listbox.UnHook += () => UnHook?.Invoke();
            listbox.Hook += () => Hook?.Invoke();
            return listbox;
            // return new ListBoxItemEx();
        }
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (e.Action == NotifyCollectionChangedAction.Reset)
                MainVM.DataTemplateWithCtrl.Clear();
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                MainVM.DataTemplateWithCtrl.Remove(e.OldItems[0] as ClipboardItem);
                int i = 1;
                foreach (KeyValuePair<ClipboardItem, ListBoxItemEx> item in MainVM.DataTemplateWithCtrl.OrderBy(kv => kv.Key.Index))
                {
                    item.Key.Index = i;
                    item.Value.TxbIndex.Text = $"{i}";
                    i++;
                }
            }
        }
    }
    

    public class ListBoxItemEx : ListBoxItem
    {
        public event Action UnHook;
        public event Action Hook;
        protected override void OnSelected(System.Windows.RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListBoxItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            ListBoxItem item = (ListBoxItem)dep;

            if (item.IsSelected)
            {
                item.IsSelected = !item.IsSelected;
                //e.Handled = true;
            }
            base.OnSelected(e);
        }

        private TextBlock textbox;
        public TextBlock TxbIndex;
        public Button button;
        private Image image;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            textbox = Template.FindName("tb", this) as TextBlock;
            textbox.MouseLeftButtonDown -= Textbox_MouseLeftButtonDown;
            textbox.MouseLeftButtonDown += Textbox_MouseLeftButtonDown;
            
            button = Template.FindName("closeBtn", this) as Button;
            button.Click -= Button_Click;
            button.Click += Button_Click;
            
            image = Template.FindName("img", this) as Image;
            image.MouseLeftButtonDown -= Textbox_MouseLeftButtonDown;
            image.MouseLeftButtonDown += Textbox_MouseLeftButtonDown;
            TxbIndex = Template.FindName("lblIndex",this) as TextBlock;
            var item = MainVM.Instance.ClipboardsItems.FirstOrDefault(c=>c.HashCode== GetSourceHashCode());
            if (!MainVM.DataTemplateWithCtrl.ContainsKey(item))
            {
                MainVM.DataTemplateWithCtrl.Add(item,this);
            }
            MainVM.DataTemplateWithCtrl[item] = this;

        }

        private int GetSourceHashCode()
        {
            int hashCode;
            if (string.IsNullOrEmpty(textbox.Text))
            {
                hashCode = image.Source.GetHashCode();
            }
            else
            {
                hashCode = textbox.Text.GetHashCode();
            }
            return hashCode;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int hashCode;            
            if (string.IsNullOrEmpty(textbox.Text))
            {
                hashCode = textbox.Text.GetHashCode();
                var item = MainVM.Instance.ClipboardsItems.FirstOrDefault(c => c.HashCode == hashCode);
                MainVM.Instance.ClipboardsItems.Remove(item);
                Trace.WriteLine("1");
            }
            else
            {
                /*var str = image.Source + "";
                var arr = str.Split("/");
                var arr1 = arr[arr.Length - 1];
                var str1 = arr1.Split(".");
                var path = System.Environment.CurrentDirectory + "\\cache\\" + arr1;
                */
                hashCode = image.Source.GetHashCode();// path.GetHashCode();//
                var item = MainVM.Instance.ClipboardsItems.FirstOrDefault(c => c.HashCode == hashCode);
                MainVM.Instance.ClipboardsItems.Remove(item);
                Trace.WriteLine("2");

            }
        }

        private void Textbox_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                HotKeyHelper.IsIgnorance = true;
                UnHook?.Invoke();
                if (!string.IsNullOrEmpty(textbox.Text))
                {
                    Clipboard.SetText(textbox.Text);
                }
                else
                {
                    Clipboard.SetImage((BitmapSource)image.Source);
                }
                Hook?.Invoke();
            }
            catch(Exception ex)
            {
                LogHelper.Instance._logger.Error(ex);
            }
        }


    }
}
