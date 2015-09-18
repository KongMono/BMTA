using System;
using System.Diagnostics;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BMTA.Resources;
using Windows.Storage;
using System.IO.IsolatedStorage;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using BMTA.Item;
using System.Collections.ObjectModel;
using Parse;
using SQLite;
using Microsoft.Phone.Net.NetworkInformation;

namespace BMTA
{
    public partial class App : Application
    {
        public string AppName = "BMTA";
        public string AppVersion = "1.0";
        public string pathAnalytic = "";
        public string Language = "";
        public string lastUpdate = "";
        public string lat_current, lon_current;
        public List<SpeedCheckItem> CheckSpeedList = new List<SpeedCheckItem>();
        public List<SlotItem> MemSlotList = new List<SlotItem>();
        public List<datasearchLandMarkByGeoItem> MemLandMarkList = new List<datasearchLandMarkByGeoItem>();
        public List<buslineItem> DataSearchList = new List<buslineItem>();
        public List<buslineItem> DataBuslinehList = new List<buslineItem>();
        public dataNearBusStopItem DataBusstopDetail = new dataNearBusStopItem();
        public new_searchfindRoutingItem DataLandMark = new new_searchfindRoutingItem();
        public new_searchfindRoutingItem DataStop = new new_searchfindRoutingItem();
        public new_searchfindRoutingItem DataStartStop = new new_searchfindRoutingItem();
        public new_searchfindRoutingItem_data RountingDataLandMark = new new_searchfindRoutingItem_data();
        public new_searchfindRoutingItem_data RountingDataBusStop = new new_searchfindRoutingItem_data();
        public new_searchfindRoutingItem_data RountingDataStartStop = new new_searchfindRoutingItem_data();
        public static string DB_PATH = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "bmtadatabase.sqlite");
        public static SQLiteAsyncConnection connection;
        public static bool isDatabaseExisting;

        /// <summary>
        /// Service
        /// </summary>
        public string getNearBusStop = "http://2f.backend.in.th/bmta/?method=getNearBusStop&";

        public void SavePersistantData()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MemLandMarkList"))
            {
                IsolatedStorageSettings.ApplicationSettings["MemLandMarkList"] = MemLandMarkList;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("MemLandMarkList", MemLandMarkList);
            }
            // make sure data is saved immediatelly

