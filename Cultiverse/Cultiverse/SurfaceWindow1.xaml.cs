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
using System.Threading;
using System.Diagnostics;
using System.Collections;
using System.Windows.Threading;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for SurfaceWindow1.xaml
    /// </summary>
    public partial class SurfaceWindow1 : SurfaceWindow
    {
        ArrayList list = new ArrayList();
        Dispatcher mainDespatch;

        ArrayList updateList = new ArrayList();
        float deltaTime;
        Stopwatch watch = new Stopwatch();
        Image bg = new Image();
        Image planet = new Image();


        /// <summary>
        /// Default constructor.
        /// </summary>
        public SurfaceWindow1()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }

            mainDespatch = Dispatcher.CurrentDispatcher;

            CompositionTarget.Rendering += update;

            initBackground();

            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();
        }

        private void initBackground()
        {
            BitmapImage bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"C:\Users\DigohD\Documents\GitHub\Cultural-Tabletop\Cultiverse\Cultiverse\Resources\bg.png", UriKind.Absolute);
            bitMap.EndInit();

            bg.Stretch = Stretch.Fill;
            bg.Source = bitMap;

            myCanvas.Children.Add(bg);

            addToUpdate(new Stars(myCanvas, "stars.png", 0.4f));
            addToUpdate(new Stars(myCanvas, "stars2.png", 0.2f));

            bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"C:\Users\DigohD\Documents\GitHub\Cultural-Tabletop\Cultiverse\Cultiverse\Resources\circle.png", UriKind.Absolute);
            bitMap.EndInit();

            planet.Stretch = Stretch.Fill;
            planet.Source = bitMap;
            planet.Width = 800;
            planet.Height = 800;
            Canvas.SetLeft(planet, 400);
            Canvas.SetTop(planet, 50);

            myCanvas.Children.Add(planet);
        }

        int count = 0;

        private void ballUpdate(float deltaTime){
            foreach (Ball b in list){
                b.update(deltaTime);
                b.collide(list);
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

                myCanvas.Children.Add(ball.getBallImage());
            }
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

            ballUpdate(deltaTime);

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

            Ball ball = new Ball(count, (int) touchPosition.X, (int) touchPosition.Y, 64, 64);
            addToUpdate(ball);
            list.Add(ball);

            myCanvas.Children.Add(ball.getBallImage());

            e.Handled = true;
        }

        float lastX, lastY;

        private void myCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            float touchX = (float)e.TouchDevice.GetPosition(this).X;
            float touchY = (float)e.TouchDevice.GetPosition(this).Y;
            float dX = touchX - lastX;
            float dY = touchY - lastY;
            foreach (Ball b in list)
                b.push(dX, dY, touchX, touchY);
            lastX = touchX;
            lastY = touchY;
        }

        
    }
}