using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BMTA.Resources;
using SQLite;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System.Globalization;
using System.Threading.Tasks;
using BMTA.Item;

namespace BMTA
{

    public partial class BMTA_Slot_Result : PhoneApplicationPage
    {
        ImageBrush background;
        public String lang = (Application.Current as App).Language;
        public String result;
        public SlotItem item = new SlotItem();

        public BMTA_Slot_Result()
        {
            InitializeComponent();

        }

        private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
        {
            background = new ImageBrush();
            if (lang.Equals("th"))
            {
                titleName.Text = "กิจกรรม";
                headfortune.Text = "ตัวเลขของคุณคือ";
                background.ImageSource = new BitmapImage(new Uri("/Assets/BMTA_slotresult_reset.png", UriKind.Relative));
                btn_tryagain.Background = background;
            }
            else
            {
                titleName.Text = "Events";
                headfortune.Text = "Your ticket No. is";
                background.ImageSource = new BitmapImage(new Uri("/Assets/BMTA_slotresult_reset_en.png", UriKind.Relative));
                btn_tryagain.Background = background;
            }


            result = this.NavigationContext.QueryString["parameter"];

            btn_num.Content = result;

            if ((Application.Current as App).MemSlotList.Count < 1)
            {
                SlotItem slot = new SlotItem();
                slot.index_slot = "0";
                slot.index_detail = "ระหว่างเดินทางระวังทรัพย์สินจะหาย ไม่ควรประมาท ความรักไม่ควรใจร้อนจะผิดหวังได้";
                slot.index_detail_en = "Aware of losing things while traveling. Should be calmer with your love issues.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "1";
                slot.index_detail = "วันนี้จะมีโชคดี เดินทางปลอดภัย การงานราบรื่นไร้ปัญหากังวลใจ";
                slot.index_detail_en = "It's your lucky day for both traveling and working.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "2";
                slot.index_detail = "ดวงความรักกำลังพุ่ง เหมาะกับการบอกรักคนที่หมายตา สำหรับคนที่มีคู่แล้ว ความรักจะหวานเป็นพิเศษ";
                slot.index_detail_en = "Love is in the air. Great time to share your romance for both singles and couples.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "3";
                slot.index_detail = "ของหายจะได้คืน หรือได้พบคนที่ไม่ได้พบกันนาน บริวารจะเดินทางนำโชคลาภมาให้";
                slot.index_detail_en = "Found your long lost thing or person. Your crew will bring you good news.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "4";
                slot.index_detail = "วันนี้จะต้องเดินทางบ่อย มีเกณฑ์จะต้องติดต่อพบปะผู้คนมากหน้าหลายตา คนมีคู่ระวังปัญหาที่เกิดจากความไม่เข้าใจ";
                slot.index_detail_en = "Many trips to take and many people to meet. Be aware of some misunderstanding between you and with your love one.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "5";
                slot.index_detail = "มีคนกำลังคิดถึงคุณ มีโชคด้านเงินๆทองๆ แต่ต้องระวังเรื่องสุขภาพ";
                slot.index_detail_en = "Somebody is missing you. Your luck will be good on financial, but be aware of your health.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "6";
                slot.index_detail = "มีเกณฑ์จะเจอเนื้อคู่ระหว่างการเดินทาง มีเรื่องน่าตื่นเต้นเข้ามาและคุณจะรู้สึกสนุกไปกับมัน";
                slot.index_detail_en = "You might meet Mr. or Miss Right while traveling. There will be some amusement and you will enjoy the time.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "7";
                slot.index_detail = "จะเกิดปัญหาที่ทำให้คุณต้องแก้ปัญหาเฉพาะหน้าแต่คุณจะผ่านมันไปได้ด้วยดีเพราะมีคนคอยช่วยเหลือ";
                slot.index_detail_en = "There are some troubles to get through, but you will be fine with some helps.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "8";
                slot.index_detail = "อย่าไว้ใจคนแปลกหน้า หรือคนที่เพิ่งรู้จักกันได้ไม่นาน จะมีโชคจากการเดินทาง";
                slot.index_detail_en = "Don't trust  the stranger or anybody you just met. Traveling will bring the goods to you.";
                (Application.Current as App).MemSlotList.Add(slot);

                slot = new SlotItem();
                slot.index_slot = "9";
                slot.index_detail = "วันนี้ไม่ควรมีเรื่องกับใครเพราะจะเป็นฝ่ายเสียเปรียบ คนกำลังแอบชอบคุณอยู่จะเปิดเผยให้คุณรู้";
                slot.index_detail_en = "Shouldn't get into trouble because you will adverse in the game. Somebody has been falling for you, and going to expose it.";
                (Application.Current as App).MemSlotList.Add(slot);
            }

            foreach (var i in (Application.Current as App).MemSlotList)
            {
                if (i.index_slot == result)
                {
                    if (lang.Equals("th"))
                    {
                        lblDetail.Text = i.index_detail;
                    }
                    else
                    {
                        lblDetail.Text = i.index_detail_en;
                    }
                    return;
                }

            }


        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public void SaveToMediaLibrary(FrameworkElement element)
        {
            try
            {
                var bmp = new WriteableBitmap(element, null);

                var ms = new MemoryStream();
                bmp.SaveJpeg(ms, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
                ms.Seek(0, SeekOrigin.Begin);

                var lib = new MediaLibrary();
                var filePath = string.Format(DateTime.Now + "_" + "BMTASlotResult.jpg");


                var bmpc = new WriteableBitmap(0, 0).FromStream(ms);
                var croppedBmp = bmpc.Crop(0, 0, bmpc.PixelWidth, 600);


                var picture = croppedBmp.SaveToMediaLibrary(DateTime.Now + "_" + "BMTASlotResult.jpg");

                ShareMediaTask shareMediaTask = new ShareMediaTask();
                shareMediaTask = new ShareMediaTask();
                shareMediaTask.FilePath = picture.GetPath();
                shareMediaTask.Show();

            }
            catch
            {
                MessageBox.Show(
                    "There was an error. Please disconnect your phone from the computer before saving.",
                    "Cannot save",
                    MessageBoxButton.OK);
            }
        }

        private void btn_share_Click(object sender, RoutedEventArgs e)
        {
            SaveToMediaLibrary(LayoutRoot);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}