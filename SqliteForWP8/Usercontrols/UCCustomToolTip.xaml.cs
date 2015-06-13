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
    public partial class UCCustomToolTip : UserControl
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

        public UCCustomToolTip()
        {
            InitializeComponent();
            Loaded += UCCustomToolTip_Loaded;
        }

        void UCCustomToolTip_Loaded(object sender, RoutedEventArgs e)
        {
            Lbltext.Text = Description;
        }
        public void FillDescription()
        {
            Lbltext.Text = Description;
        }

        private void imgmarker_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {

            if (imgpath.Opacity == 0)
            {
                imgpath.Opacity = 1;
                imgpath.Visibility = System.Windows.Visibility.Collapsed;
                imgborderg.Opacity = 1;
                imgborderg.Visibility = Visibility;
            }
            else
            {
                imgpath.Opacity = 0;
                imgpath.Visibility = System.Windows.Visibility.Collapsed;
                imgborderg.Opacity = 0;
                imgborderg.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
