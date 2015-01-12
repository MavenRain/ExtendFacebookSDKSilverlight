using Facebook;
using Facebook.Client;
using Microsoft.Phone.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;
using Windows.Storage.Streams;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();
        }

        private void firstButton_Click(object sender, RoutedEventArgs e)
        {
            
            //Boot FB client
            try
            {
                FacebookClient client = new FacebookClient(Session.ActiveSession.CurrentAccessTokenData.AccessToken);
                //Boot camera capture
                CameraCaptureTask camCaptureTask = new CameraCaptureTask();
                //Do the following after picture is captured
                camCaptureTask.Completed += new EventHandler<PhotoResult>((sentObject, eventArgs) =>
                {
                    //Prep byte array that will travel the HTTP POST request
                    byte[] imageData = new byte[eventArgs.ChosenPhoto.Length];
                    //Generate an input stream from the System.IO.Stream of the PhotoResult object
                    IInputStream inputStream = (IInputStream)eventArgs.ChosenPhoto;
                    //Construct DataReader that will convert System.IO.Stream to byte array, and do conversion
                    using (DataReader dataReader = new DataReader(inputStream))
                    {
                        dataReader.LoadAsync((uint)eventArgs.ChosenPhoto.Length);
                        dataReader.ReadBytes(imageData);
                    };
                    //Consumer is responsible for disposing of stream memory once conversion is finished
                    inputStream.Dispose();
                    //Construct FB request payload
                    FacebookMediaObject picture = new FacebookMediaObject();
                    //Set payload value to byte array
                    picture.SetValue(imageData);
                    Dictionary<string, object> parameters = new Dictionary<string, object>();
                    parameters["source"] = picture;
                    //Send FB request
                    client.PostTaskAsync("me/photos", parameters);
                    //Update UI
                    firstTextBlock.Text = "Picture uploaded successfully";
                });
                //Execute camera capture
                camCaptureTask.Show();
            }
            catch (ArgumentNullException) { }
        }

        private void secondButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Sign onto FB to retrieve access token that will allow uploads
            (new Session()).LoginWithBehavior("email,public_profile,user_friends", FacebookLoginBehavior.LoginBehaviorMobileInternetExplorerOnly);
        }

        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}