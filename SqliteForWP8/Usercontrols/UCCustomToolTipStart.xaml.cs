using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BMTA.Usercontrols
{
    public partial class UCCustomToolTipStart : UserControl
    {
        private string _description;
        private string _busline;

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string Busline
        {
            get { return _busline; }
            set { _busline = value; }
        }

        public UCCustomToolTipStart()
        {
            InitializeComponent();
            Loaded += UCCustomToolTipDetail_Loaded;
        }

        void UCCustomToolTipDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Lbltext.Text = Description;
        }
        public void FillDescription()
        {
            Lbltext.Text = Description;
        }
    }
}
