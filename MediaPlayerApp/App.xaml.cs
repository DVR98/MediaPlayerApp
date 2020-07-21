using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Text;

namespace MediaPlayerApp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public static AllSongsModel Model { get; set; }

        async public static Task SaveModelAsync()
        {
            //serialize the model object to a memory stream
            DataContractSerializer dcs = new DataContractSerializer(Model.GetType());

            //write model string to file
            var file = await ApplicationData.Current
                        .LocalFolder
                        .CreateFileAsync("corralmodel.xml",
                        CreationCollisionOption.OpenIfExists);
            var transaction = await file.OpenTransactedWriteAsync();
            var opn = transaction.Stream;
            var ostream = opn.GetOutputStreamAt(0);
            var stream = ostream.AsStreamForWrite();
            dcs.WriteObject(stream, Model);
            stream.Flush();
            stream.Dispose();
            await transaction.CommitAsync();
            transaction.Dispose();
        }

        async public static Task<AllSongsModel> LoadModelAsync()
        {
            //read the target file's text
            try
            {
                var file = await ApplicationData
                    .Current
                    .LocalFolder.GetFileAsync("corralmodel.xml");
                var opn = await file
                    .OpenAsync(Windows.Storage.FileAccessMode.Read);
                var stream = opn.GetInputStreamAt(0);
                var reader = new DataReader(stream);
                var size = (await file
                    .GetBasicPropertiesAsync()).Size;
                await reader.LoadAsync((uint)size);
                var model_text = reader.ReadString((uint)size);

                //before attempting deserialize, ensure the string is valid
                if (string.IsNullOrWhiteSpace(model_text))
                    return null;

                //deserialize the text
                var ms = new MemoryStream(Encoding
                    .UTF8.GetBytes(model_text));
                var dsc =
                    new DataContractSerializer(typeof(AllSongsModel));
                var ret_val = dsc.ReadObject(ms) as AllSongsModel;
                return ret_val;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        async protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            Model = await LoadModelAsync();
            //Model = null;
            if (Model == null)
            {
                Model = new AllSongsModel();

                //add default songs if Model is empty
                Model.Songs.Add(
                    new AllSongs
                    {
                        Artist = "Gorillaz",
                        SongSource = "ms-appx://MediaPlayerApp/Songs/Gorillaz - Andromeda (Official Audio).mp3",
                        Title = "Andromeda",
                    }
                    );

                Model.Songs.Add(new AllSongs
                {
                    Artist = "Coldplay",
                    Title = "Adventure of a Lifetime",
                    SongSource = "ms-appx://MediaPlayerApp/Songs/Coldplay - Adventure Of A Lifetime (Official Video).mp3"
                });

                Model.Songs.Add(new AllSongs
                    {
                        Artist = "Eminem",
                        Title = "Without me",
                        SongSource = "ms-appx://MediaPlayerApp/Songs/Eminem - Without Me.mp3"
                    });
                Model.Songs.Add(new AllSongs
                {
                    Artist = "David Guetta Feat. Sia",
                    Title = "Flames",
                    SongSource = "ms-appx://MediaPlayerApp/Songs/David Guetta & Sia - Flames (Extended).mp3"
                });
                Model.Songs.Add(new AllSongs
                {
                    Artist = "J Cole",
                    Title = "Wet Dreamz",
                    SongSource = "ms-appx://MediaPlayerApp/Songs/J. Cole - Wet Dreamz.mp3"
                });
                Model.Songs.Add(new AllSongs
                {
                    Artist = "Joji",
                    Title = "Will He",
                    SongSource = "ms-appx://MediaPlayerApp/Songs/joji - will he.mp3"
                });
                Model.Songs.Add(new AllSongs
                {
                    Artist = "Unknown",
                    Title = "Panzerkampf",
                    SongSource = "ms-appx://MediaPlayerApp/Songs/Panzerkampf.mp3"
                });
            }

            var root_host = new RootHost();
            Window.Current.Content = root_host;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                //Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
