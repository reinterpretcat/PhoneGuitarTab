using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneGuitarTab.UI.Infrastructure;
using PhoneGuitarTab.UI.Resources;

namespace PhoneGuitarTab.UI
{
    public partial class App : Application
    {
        public static String AppForceCulture = "tr-TR";
        /// <summary>
        ///     Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
#if DEBUG
            Utilities.BeginRecording();
#endif
            // Show graphics profiling information while debugging.
            if (Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            // Language display initialization
            InitializeLanguage();
        }

        private void InitializeLanguage()
        {
            try
            {
                // Change locale to appForceCulture if it is not empty
                if (String.IsNullOrWhiteSpace(AppForceCulture) == false)
                {
                    // Force app globalization to follow appForceCulture
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(AppForceCulture);

                    // Force app UI culture to follow appForceCulture
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(AppForceCulture);
                }
                // Set the font to match the display language defined by the
                // ResourceLanguage resource string for each supported language.
                //
                // Fall back to the font of the neutral language if the display
                // language of the phone is not supported.
                //
                // If a compiler error occurs, ResourceLanguage is missing from
                // the resource file.
                RootFrame.Language = XmlLanguage.GetLanguage(AppResources.ResourceLanguage);
                // Set the FlowDirection of all elements under the root frame based
                // on the ResourceFlowDirection resource string for each
                // supported language.
                //
                // If a compiler error occurs, ResourceFlowDirection is missing from
                // the resource file.
                var flow = (FlowDirection)Enum.Parse(typeof(FlowDirection), AppResources.ResourceFlowDirection, false);
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

        #region Properties

        /// <summary>
        ///     Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }


        public static string Version
        {
            get
            {
                string name = typeof (App).Assembly.FullName;
                AssemblyName asmName = new AssemblyName(name);
                return asmName.Version.ToString();
            }
        }

        #endregion Properties

        #region Event handlers

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
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

            if (FatalException == null)
                FatalException = e.ExceptionObject;


            e.Handled = true;

            // (RootVisual as PhoneApplicationFrame).Source =
            //   new Uri("/Views/ErrorView.xaml", UriKind.Relative);
        }

        #endregion Event handlers

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized;

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

        #endregion

        #region

        public static Exception FatalException { get; set; }

        #endregion
    }
}