using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MediaPlayerApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();

            //systemControls = SystemMediaTransportControls.GetForCurrentView();
            //systemControls.ButtonPressed += SystemControls_ButtonPressed;

            // Register to handle the following system transpot control buttons.
            this.Loaded += Dashboard_Loaded;
        }

        private void Dashboard_Loaded
            (object sender, RoutedEventArgs e)
        {
            gridview_Songs.DataContext = App.Model.Songs;
        }

        // This code executes when you click an item in the GridView
        private void SongSelected(object sender, ItemClickEventArgs e)
        {
            var selected_Song = e.ClickedItem as AllSongs;
            Frame.Navigate(typeof(SongSelected), selected_Song);
        }

        private void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            NavigationBar.IsPaneOpen = !NavigationBar.IsPaneOpen;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string SearchRequest = SearchBox.Text;

            var song = App.Model.Songs
                .Where(i =>
                i.Title.ToLower() == SearchRequest.ToLower())
                .FirstOrDefault();

            if (song == null)
            {
                Frame.Navigate(typeof(MainPage));
                SearchBox.Text = "";
            }
            else if (song.Title == SearchRequest)
            {
                gridview_Songs.DataContext = App.Model.Songs
                .Where(i =>
                i.Title.ToLower() == SearchRequest.ToLower())
                .ToList();
                SearchBox.Text = "";
            }
            else if (SearchRequest == "")
            {
                gridview_Songs.DataContext = App.Model.Songs;
            }
            else if ((SearchRequest != song.Title) || (SearchRequest != ""))
            {
                MessageDialog ms = new MessageDialog("Couldn't find " + SearchRequest + ", please try again ");
                ms.ShowAsync();
            }

        }

        private void Playlists_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlaylistPage));
        }

        //private void AddSong_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    Frame.Navigate(typeof(SongCreate));
        //}

        private void StreamSong_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Frame.Navigate(typeof(SongSelected));
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    string path = "ms-appx://MediaPlayerApp/Songs/Gorillaz - Andromeda (Official Audio).mp3";
        //    Video1.Source = new Uri(path, UriKind.RelativeOrAbsolute);
        //}
    }
}
