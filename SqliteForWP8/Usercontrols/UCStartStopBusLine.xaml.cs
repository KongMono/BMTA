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
    public partial class UCStartStopBusLine : UserControl
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

        public UCStartStopBusLine()
        {
            InitializeComponent();
        }
    }
}
