﻿using System;
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


        public UniverseView()
        {
            CompositionTarget.Rendering += update;

            InitializeComponent();
            initBackground();

            worldDatabase = new WorldDatabase();
            worlds = worldDatabase.getUniverse();

            Random rng = new Random();

            int counter = 0;
            foreach(World world in worlds){
                Planet newPlanet = new Planet(100 + rng.Next(0, 4000 - 200), 100 + rng.Next(0, 4000 - 200), 0.2f, world, counter++);
                this.uniCanvas.Children.Add(newPlanet);
                planets.Add(newPlanet);
            }

            
        }

        private void initBackground()
        {
            background = new Stars("bg.png", 0.03f);
            stars1 = new Stars("stars.png", 0.1f);
            stars2 = new Stars("stars2.png", 0.1f);
            stars3 = new Stars("stars3.png", 0.1f);
            uniCanvas.Children.Add(background);
            uniCanvas.Children.Add(stars1);
            uniCanvas.Children.Add(stars2);
            uniCanvas.Children.Add(stars3);

            addToUpdate(background);
            addToUpdate(stars1);
            addToUpdate(stars2);
            addToUpdate(stars3);
        }

        public void p(int index)
        {
            //planets[index].setToScale(0.2f);
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
        /*private void uniCanvas_TouchMove(object sender, TouchEventArgs e)
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
        */

        private void uniCanvas_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
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

        private void SurfaceScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Matrix matrix = ((MatrixTransform)background.RenderTransform).Matrix;
            matrix.TranslatePrepend(e.HorizontalChange * 0.8, e.VerticalChange * 0.8);

            background.RenderTransform = new MatrixTransform(matrix);

            Console.WriteLine("Vertical change" + e.VerticalOffset);
            Console.WriteLine("Horizontal change" + e.HorizontalOffset);
        }
    }
}
