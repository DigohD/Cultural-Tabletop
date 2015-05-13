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
using System.IO;
using Cultiverse.Model;
using System.Globalization;
using System.Collections;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using Microsoft.Surface.Presentation.Controls;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for CreateWorldView.xaml
    /// </summary>
    public partial class CreateWorldView : UserControl
    {
        private World currentWorld;

        ArrayList list = new ArrayList();
        Dispatcher mainDespatch;

        ArrayList updateList = new ArrayList();
        float deltaTime;
        Stopwatch watch = new Stopwatch();
        Image bg = new Image();
        Image planet = new Image();

        public CreateWorldView()
        {
            InitializeComponent();

            mainDespatch = Dispatcher.CurrentDispatcher;

            CompositionTarget.Rendering += update;

            initBackground();


        }

        private void initBackground()
        {
            BitmapImage bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"Resources\bg.png", UriKind.Relative);
            bitMap.EndInit();

            bg.Stretch = Stretch.Fill;
            bg.Source = bitMap;

            myCanvas.Children.Add(bg);

            addToUpdate(new Stars(myCanvas, "stars.png", 0.4f));
            addToUpdate(new Stars(myCanvas, "stars2.png", 0.2f));

            bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"Resources\circle.png", UriKind.Relative);
            bitMap.EndInit();

            planet.Stretch = Stretch.Fill;
            planet.Source = bitMap;
            planet.Width = 800;
            planet.Height = 800;
            Canvas.SetLeft(planet, myCanvas.Width / 2 - planet.Width/2);
            Canvas.SetTop(planet, myCanvas.Height / 2 - planet.Height/2);

            myCanvas.Children.Add(planet);
        }

        int count = 0;

        private void ballUpdate(float deltaTime)
        {
            foreach (Ball b in list)
            {
                b.update(deltaTime);
                b.collide(list);
            }
        }

        SolidColorBrush solidC = new SolidColorBrush();
        byte r;
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

        private void saveWorldButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addDrawingButton_Click(object sender, RoutedEventArgs e)
        {
        }


        internal void setWorld(World world)
        {
            currentWorld = world;
        }

        float lastX, lastY;

        private void myCanvas_TouchMove(object sender, TouchEventArgs e)
        {
            float touchX = (float)e.GetTouchPoint(myCanvas).Position.X;
            float touchY = (float)e.GetTouchPoint(myCanvas).Position.Y;
            float dX = touchX - lastX;
            float dY = touchY - lastY;
            foreach (Ball b in list)
                b.push(dX, dY, touchX, touchY);
            lastX = touchX;
            lastY = touchY;
        }

        private void addDrawingButton1_Click(object sender, RoutedEventArgs e)
        {
            addDrawingFromInkCanvas(inkCanvas1);
        }


        private void addDrawingButton2_Click(object sender, RoutedEventArgs e)
        {
            addDrawingFromInkCanvas(inkCanvas2);
        }

        private void addDrawingFromInkCanvas(SurfaceInkCanvas inkCanvas)
        {
            WorldDrawing drawing = currentWorld.createNewDrawing();

            //Save strokes
            FileStream fileStream = new FileStream(drawing.StrokesFilePath, FileMode.Create, FileAccess.Write);
            inkCanvas.Strokes.Save(fileStream);
            fileStream.Close();

            //Save bitmap
            fileStream = new FileStream(drawing.BitmapFilePath, FileMode.Create, FileAccess.Write);
            int marg = int.Parse(inkCanvas1.Margin.Left.ToString());
            RenderTargetBitmap rtb =
                    new RenderTargetBitmap((int)inkCanvas1.ActualWidth - marg,
                            (int)inkCanvas1.ActualHeight - marg, 0, 0,
                        PixelFormats.Pbgra32);
            rtb.Render(inkCanvas1);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            png.Save(fileStream);
            fileStream.Close();

            Image image = new Image();
            image.Source = new ImageSourceConverter().ConvertFromString(drawing.BitmapFilePath) as ImageSource;

            Ball ball = new Ball(count, (int)(myCanvas.Width / 2 - 64), (int)(myCanvas.Height / 2 - 64), 128, 128, drawing.BitmapFilePath);
            addToUpdate(ball);
            list.Add(ball);

            myCanvas.Children.Add(ball.getBallImage());

            inkCanvas1.Strokes.Clear();
        }
    }

}
