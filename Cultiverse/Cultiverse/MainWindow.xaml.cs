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

            CompositionTarget.Rendering += update;
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
            createWorldView.setWorld(worldDatabase.createNewWorld());
            createWorldView.Visibility = Visibility.Visible;
            universeView.Visibility = Visibility.Hidden;
        }

        private void NewWorld_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        float sizeMul, ticker;
        public void update(object sender, EventArgs e)
        {
            sizeMul = (float)Math.Abs(Math.Sin(ticker++ / 100) * 0.2f) + 0.7f;
            /*
            ellipse1.Width = 150 * sizeMul;
            ellipse1.Height = 150 * sizeMul;
            ellipse2.Width = 150 * sizeMul;
            ellipse2.Height = 150 * sizeMul;
            ellipse3.Width = 150 * sizeMul;
            ellipse3.Height = 150 * sizeMul;
            ellipse4.Width = 150 * sizeMul;
            ellipse4.Height = 150 * sizeMul;
            */
            Matrix sizingMatrix = Matrix.Identity;
            sizingMatrix.ScaleAt(sizeMul, sizeMul, 150 / 2, 150 / 2);

            ellipse1.RenderTransform = new MatrixTransform(sizingMatrix);
            ellipse2.RenderTransform = new MatrixTransform(sizingMatrix);
            ellipse3.RenderTransform = new MatrixTransform(sizingMatrix);
            ellipse4.RenderTransform = new MatrixTransform(sizingMatrix);

            //Canvas.SetLeft(ellipse1, 354 - ellipse1.Width/2);
            //Canvas.SetTop(ellipse1, 400 - ellipse1.Height / 2);
            RotateTransform rt1 = new RotateTransform();
            RotateTransform rt2 = new RotateTransform();
            RotateTransform rt3 = new RotateTransform();
            RotateTransform rt4 = new RotateTransform();

            rt1.Angle = 45;
            rt2.Angle = 135;
            rt3.Angle = 225;
            rt4.Angle = 315;

            ellipse1.LayoutTransform = rt1;
            ellipse2.LayoutTransform = rt2;
            ellipse3.LayoutTransform = rt3;
            ellipse4.LayoutTransform = rt4;
            label1.LayoutTransform = rt1;
            label2.LayoutTransform = rt2;
            label3.LayoutTransform = rt3;
            label4.LayoutTransform = rt4;
        }

        private void ellipse1_TouchDown(object sender, TouchEventArgs e)
        {
            createWorldView.setWorld(worldDatabase.createNewWorld());
            createWorldView.Visibility = Visibility.Visible;
            universeView.Visibility = Visibility.Hidden;
            //Debug.WriteLine("kbuygut");
            //label1.Content = "Player #1";

        }
    }
}