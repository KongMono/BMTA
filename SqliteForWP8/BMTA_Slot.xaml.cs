using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ShakeGestures;
using System.IO;
using System.Windows.Threading;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Xna.Framework;

namespace BMTA
{
    public partial class BMTA_Slot : PhoneApplicationPage
    {
        public String lang = (Application.Current as App).Language;
        public BMTA_Slot()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 6;
            ShakeGesturesHelper.Instance.Active = true;
            InitializeComponent();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (lang.Equals("th"))
            {
                titleName.Text = "กิจกรรม";
                headslot.Text = "ตั๋วทำนายดวง";
                headslotdetail.Text = "บวกเลขทั้งหมด ลงท้ายด้วยเลขอะไร";
                headslotshake.Text = "กรุณาเขย่าเพื่อทำนายดวง";
            }
            else
            {
                titleName.Text = "Events";
                headslot.Text = "Ticket Fortune";
                headslotdetail.Text = "What is the last number of your ticket no.summation?";
                headslotshake.Text = "Please input your ticket number!";
            }

        }

        // Set the data context of the TextBlock to the answer.
        void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            // Use BeginInvoke to write to the UI thread.
            txtnumber.Dispatcher.BeginInvoke(() =>
            {
                if (txtnumber.Text.Length == 6 || txtnumber.Text.Length == 7)
                {
                    if (lang.Equals("th"))
                    {
                        MessageBox.Show("กรุณาใส่จำนวนตัวเลขให้ครบ 6-7 หลัก");
                    }
                    else
                    {
                        MessageBox.Show("Please check your ticket number!");
                    }

                    txtnumber.Focus();
                    return;
                }
                else
                {
                    int len = txtnumber.Text.Length;
                    int sum = 0;
                    string pasteText = txtnumber.Text;
                    for (int i = 0; i < pasteText.Length; i++)
                    {
                        if (char.IsDigit(pasteText[i]))
                            sum += Convert.ToInt32(pasteText[i].ToString());
                    }
                    string spintext = sum.ToString();
                    string output = spintext.Substring(spintext.Length - 1, 1);
                    NavigationService.Navigate(new Uri("/BMTA_Slot_Result.xaml?parameter=" + output, UriKind.Relative));
                }
            });

        }

        private void txtnumber_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}