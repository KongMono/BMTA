using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BMTA.Usercontrols
{
    public partial class UCCustomToolTipDetail : UserControl
    {
        private string _description;
        private string _busline;
        private string _imgPath;

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

        public string ImagePath
        {
            get { return _imgPath; }
            set { _imgPath = value; }
        }

        public UCCustomToolTipDetail()
        {
            InitializeComponent();
            Loaded += UCCustomToolTipDetail_Loaded;
        }

        void UCCustomToolTipDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Lbltext.Text = Description;
            if (_imgPath != null)
            {
                BitmapImage licoriceImage = new BitmapImage(new Uri("/Assets/" + ImagePath + ".png", UriKind.Relative));
                imgmarker.Source = licoriceImage;
            }
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
