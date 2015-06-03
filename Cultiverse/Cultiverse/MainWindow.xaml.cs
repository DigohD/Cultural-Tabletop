using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using Cultiverse.Model;
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Windows.Threading;
using Cultiverse.UI;
using Tweetinvi;
using System.IO;
using System.Threading.Tasks;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        private WorldDatabase worldDatabase;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            worldDatabase = new WorldDatabase();

            createWorldView.drawingSpace1.Visibility = Visibility.Hidden;
            createWorldView.drawingSpace2.Visibility = Visibility.Hidden;
            createWorldView.drawingSpace3.Visibility = Visibility.Hidden;
            createWorldView.drawingSpace4.Visibility = Visibility.Hidden;
            createWorldView.saveCheck1Border.Visibility = Visibility.Hidden;
            createWorldView.saveCheck2Border.Visibility = Visibility.Hidden;
            createWorldView.saveCheck3Border.Visibility = Visibility.Hidden;
            createWorldView.saveCheck4Border.Visibility = Visibility.Hidden;

            CompositionTarget.Rendering += update;

            createWorldView.Hide();

            TwitterCredentials.SetCredentials("3307331332-xhPgwZ663wad6U2vlpAp51ZYY9AlEHwAGTuNZJz", "E7Bgs2X5nHSExxBqG5VgRG8jHdU58kYc0BGnefhMCDvqP", "wh8BRnf9zZERLAWLKDcKlTeWf", "Ue4GqWWM0jaMHg3S02AkuZ4Jrhaqcx8wsx55j4OTczPQdGdsKj");
            // Publish a tweet
            // Publish with media
            
            //var imageURL = tweet.Entities.Medias.First().MediaURL;
            
        }

        void update(object sender, EventArgs e)
        {
            this.IsHitTestVisible = !universeView.zoomBlockInput;
        }

        /// <summary>
        /// Occurs when the window is about to close. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // Remove handlers for window availability events
            RemoveWindowAvailabilityHandlers();
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }

        private void NewWorld(object sender, ExecutedRoutedEventArgs e)
        {
            createWorldView.clearCreateWorldCanvas();
            createWorldView.setWorld(worldDatabase.createNewWorld());
            createWorldView.Show();
        }

        private void NewWorld_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }



        public static bool worldCreated = false;

        private void createWorldView_CreateWorldDone(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew( delegate
            {
                byte[] file = File.ReadAllBytes(createWorldView.currentWorld.ScreenshotPath);
                var tweet = Tweet.CreateTweetWithMedia("Look! A new planet was created!", file);
                tweet.Publish();
            }, TaskScheduler.FromCurrentSynchronizationContext());

            Planet planet = universeView.addWorld(createWorldView.currentWorld);
            worldDatabase.saveWorld(createWorldView.currentWorld);

            createWorldView.Hide();

            worldCreated = false;

            universeView.scrollToInstant(planet);
        }

        private void surfaceButton1_Click(object sender, RoutedEventArgs e)
        {

            if (!worldCreated)
            {
                createWorldView.setWorld(worldDatabase.createNewWorld());
                worldCreated = true;
            }

            if (createWorldView.drawingSpace2.Visibility == Visibility.Hidden)
            {
                createWorldView.Show();

                createWorldView.drawingSpace2.Show();
                createWorldView.saveCheck2Border.Visibility = Visibility.Visible;
            }
        }

        private void tokenSensor1_TokenDown(object sender, RoutedEventArgs e)
        {
            if (!worldCreated)
            {
                createWorldView.setWorld(worldDatabase.createNewWorld());
                worldCreated = true;
            }

            if (createWorldView.drawingSpace1.Visibility == Visibility.Hidden)
            {
                createWorldView.Show();

                createWorldView.drawingSpace1.Show();
                createWorldView.saveCheck1Border.Visibility = Visibility.Visible;
            }
        }

        private void tokenSensor1_TokenUp(object sender, RoutedEventArgs e)
        {
            if (createWorldView.drawingSpace1.Visibility == Visibility.Visible)
            {
                createWorldView.drawingSpace1.Hide();
                createWorldView.saveCheck1Border.Visibility = Visibility.Hidden;
            }

            if (allTokensAreUp())
            {
                createWorldView.Hide();
            }
        }

        private void tokenSensor2_TokenDown(object sender, RoutedEventArgs e)
        {
            if (!worldCreated)
            {
                createWorldView.setWorld(worldDatabase.createNewWorld());
                worldCreated = true;
            }

            if (createWorldView.drawingSpace2.Visibility == Visibility.Hidden)
            {
                createWorldView.Show();

                createWorldView.drawingSpace2.Show();
                createWorldView.saveCheck2Border.Visibility = Visibility.Visible;
            }
        }

        private void tokenSensor2_TokenUp(object sender, RoutedEventArgs e)
        {

            if (createWorldView.drawingSpace2.Visibility == Visibility.Visible)
            {
                createWorldView.drawingSpace2.Hide();
                createWorldView.saveCheck2Border.Visibility = Visibility.Hidden;
            }

            if (allTokensAreUp())
            {
                createWorldView.Hide();
            }
        }

        private void tokenSensor3_TokenDown(object sender, RoutedEventArgs e)
        {
            if (!worldCreated)
            {
                createWorldView.setWorld(worldDatabase.createNewWorld());
                worldCreated = true;
            }

            if (createWorldView.drawingSpace3.Visibility == Visibility.Hidden)
            {
                createWorldView.Show();

                createWorldView.drawingSpace3.Show();
                createWorldView.saveCheck3Border.Visibility = Visibility.Visible;
            }
        }

        private void tokenSensor3_TokenUp(object sender, RoutedEventArgs e)
        {
            if (createWorldView.drawingSpace3.Visibility == Visibility.Visible)
            {
                createWorldView.drawingSpace3.Hide();
                createWorldView.saveCheck3Border.Visibility = Visibility.Hidden;
            }

            if (allTokensAreUp())
            {
                createWorldView.Hide();
                //universeView.Visibility = Visibility.Visible;
            }
        }

        private void tokenSensor4_TokenDown(object sender, RoutedEventArgs e)
        {
            if (!worldCreated)
            {
                createWorldView.setWorld(worldDatabase.createNewWorld());
                worldCreated = true;
            }

            if (createWorldView.drawingSpace4.Visibility == Visibility.Hidden)
            {
                createWorldView.Show();

                createWorldView.drawingSpace4.Show();
                createWorldView.saveCheck4Border.Visibility = Visibility.Visible;
            }
        }

        private void tokenSensor4_TokenUp(object sender, RoutedEventArgs e)
        {

            if (createWorldView.drawingSpace4.Visibility == Visibility.Visible)
            {
                createWorldView.drawingSpace4.Hide();
                createWorldView.saveCheck4Border.Visibility = Visibility.Hidden;
            }

            if (allTokensAreUp())
            {
                createWorldView.Hide();
            }
        }

        private bool allTokensAreUp()
        {
            return !tokenSensor1.IsTokenDown
                && !tokenSensor2.IsTokenDown
                && !tokenSensor3.IsTokenDown
                && !tokenSensor4.IsTokenDown;
        }
    }
}