            if (IsolatedStorageSettings.ApplicationSettings.Contains("MemSlotList"))
            {
                IsolatedStorageSettings.ApplicationSettings["MemSlotList"] = MemSlotList;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("MemSlotList", MemSlotList);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("CheckSpeedList"))
            {
                IsolatedStorageSettings.ApplicationSettings["CheckSpeedList"] = CheckSpeedList;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("CheckSpeedList", CheckSpeedList);
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("lastUpdate"))
            {
                IsolatedStorageSettings.ApplicationSettings["lastUpdate"] = lastUpdate;
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings.Add("lastUpdate", lastUpdate);
            }

            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public void LoadPersistantData()
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("MemLandMarkList"))
            {
                MemLandMarkList = (List<datasearchLandMarkByGeoItem>)IsolatedStorageSettings.ApplicationSettings["MemLandMarkList"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("MemSlotList"))
            {
                MemSlotList = (List<SlotItem>)IsolatedStorageSettings.ApplicationSettings["MemSlotList"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("CheckSpeedList"))
            {
                CheckSpeedList = (List<SpeedCheckItem>)IsolatedStorageSettings.ApplicationSettings["CheckSpeedList"];
            }

            if (IsolatedStorageSettings.ApplicationSettings.Contains("lastUpdate"))
            {
                lastUpdate = IsolatedStorageSettings.ApplicationSettings["lastUpdate"] as string;
            }
        }
        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// 

        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public static PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions.
            UnhandledException += Application_UnhandledException;

            // Standard XAML initialization
            InitializeComponent();

            if (DeviceNetworkInformation.IsNetworkAvailable || NetworkInterface.GetIsNetworkAvailable())
            {
                ParseClient.Initialize("u3c5C2f1biiBTtbPjaxJsBJayvVArZl8ED7YfgOv", "BtLxWbgRSaZaxSKemzlSJpw9NhT1yTWRylpntjFw");

                this.Startup += async (sender, args) =>
                {
                    // This optional line tracks statistics around app opens, including push effectiveness:
                    ParseAnalytics.TrackAppOpens(RootFrame);

                    // By convention, the empty string is considered a "Broadcast" channel
                    // Note that we had to add "async" to the definition to use the await keyword
                    await ParsePush.SubscribeAsync("");
                };
            }           
            // Phone-specific initialization
            InitializePhoneApplication();
            //SubscribeToParse();
            // Language display initialization
            InitializeLanguage();

            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode,
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Prevent the screen from turning off while under the debugger by disabling
                // the application's idle detection.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
        }

        //private void ParsePushOnToastNotificationReceived(object sender, Microsoft.Phone.Notification.NotificationEventArgs e)
        //{
           
        //}
        //public void SubscribeToParse() { ParsePush.SubscribeAsync(""); }
       

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            LoadPersistantData();
            CopyDatabase();
            //ConnectToDB();
        }

        public static async void ConnectToDB()
        {
            bool isDatabaseExisting = false;

            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("bmtadatabase.sqlite");
                isDatabaseExisting = true;
            }
            catch (Exception ex)
            {
                isDatabaseExisting = false;
            }

            if (!isDatabaseExisting)
            {
                try
                {
                    StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync("bmtadatabase.sqlite");
                    await databaseFile.CopyAsync(ApplicationData.Current.LocalFolder);
                    isDatabaseExisting = true;
                }
                catch (Exception ex)
                {
                    isDatabaseExisting = false;
                }
            }

            if (isDatabaseExisting)
            {
                connection = new SQLiteAsyncConnection(Path.Combine(ApplicationData.Current.LocalFolder.Path, "bmtadatabase.sqlite"), true);
            }
        }

        private async Task CopyDatabase()
        {
            bool isDatabaseExisting = false;
            try
            {
                StorageFile storageFile = await ApplicationData.Current.LocalFolder.GetFileAsync("bmtadatabase.sqlite");
                isDatabaseExisting = true;
            }
            catch(Exception ex)
            {
                isDatabaseExisting = false;
            }

            if (!isDatabaseExisting)
            {
                StorageFile databaseFile = await Package.Current.InstalledLocation.GetFileAsync("bmtadatabase.sqlite");
                await databaseFile.CopyAsync(ApplicationData.Current.LocalFolder);
            }
        }



        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            LoadPersistantData();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            SavePersistantData();
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
            SavePersistantData();
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new TransitionFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Handle reset requests for clearing the backstack
            RootFrame.Navigated += CheckForResetNavigation;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        private void CheckForResetNavigation(object sender, NavigationEventArgs e)
        {
            // If the app has received a 'reset' navigation, then we need to check
            // on the next navigation to see if the page stack should be reset
            if (e.NavigationMode == NavigationMode.Reset)
                RootFrame.Navigated += ClearBackStackAfterReset;
        }

        private void ClearBackStackAfterReset(object sender, NavigationEventArgs e)
        {
            // Unregister the event so it doesn't get called again
            RootFrame.Navigated -= ClearBackStackAfterReset;

            // Only clear the stack for 'new' (forward) and 'refresh' navigations
            if (e.NavigationMode != NavigationMode.New && e.NavigationMode != NavigationMode.Refresh)
                return;

            // For UI consistency, clear the entire page stack
            while (RootFrame.RemoveBackEntry() != null)
            {
                ; // do nothing
            }
        }

        #endregion

        // Initialize the app's font and flow direction as defined in its localized resource strings.
        //
        // To ensure that the font of your application is aligned with its supported languages and that the
        // FlowDirection for each of those languages follows its traditional direction, ResourceLanguage
        // and ResourceFlowDirection should be initialized in each resx file to match these values with that
        // file's culture. For example:
        //
        // AppResources.es-ES.resx
        //    ResourceLanguage's value should be "es-ES"
        //    ResourceFlowDirection's value should be "LeftToRight"
        //
        // AppResources.ar-SA.resx
        //     ResourceLanguage's value should be "ar-SA"
        //     ResourceFlowDirection's value should be "RightToLeft"
        //
        // For more info on localizing Windows Phone apps see http://go.microsoft.com/fwlink/?LinkId=262072.
        //
        private void InitializeLanguage()
        {
            try
            {
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the Display
                // language of the phone is not supported.
                //
                // If a compiler error is hit then ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);

                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error is hit then ResourceFlowDirection is missing from
                // the resource file.
                FlowDirection flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection);
                RootFrame.FlowDirection = flow;
            }
            catch
            {
                // If an exception is caught here it is most likely due to either
                // ResourceLangauge not being correctly set to a supported language
                // code or ResourceFlowDirection is set to a value other than LeftToRight
                // or RightToLeft.

                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }

                throw;
            }
        }
    }
}