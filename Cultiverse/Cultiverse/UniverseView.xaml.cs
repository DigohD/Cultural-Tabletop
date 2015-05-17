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

        public UniverseView()
        {
            CompositionTarget.Rendering += update;

            InitializeComponent();
            initBackground();

            worldDatabase = new WorldDatabase();
            worlds = worldDatabase.getUniverse();

            Random rng = new Random();

            foreach(World w in worlds){
                Planet newPlanet = new Planet(rng.Next(0, 1920), rng.Next(0, 1080), 0.2f, uniCanvas, w, this);
                planets.Add(newPlanet);
            }
        }

        private void initBackground()
        {
            addToUpdate(new Stars(uniCanvas, "bg.png", 0.03f));
            addToUpdate(new Stars(uniCanvas, "stars.png", 0.1f));
            addToUpdate(new Stars(uniCanvas, "stars2.png", 0.08f));
            addToUpdate(new Stars(uniCanvas, "stars3.png", 0.2f));
        }

        SolidColorBrush solidC = new SolidColorBrush();
        byte r;
        float deltaTime;
        public void update(object sender, EventArgs e)
        {
            watch.Stop();
            deltaTime = watch.ElapsedMilliseconds;

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

        bool lastLift = true;
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

            cX += dX;
            cY += dY;

            if (cX < -1920)
                cX = -1920;
            if (cY > 1920)
                cX = 1920;

            if (cY < -1080)
                cY = -1080;
            if (cY > 1080)
                cY = 1080;

            lastX = touchX;
            lastY = touchY;

            Debug.WriteLine("Canvas Offset " + cX + " : " + cY);

            foreach (Planet p in planets)
            {
                p.updateViewOffset(cX, cY);
            }
        }

        private void uniCanvas_TouchUp(object sender, TouchEventArgs e)
        {
            lastLift = true;
        }
    }
}
