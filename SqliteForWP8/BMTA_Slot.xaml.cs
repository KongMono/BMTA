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
//using System.Windows.Markup.XamlParseException;
using Microsoft.Xna.Framework;

namespace BMTA
{
    public partial class BMTA_Slot : PhoneApplicationPage
    {
        public BMTA_Slot()
        {
            ShakeGesturesHelper.Instance.ShakeGesture += new EventHandler<ShakeGestureEventArgs>(Instance_ShakeGesture);
            ShakeGesturesHelper.Instance.MinimumRequiredMovesForShake = 6;
            ShakeGesturesHelper.Instance.Active = true;
            InitializeComponent();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {

            if (!HasNetwork())
            {

                Application.Current.Terminate();
            }
            else if (!HasInternet())
            {
                Application.Current.Terminate();
            }
            else
            {
                string x = BMTA.clGetResolution.Width.ToString();
                string y = BMTA.clGetResolution.Height.ToString();
                string xy = x + "x" + y;
                if (x == "480")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/480x852/BMTA_main_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else if (x == "720")
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/720x1280/BMTA_slot_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }
                else
                {
                    ImageBrush brush = new ImageBrush
                    {
                        ImageSource = new System.Windows.Media.Imaging.BitmapImage(new Uri("/Assets/768x1280/BMTA_main_bg.png", UriKind.Relative)),
                        Opacity = 1d
                    };
                    this.LayoutRoot.Background = brush;
                    brush.Stretch = Stretch.Fill;
                }

                txtnumber.Text = "";
                txtnumber.Focus();
            }
        }

        // Set the data context of the TextBlock to the answer.
        void Instance_ShakeGesture(object sender, ShakeGestureEventArgs e)
        {
            // Use BeginInvoke to write to the UI thread.
            txtnumber.Dispatcher.BeginInvoke(() =>
            {
                if (txtnumber.Text.Length < 7)
                {
                    MessageBox.Show("กรุณาใส่จำนวนตัวเลขให้ครบ 7หลัก");
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
                    int result = 0;
                    result = Convert.ToInt32(spintext[1].ToString());
                    NavigationService.Navigate(new Uri("/BMTA_Slot_Result.xaml?parameter=" + result, UriKind.Relative));
                }
            });

        }

        void checkInput(int length)
        {
            if (length < 7)
            {
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
                int result = 0;
                result = Convert.ToInt32(spintext[1].ToString());

                NavigationService.Navigate(new Uri("/BMTA_Slot_Result.xaml?parameter=" + result, UriKind.Relative));

            }
        }

        private string GetAnswer()
        {
            Random random = new Random();
            int randomNumber = random.Next(Answers.Count);
            return Answers[randomNumber];
        }

        // List of answers.
        private List<string> answersValue;
        public List<string> Answers
        {
            get
            {
                if (answersValue == null)
                    LoadAnswers();

                return answersValue;
            }
        }

        // Load the answers from the text file.
        private void LoadAnswers()
        {
            answersValue = new List<string>();

            using (StreamReader reader =
                new StreamReader(Application.GetResourceStream(new Uri("answers.txt", UriKind.Relative)).Stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    answersValue.Add(line);
            }
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