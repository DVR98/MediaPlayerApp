using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Pdf;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaPlayerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SongSelected : Page
    {
        private SystemMediaTransportControls systemControls;

        AllSongs SongInfo;

        public ObservableCollection<BitmapImage> PdfPages
        {
            get;
            set;

        } = new ObservableCollection<BitmapImage>();

        public SongSelected()
        {
            this.InitializeComponent();
            this.DataContext = this;
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += SystemControls_ButtonPressed;

            // Register to handle the following system transpot control buttons.
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SongInfo = e.Parameter as AllSongs;

            if (SongInfo == null)
            {
                ChangeSource();
            }
            else
            {
                OpenMedia();
            }        
        }

        public void OpenMedia()
        {
            var Path = SongInfo.SongSource;
            Video1.Source = new Uri(Path, UriKind.RelativeOrAbsolute);
            Songinfo.Text = SongInfo.Artist + '\n' + SongInfo.Title;
        }

        private void SystemControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            //throw new NotImplementedException();
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayMedia();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PauseMedia();
                    break;
                default:
                    break;
            }

        }
        async void PlayMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Video1.Play();
            });
        }

        async void PauseMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Video1.Pause();
            });
        }

        private void media_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (Video1.CurrentState)
            {
                case MediaElementState.Playing:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Paused:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Stopped:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaElementState.Closed:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangeSource();
        }

        public StorageFile file;

        async public void ChangeSource()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            file = await openPicker.PickSingleFileAsync();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;


            // video1 is a MediaElement defined in XAML
            if (null != file)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                Video1.SetSource(stream, file.ContentType);
                Songinfo.Text = file.DisplayName;
                Video1.Play();
            }
        }

        async void Load(PdfDocument pdfDoc)
        {
            PdfPages.Clear();
            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                BitmapImage image = new BitmapImage();
                var page = pdfDoc.GetPage(i);
                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    await image.SetSourceAsync(stream);
                }
                PdfPages.Add(image);
            }
        }

        async private void AddPdf_Click(object sender, RoutedEventArgs e)
        {
            // //reading pdf file
            var openPicker = new FileOpenPicker();
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add(".pdf");

            StorageFile file = await openPicker.PickSingleFileAsync();
            PdfDocument pdfDoc = await PdfDocument.LoadFromFileAsync(file);

            Load(pdfDoc);
        }
    }
}
