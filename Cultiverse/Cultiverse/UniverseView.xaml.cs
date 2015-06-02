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
using System.Collections;
using System.Diagnostics;
using Cultiverse.Model;
using Cultiverse.UI;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for UniverseView.xaml
    /// </summary>
    public partial class UniverseView : UserControl
    {
        ArrayList updateList = new ArrayList();
        Stopwatch watch = new Stopwatch();
        List<Planet> planets = new List<Planet>();
        List<World> worlds;

        private WorldDatabase worldDatabase;

        private Stars background;
        private Stars stars1;
        private Stars stars2;
        private Stars stars3;

        int worldCounter = 0;

        public UniverseView()
        {
            CompositionTarget.Rendering += update;

            InitializeComponent();
            initBackground();

            worldDatabase = new WorldDatabase();
            worlds = worldDatabase.getUniverse();

            Random rng = new Random();

            foreach(World world in worlds){
                Planet newPlanet = new Planet(100 + rng.Next(0, 4000 - 200), 100 + rng.Next(0, 4000 - 200), 0.2f, world, worldCounter++);
                this.uniCanvas.Children.Add(newPlanet);
                newPlanet.TouchDown += planet_TouchDown;
                planets.Add(newPlanet);
            }
            //Scroll to center
            scrollViewer.ScrollToHorizontalOffset(uniCanvas.Width / 2 - scrollViewer.Width / 2);
            scrollViewer.ScrollToVerticalOffset(uniCanvas.Height / 2 - scrollViewer.Height / 2);
        }

        private double _universeZoom = 1.0;
        public double UniverseZoom
        {
            get
            {
                return _universeZoom;
            }
            set
            {
                _universeZoom = value;

                Matrix scaleMatrix = new Matrix();
                scaleMatrix.Scale(_universeZoom, _universeZoom);

                uniCanvas.LayoutTransform = new MatrixTransform(scaleMatrix);
            }
        }

        Planet viewingPlanet = null;

        public void planet_TouchDown(object sender, TouchEventArgs e)
        {
            Planet planet = (Planet)sender;
            UniverseZoom = 4.0;

            scrollViewer.CanContentScroll = false;

            viewingPlanet = planet;
            this.scrollTo(planet);

            e.Handled = true;
        }

        public void scrollTo(Planet planet)
        {
            scrollViewer.ScrollToHorizontalOffset(planet.posX * UniverseZoom - scrollViewer.Width / 2 + 100 * UniverseZoom);
            scrollViewer.ScrollToVerticalOffset(planet.posY * UniverseZoom - scrollViewer.Height / 2 + 100 * UniverseZoom);
        }

        public Planet addWorld(World world)
        {
            worlds.Add(world);
            Random rng = new Random();
            float posX = 100 + rng.Next(0, 4000 - 200);
            float posY = 100 + rng.Next(0, 4000 - 200);
            Planet newPlanet = new Planet(posX, posY, 0.2f, world, worldCounter++);
            this.uniCanvas.Children.Add(newPlanet);
            newPlanet.TouchDown += planet_TouchDown;
            planets.Add(newPlanet);

            return newPlanet;
        }

        private void initBackground()
        {
            background = new Stars("bg.png", 0f);
            stars1 = new Stars("stars.png", 0.1f);
            stars2 = new Stars("stars2.png", 0.1f);
            stars3 = new Stars("stars3.png", 0.1f);


            uniCanvas.Children.Add(background);
            uniCanvas.Children.Add(stars1);
            uniCanvas.Children.Add(stars2);
            uniCanvas.Children.Add(stars3);

            //Put background in the center of the view.
            Canvas.SetLeft(background, scrollViewer.Width / 2 - background.Width / 3);
            Canvas.SetTop(background, scrollViewer.Height / 2 - background.Height / 3);

            addToUpdate(background);
            addToUpdate(stars1);
            addToUpdate(stars2);
            addToUpdate(stars3);
        }

        public void reInitBackground()
        {
            uniCanvas.Children.Remove(background);
            uniCanvas.Children.Remove(stars1);
            uniCanvas.Children.Remove(stars2);
            uniCanvas.Children.Remove(stars3);

            removeFromUpdate(background);
            removeFromUpdate(stars1);
            removeFromUpdate(stars2);
            removeFromUpdate(stars3);

            background = new Stars("bg.png", 0f);
            stars1 = new Stars("stars.png", 0.1f);
            stars2 = new Stars("stars2.png", 0.1f);
            stars3 = new Stars("stars3.png", 0.1f);

            uniCanvas.Children.Add(background);
            uniCanvas.Children.Add(stars1);
            uniCanvas.Children.Add(stars2);
            uniCanvas.Children.Add(stars3);

            //Put background in the center of the view.
            Canvas.SetLeft(background, scrollViewer.Width / 2 - background.Width / 3);
            Canvas.SetTop(background, scrollViewer.Height / 2 - background.Height / 3);

            addToUpdate(background);
            addToUpdate(stars1);
            addToUpdate(stars2);
            addToUpdate(stars3);

            ArrayList tmpStore = new ArrayList();
            foreach(Planet p in uniCanvas.Children.OfType<Planet>())
            {
                tmpStore.Add(p);
            }
            foreach (Planet p in tmpStore)
            {
                uniCanvas.Children.Remove(p);
                uniCanvas.Children.Add(p);
            }
        }

        public void p(int index)
        {
            //planets[index].setToScale(0.2f);
        }

        SolidColorBrush solidC = new SolidColorBrush();
        byte r;
        float deltaTime;
        float targetzoom = 4f, zoomInc = 0.03f, zoomAmount = 1;
        public bool zoomBlockInput = false;
        public void update(object sender, EventArgs e)
        {
            watch.Stop();
            deltaTime = watch.ElapsedMilliseconds;

            if (viewingPlanet != null && !leavingPlanet)
            {
                zoomAmount += zoomInc;
                if (zoomAmount > targetzoom)
                {
                    zoomAmount = targetzoom;
                    zoomBlockInput = false;
                }else
                    zoomBlockInput = true;

                UniverseZoom = zoomAmount;
                scrollTo(viewingPlanet);
            }
            else if (viewingPlanet != null && leavingPlanet)
            {
                zoomAmount -= zoomInc * 2;

                zoomBlockInput = true;

                UniverseZoom = zoomAmount;
                this.scrollTo(viewingPlanet);

                if (zoomAmount < 1){
                    zoomAmount = 1;
                    UniverseZoom = zoomAmount;
                    this.scrollTo(viewingPlanet);
                    scrollViewer.CanContentScroll = true;
                    viewingPlanet = null;
                    leavingPlanet = false;
                    zoomBlockInput = false;
                }
                
            }

            foreach (Updateable u in updateList)
                u.update(deltaTime);
            foreach (Planet p in planets)
                p.update(deltaTime);

            solidC.Color = Color.FromRgb(r, 0, 0);
            uniCanvas.Background = solidC;

            //ballUpdate(deltaTime);

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

        private void createWorldButton_Click(object sender, RoutedEventArgs e)
        {

        }

        /*bool lastLift = true;
        public float lastX, lastY, cX, cY;
        private void uniCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            float touchX = (float)e.GetTouchPoint(uniCanvas).Position.X;
            float touchY = (float)e.GetTouchPoint(uniCanvas).Position.Y;

            if (lastLift)
            {
                lastX = touchX;
                lastY = touchY;
                lastLift = false;
            }

            float dX = touchX - lastX;
            float dY = touchY - lastY;

            foreach (Planet p in planets)
            {
                p.pushInertedBalls(dX, dY);
            }

            Debug.WriteLine("TOUCH");

            lastX = touchX;
            lastY = touchY;
        }

        private void uniCanvas_TouchUp(object sender, TouchEventArgs e)
        {
            lastLift = true;
        }*/
        

        private void uniCanvas_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (zoomBlockInput)
                return;

            // Get the Rectangle and its RenderTransform matrix.
            Canvas rectToMove = e.OriginalSource as Canvas;
            Matrix rectsMatrix = ((MatrixTransform)rectToMove.RenderTransform).Matrix;

            // Rotate the Rectangle.
            rectsMatrix.RotateAt(e.DeltaManipulation.Rotation,
                                 e.ManipulationOrigin.X,
                                 e.ManipulationOrigin.Y);

            // Resize the Rectangle.  Keep it square 
            // so use only the X value of Scale.
            rectsMatrix.ScaleAt(e.DeltaManipulation.Scale.X,
                                e.DeltaManipulation.Scale.X,
                                e.ManipulationOrigin.X,
                                e.ManipulationOrigin.Y);

            // Apply the changes to the Rectangle.
            rectToMove.RenderTransform = new MatrixTransform(rectsMatrix);

            e.Handled = false;
        }


        void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (zoomBlockInput)
                return;

            //Background parallax
            Matrix matrix = ((MatrixTransform)background.RenderTransform).Matrix;
            matrix.TranslatePrepend(e.HorizontalChange * 0.8, e.VerticalChange * 0.8);
            background.RenderTransform = new MatrixTransform(matrix);
            //Stars 3 parallax
            matrix = ((MatrixTransform)stars3.RenderTransform).Matrix;
            matrix.TranslatePrepend(e.HorizontalChange * 0.3, e.VerticalChange * 0.3);
            stars3.RenderTransform = new MatrixTransform(matrix);
            
            /*
            foreach (Planet p in planets)
            {
                p.pushInertedBalls((float) -e.HorizontalChange * 4, (float) -e.VerticalChange * 4);
            }
            */
        }

        bool leavingPlanet;
        private void uniCanvas_TouchDown(object sender, TouchEventArgs e)
        {
            if (zoomBlockInput)
                return;

            if (viewingPlanet != null)
            {
                leavingPlanet = true;
            }
            e.Handled = false;
        }
    }
}
