﻿using Clipboards.Styles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Clipboards.Class
{
    public class MainVM : ValidationBase
    {
        MainWindow win;
        public static MainVM Instance;
        public static Dictionary<ClipboardItem,ListBoxItemEx> DataTemplateWithCtrl = new Dictionary<ClipboardItem,ListBoxItemEx>();
        public MainVM(MainWindow window)
        {
            win = window;
            ClipboardsItems = new ObservableCollection<ClipboardItem>();
            Instance = this;
        }

        private ObservableCollection<ClipboardItem> clipboardsItems;
        public ObservableCollection<ClipboardItem> ClipboardsItems
        {
            get { return clipboardsItems; }
            set { clipboardsItems = value; NotifyPropertyChanged(nameof(ClipboardsItems)); }
        }
    }
}
