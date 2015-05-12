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
<<<<<<< HEAD
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Windows.Threading;
=======
using System.Collections;
using System.Diagnostics;
>>>>>>> origin/craptop-Branch

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {

<<<<<<< HEAD
        ArrayList list = new ArrayList();
        Dispatcher mainDespatch;
=======
        ArrayList updateList = new ArrayList();
        float deltaTime;
        Stopwatch watch = new Stopwatch();
        Image bg = new Image();
>>>>>>> origin/craptop-Branch

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            InitializeComponent();

<<<<<<< HEAD
            mainDespatch = Dispatcher.CurrentDispatcher;

            Thread ballThread = new Thread(this.ballUpdate);
            ballThread.SetApartmentState(ApartmentState.STA);
            ballThread.Start();
=======
            CompositionTarget.Rendering += update;

            BitmapImage bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"C:\Users\Simon\Documents\GitHub\Cultural-Tabletop\Cultiverse\Cultiverse\Resources\bg.png", UriKind.Absolute);
            bitMap.EndInit();

            bg.Stretch = Stretch.Fill;
            bg.Source = bitMap;

            myCanvas.Children.Add(bg);
>>>>>>> origin/craptop-Branch

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

<<<<<<< HEAD
            ballLoop();
            
        }

        int count = 0;

        private void ballUpdate(){
            while (true)
            {
                foreach (Ball b1 in list)
                    foreach (Ball b2 in list)
                        b1.collide(b2);
                foreach (Ball b in list)
                    b.update(mainDespatch);
                Thread.Sleep(15);
            }

        }

        private void ballLoop()
        {
            while(count < 0)
            {
                count++;
                Thread.Sleep(100);

                Ball ball = new Ball(count);
                list.Add(ball);

                Canvas_Main.Children.Add(ball.getBallImage());
            }
=======
            addToUpdate(new Stars(myCanvas, "stars.png", 0.4f));
            addToUpdate(new Stars(myCanvas, "stars2.png", 0.2f));
        }

        byte r;
        bool rising;
        SolidColorBrush solidC = new SolidColorBrush();

        public void update(object sender, EventArgs e)
        {
            watch.Stop();
            deltaTime = watch.ElapsedMilliseconds;

            foreach (Updateable u in updateList)
                u.update(deltaTime);

            solidC.Color = Color.FromRgb(r, 0, 0);
            myCanvas.Background = solidC;

            watch.Reset();
            watch.Start();
        }

        public void addToUpdate(object updateable)
        {
            updateList.Add(updateable);
        }

        public void removeFromUpdate(object updateable)
        {
            updateList.Remove(updateable);
>>>>>>> origin/craptop-Branch
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

        private void Canvas_TouchDown(object sender, TouchEventArgs e)
        {
            // Get the position of the current contact.
            Point touchPosition = e.TouchDevice.GetPosition(this);

            Ball ball = new Ball(count, (int) touchPosition.X, (int) touchPosition.Y);
            list.Add(ball);

            Canvas_Main.Children.Add(ball.getBallImage());

            e.Handled = true;
        }

        
    }
}