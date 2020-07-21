using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Storage;
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
using Windows.Web.Http;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MediaPlayerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public class GenericCommand : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;
        public event Action<string> DoSomething;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var command = parameter as string;
            DoSomething?.Invoke(command);
        }
    }

    public sealed partial class SongCreate : Page
    {
        GenericCommand _command;

        public SongCreate()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            _command = new GenericCommand();
            _command.DoSomething += _command_DoSomething;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Refer Command Binding in Chapter 2
            this.DataContext = _command;

            // Load Name and Procedure controls
        }

        private AllSongs CreateReservation()
        {
             var song = App.Model.Songs
                .Where(i =>
                i.Artist.ToLower() == ArtistName.Text.ToLower())
                .FirstOrDefault();

            var songName = App.Model.Songs
                .Where(i =>
                i.Title.ToLower() == SongName.Text.ToLower())
                .FirstOrDefault();

            if ((song == null) && (songName == null))
            {
                //add a new song if 
                //none exist with that name
                song = new AllSongs
                {
                    Artist = ArtistName.Text,
                    Title = SongName.Text,
                    SongSource = SongPath.Text,
                };
            }

            return song;
        }

        async private void _command_DoSomething(string command)
        {

            if (command == "Add Now")
            {
                var new_song = CreateReservation();
                App.Model.Songs.Add(new_song);
                await App.SaveModelAsync();
                Frame.Navigate(typeof(MainPage));
            }
        }

        async private void CollectSource()
        {
            var openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            StorageFile file = await openPicker.PickSingleFileAsync();
            // Process picked file
            if (file != null)
            {
                // Store file for future access
                Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
                string path = file.Path;
                SongPath.Text = path;
            }
            else
            {
                MessageDialog message = new MessageDialog("Please pick a song");
            }
        }

        private void GetSource_Click(object sender, RoutedEventArgs e)
        {
            CollectSource();
        }
    }
    
}
