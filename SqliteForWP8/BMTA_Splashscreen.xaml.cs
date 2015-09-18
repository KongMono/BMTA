using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Threading;
using SQLite;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using System.IO.IsolatedStorage;

namespace BMTA
{
    public partial class BMTA_Splashscreen : PhoneApplicationPage
    {
        String lastupdate;
        Boolean doingInsert = true;
        int countpage = 0;
        private SQLiteConnection dbConn;
        private WebClient webClient;
        private ProgressIndicator progressIndicator = new ProgressIndicator();
        private insert_buslineItem buslineResults = new insert_buslineItem();
        private insert_busstopItem busstopResults = new insert_busstopItem();
        private insert_landmarkItem landmarkResults = new insert_landmarkItem();

        public BMTA_Splashscreen()
        {
            InitializeComponent();
            dbConn = new SQLiteConnection(App.DB_PATH);

            lastupdate = (Application.Current as App).lastUpdate;

            countpage++;
            if (!HasNetwork() || !HasInternet())
            {
                callSplashscreen();
            }
            else
            {
                if (lastupdate == "")
                {
                    lastupdate = "2015-07-01";
                }

                callServicegetupdateAllBusline(lastupdate, countpage);
            }
        }

        private bool HasInternet()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                MessageBox.Show("No internet connection is available. Try again later.");
                return false;
            }
            return true;
        }

        private bool HasNetwork()
        {
            if (!DeviceNetworkInformation.IsNetworkAvailable)
            {
                MessageBox.Show("No network is available. Try again later.");
                return false;
            }
            return true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dbConn != null)
            {
                dbConn.Close();
            }
        }

        private void ShowProgressIndicator(String msg)
        {
            if (progressIndicator == null)
            {
                progressIndicator = new ProgressIndicator();
                progressIndicator.IsIndeterminate = true;
            }
            SystemTray.Opacity = 0;
            progressIndicator.Text = msg;
            progressIndicator.IsVisible = true;
            progressIndicator.IsIndeterminate = false;
            SystemTray.SetIsVisible(this, true);
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        private void HideProgressIndicator()
        {
            progressIndicator.IsVisible = false;
            progressIndicator.IsIndeterminate = false;
            SystemTray.SetIsVisible(this, false);
            SystemTray.SetProgressIndicator(this, progressIndicator);
        }

        public void callServicegetupdateAllBusline(String date, int page)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/updateAllBusline";
            string myParameters;
            try
            {
                myParameters = "datenow=" + date + "&page=" + Convert.ToString(page);
                Debug.WriteLine("URL callServicegetupdateAllBusline = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetupdateAllBusline_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
            }
        }

        private void callServicegetupdateAllBusline_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            ShowProgressIndicator("syncData..");

            if (e.Error != null)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                countpage = 0;
                countpage++;
                callServicegetupdateAllBusstop(lastupdate, countpage);
            }
            else
            {
                try
                {
                    buslineResults = JsonConvert.DeserializeObject<insert_buslineItem>(e.Result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                }

                if (buslineResults.status == 1)
                {
                    List<insert_busline_detailItem> ls = buslineResults.data;

                    foreach (var item in ls)
                    {
                        if (item.status == 1)
                        {

                            string json_bus_polyline = JsonConvert.SerializeObject(item.bus_polyline);
                            busline db = null;
                            try
                            {
                                db = dbConn.Query<busline>("SELECT * FROM busline WHERE id =" + item.id).FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                            }
                            if (db != null)
                            {
                                db.bus_name = item.bus_name;
                                db.bus_name_en = item.bus_name_en;
                                db.bus_line = item.bus_line;
                                db.bus_owner = item.bus_owner;
                                db.bustype = item.bustype;
                                db.bus_running = item.bus_running;
                                db.bus_color = item.bus_color;
                                db.bus_description = item.bus_description;
                                db.bus_startstop_time = item.bus_startstop_time;
                                db.bus_polyline = json_bus_polyline;
                                db.bus_start = item.bus_start;
                                db.bus_start_en = item.bus_start_en;
                                db.bus_stop = item.bus_stop;
                                db.bus_stop_en = item.bus_stop_en;
                                db.bus_direction = item.bus_direction;
                                db.bus_direction_en = item.bus_direction_en;
                                db.busstop_list = item.busstop_list;
                                db.modify_date = item.modify_date;
                                db.published = item.status;

                                try
                                {
                                    dbConn.RunInTransaction(() =>
                                    {
                                        dbConn.Update(db);
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                                }
                            }
                            else
                            {
                                try
                                {
                                    dbConn.RunInTransaction(() =>
                                    {
                                        dbConn.Insert(new busline()
                                        {
                                            id = item.id,
                                            bus_name = item.bus_name,
                                            bus_name_en = item.bus_name_en,
                                            bus_line = item.bus_line,
                                            bus_owner = item.bus_owner,
                                            bustype = item.bustype,
                                            bus_running = item.bus_running,
                                            bus_color = item.bus_color,
                                            bus_description = item.bus_description,
                                            bus_startstop_time = item.bus_startstop_time,
                                            bus_polyline = json_bus_polyline,
                                            bus_start = item.bus_start,
                                            bus_start_en = item.bus_start_en,
                                            bus_stop = item.bus_stop,
                                            bus_stop_en = item.bus_stop_en,
                                            bus_direction = item.bus_direction,
                                            bus_direction_en = item.bus_direction_en,
                                            busstop_list = item.busstop_list,
                                            modify_date = item.modify_date,
                                            published = item.status
                                        });
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                                }
                            }
                        }
                        else
                        {
                            var db = dbConn.Query<busline>("SELECT * FROM busline WHERE id =" + item.id).FirstOrDefault();
                            if (db != null)
                            {
                                db.id = item.id;

                                try
                                {
                                    dbConn.RunInTransaction(() =>
                                    {
                                        dbConn.Delete(db);
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                                }
                            }
                        }
                    }

                    countpage++;
                    callServicegetupdateAllBusline(lastupdate, countpage);

                }
                else
                {
                    countpage = 0;
                    countpage++;
                    callServicegetupdateAllBusstop(lastupdate, countpage);
                }
            }
        }



        public void callServicegetupdateAllBusstop(String date, int page)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/updateAllBusstop";
            string myParameters;
            try
            {
                myParameters = "datenow=" + date + "&page=" + Convert.ToString(page);
                Debug.WriteLine("URL callServicegetupdateAllBusstop = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetupdateAllBusstop_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
            }
        }

        private void callServicegetupdateAllBusstop_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            ShowProgressIndicator("syncData..");

            if (e.Error != null)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                countpage = 0;
                countpage++;
                callServicegetupdateAllLandmark(lastupdate, countpage);
            }
            else
            {
                try
                {
                    busstopResults = JsonConvert.DeserializeObject<insert_busstopItem>(e.Result);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                }

                if (busstopResults.status == 1)
                {
                    List<insert_busstop_detailItem> ls = busstopResults.data;

                    foreach (var item in ls)
                    {

                        if (item.status == 1)
                        {
                            try
                            {
                                var db = dbConn.Query<busstop>("SELECT * FROM busstop WHERE id = " + item.id).FirstOrDefault();
                                if (db != null)
                                {
                                    db.stop_name = item.stop_name;
                                    db.stop_name_en = item.stop_name_en;
                                    db.stop_description = item.stop_description;
                                    db.latitude = item.latitude;
                                    db.longitude = item.longitude;
                                    db.modify_date = item.modify_date;
                                    db.status = item.status;

                                    try
                                    {
                                        dbConn.RunInTransaction(() =>
                                        {
                                            dbConn.Update(db);
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        dbConn.RunInTransaction(() =>
                                        {
                                            dbConn.Insert(new busstop()
                                            {
                                                id = item.id,
                                                stop_name = item.stop_name,
                                                stop_name_en = item.stop_name_en,
                                                stop_description = item.stop_description,
                                                latitude = item.latitude,
                                                longitude = item.longitude,
                                                modify_date = item.modify_date,
                                                status = item.status
                                            });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                            }


                        }
                        else
                        {
                            var db = dbConn.Query<busstop>("SELECT * FROM busstop WHERE id = " + item.id).FirstOrDefault();
                            if (db != null)
                            {

                                try
                                {
                                    dbConn.RunInTransaction(() =>
                                    {
                                        dbConn.Delete(db);
                                    });
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                                }
                            }
                        }
                    }

                    countpage++;
                    callServicegetupdateAllBusstop(lastupdate, countpage);

                }
                else
                {
                    countpage = 0;
                    countpage++;
                    callServicegetupdateAllLandmark(lastupdate, countpage);
                }
            }
        }

        public void callServicegetupdateAllLandmark(String date, int page)
        {
            webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            String url = "http://202.6.18.31:7777/updateAllLandmark";
            string myParameters;
            try
            {
                myParameters = "datenow=" + date + "&page=" + Convert.ToString(page);
                Debug.WriteLine("URL callServicegetupdateAllBusstop = " + url);
                webClient.UploadStringCompleted += new UploadStringCompletedEventHandler(callServicegetupdateAllLandmark_Completed);
                webClient.UploadStringAsync(new Uri(url), myParameters);
            }
            catch (WebException ex)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
            }
        }

        private void callServicegetupdateAllLandmark_Completed(object sender, UploadStringCompletedEventArgs e)
        {
            ShowProgressIndicator("syncData..");
            if (e.Error != null)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                ShowProgressIndicator("success..");
                DateTime today = DateTime.Today;
                (Application.Current as App).lastUpdate = today.ToString("yyyy-MM-dd");

                if (IsolatedStorageSettings.ApplicationSettings.Contains("lastUpdate"))
                {
                    IsolatedStorageSettings.ApplicationSettings["lastUpdate"] = (Application.Current as App).lastUpdate;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("lastUpdate", (Application.Current as App).lastUpdate);
                }

                IsolatedStorageSettings.ApplicationSettings.Save();

                callSplashscreen();
            }
            try
            {
                landmarkResults = JsonConvert.DeserializeObject<insert_landmarkItem>(e.Result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
            }

            if (landmarkResults.status == 1)
            {
                List<insert_landmark_detailItem> ls = landmarkResults.data;

                foreach (var item in ls)
                {
                    if (item.published == 1)
                    {

                        var db = dbConn.Query<landmark>("SELECT * FROM landmark WHERE id =" + item.id).FirstOrDefault();
                        if (db != null)
                        {
                            db.name = item.name;
                            db.name_en = item.name_en;
                            db.lattitude = item.latitude;
                            db.longtitude = item.longitude;
                            db.type = item.type;
                            db.modify_date = item.modify_date;
                            db.published = item.published;

                            try
                            {
                                dbConn.RunInTransaction(() =>
                                {
                                    dbConn.Update(db);
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                            }
                        }
                        else
                        {
                            try
                            {
                                dbConn.RunInTransaction(() =>
                                {
                                    dbConn.Insert(new landmark()
                                    {
                                        id = item.id,
                                        name = item.name,
                                        name_en = item.name_en,
                                        lattitude = item.latitude,
                                        longtitude = item.longitude,
                                        type = item.type,
                                        modify_date = item.modify_date,
                                        published = item.published
                                    });
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                            }
                        }
                    }
                    else
                    {
                        var db = dbConn.Query<landmark>("SELECT * FROM landmark WHERE id =" + item.id).FirstOrDefault();
                        if (db != null)
                        {
                            db.id = item.id;
                            try
                            {
                                dbConn.RunInTransaction(() =>
                                {
                                    dbConn.Delete(db);
                                });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("ไม่พบข้อมูล สอบถามข้อมูลเพิ่มเติมได้ที่ 1348" + Environment.NewLine + "No Data , BMTA Call Center Tel. 1348");
                            }
                        }
                    }
                }

                countpage++;
                callServicegetupdateAllLandmark(lastupdate, countpage);
            }
            else
            {
                ShowProgressIndicator("success..");
                DateTime today = DateTime.Today;
                (Application.Current as App).lastUpdate = today.ToString("yyyy-MM-dd");

                if (IsolatedStorageSettings.ApplicationSettings.Contains("lastUpdate"))
                {
                    IsolatedStorageSettings.ApplicationSettings["lastUpdate"] = (Application.Current as App).lastUpdate;
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add("lastUpdate", (Application.Current as App).lastUpdate);
                }

                IsolatedStorageSettings.ApplicationSettings.Save();

                callSplashscreen();
            }

        }

        async void callSplashscreen()
        {
            HideProgressIndicator();
            await Task.Delay(TimeSpan.FromSeconds(3));
            NavigationService.Navigate(new Uri("/BMTA_LanguagePage.xaml", UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (NavigationService.BackStack.Count() == 1)
            {
                NavigationService.RemoveBackEntry();
            }
        }
    }
}