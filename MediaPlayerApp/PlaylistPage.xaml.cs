using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaPlayerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistPage : Page
    {
        private SystemMediaTransportControls systemControls;

        public PlaylistPage()
        {
            this.InitializeComponent();
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += SystemControls_ButtonPressed;
            Songinfo.Text = "Add songs, but the Maximum is 500";
            ChangeSource();
        }

        //public StorageFile file;
        StorageFile[] playlist = new StorageFile[500];
        int i = 0;


        async public void ChangeSource()
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            //file = await openPicker.PickSingleFileAsync();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;


            var files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                // Application now has read/write access to the picked file(s)
                foreach (Windows.Storage.StorageFile file1 in files)
                {
                    var stream = await file1.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                    Video1.SetSource(stream, file1.ContentType);
                    playlist[i] = file1;
                    Songinfo.Text = file1.DisplayName;
                    int n = i + 1;
                    PlaylistSongs.Text += "\n" + n.ToString() + ") " + playlist[i].DisplayName;
                    i++;
                }
            }
            else
            {
                Songinfo.Text = "Operation cancelled.";
            }

            // video1 is a MediaElement defined in XAML
            //if (null != file)
            //{
            //    var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
            //    Video1.SetSource(stream, file.ContentType);
            //    playlist[i] = file;
            //    Songinfo.Text = file.DisplayName;
            //    PlaylistSongs.Text += "\n" + "1" + ") " + playlist[i].DisplayName;
            //    i++;
            //    Video1.Play();
            //}
        }

        int x = 0;
        private async void NextSong()
        {
            x++;
            var file = playlist[x];
            if (file == null)
            {
                x = 0;
                file = playlist[x];
                var properties = await file.Properties.GetMusicPropertiesAsync();
                if (file != null)
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    Songinfo.Text = file.DisplayName;
                    // mediaControl is a MediaElement defined in XAML
                    Video1.SetSource(stream, file.ContentType);
                }
            }
            else
            {
                var properties = await file.Properties.GetMusicPropertiesAsync();
                if (file != null)
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    Songinfo.Text = file.DisplayName;
                    // mediaControl is a MediaElement defined in XAML
                    Video1.SetSource(stream, file.ContentType);
                }
            }
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

        async private void AddSong_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            StorageFile file = await openPicker.PickSingleFileAsync();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;


            // video1 is a MediaElement defined in XAML
            if (null != file)
            {
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
                playlist[i] = file;
                int b = i + 1;
                PlaylistSongs.Text += "\n" + b.ToString() + ") " + playlist[i].DisplayName;
                Songinfo.Text = file.DisplayName;
                // mediaControl is a MediaElement defined in XAML
                Video1.SetSource(stream, file.ContentType);
                i++;
            }
        }

        private void NextSong_Click(object sender, RoutedEventArgs e)
        {
            NextSong();
        }

        async private void Previous1Song_Click(object sender, RoutedEventArgs e)
        {
            x--;
            if (x < 0)
            {
                x = 0;
                var file = playlist[x];
                var properties = await file.Properties.GetMusicPropertiesAsync();
                if (file != null)
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    Songinfo.Text = file.DisplayName;
                    // mediaControl is a MediaElement defined in XAML
                    Video1.SetSource(stream, file.ContentType);
                }
            }
            else
            {
                var file = playlist[x];
                var properties = await file.Properties.GetMusicPropertiesAsync();
                if (file != null)
                {
                    var stream = await file.OpenAsync(FileAccessMode.Read);
                    Songinfo.Text = file.DisplayName;
                    // mediaControl is a MediaElement defined in XAML
                    Video1.SetSource(stream, file.ContentType);
                }
            }
        }
    }
}
