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
        Ellipse planet = new Ellipse();

        public CreateWorldView()
        {
            InitializeComponent();

            mainDespatch = Dispatcher.CurrentDispatcher;

            CompositionTarget.Rendering += update;

            initBackground();
        }

        private void initBackground()
        {
            addToUpdate(new Stars(myCanvas, "bg.png", 0.03f));
            addToUpdate(new Stars(myCanvas, "stars.png", 0.1f));
            addToUpdate(new Stars(myCanvas, "stars2.png", 0.08f));
            addToUpdate(new Stars(myCanvas, "stars3.png", 0.2f));

            planet.Fill = new SolidColorBrush(Colors.LightGray);
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
            //Save bitmap
            FileStream fileStream = new FileStream("C:\\Users\\Simon\\Desktop\\img.png", FileMode.Create, FileAccess.Write);
            int marg = int.Parse(myCanvas.Margin.Left.ToString());
            RenderTargetBitmap rtb =
                    new RenderTargetBitmap((int)myCanvas.ActualWidth - marg,
                            (int)myCanvas.ActualHeight - marg, 0, 0,
                        PixelFormats.Pbgra32);
            rtb.Render(myCanvas);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            png.Save(fileStream);
            fileStream.Close();

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

        private void addDrawingFromInkCanvas(SurfaceInkCanvas inkCanvas)
        {
            WorldDrawing drawing = currentWorld.createNewDrawing();

            inkCanvas.Clip = new EllipseGeometry(new Point(inkCanvas.ActualWidth / 2, inkCanvas.ActualHeight / 2), inkCanvas.ActualWidth, inkCanvas.ActualHeight);
            

            //Save strokes
            FileStream fileStream = new FileStream(drawing.StrokesFilePath, FileMode.Create, FileAccess.Write);
            inkCanvas.Strokes.Save(fileStream);
            fileStream.Close();

            //Save bitmap
            fileStream = new FileStream(drawing.BitmapFilePath, FileMode.Create, FileAccess.Write);
            int marg = int.Parse(inkCanvas.Margin.Left.ToString());
            RenderTargetBitmap rtb =
                    new RenderTargetBitmap((int)inkCanvas.ActualWidth - marg,
                            (int)inkCanvas.ActualHeight - marg, 0, 0,
                        PixelFormats.Pbgra32);
            rtb.Render(inkCanvas);
            PngBitmapEncoder png = new PngBitmapEncoder();
            png.Frames.Add(BitmapFrame.Create(rtb));
            png.Save(fileStream);
            fileStream.Close();

            Image image = new Image();
            image.Source = new ImageSourceConverter().ConvertFromString(drawing.BitmapFilePath) as ImageSource;

            Ball ball = new Ball(count, (int)(myCanvas.Width / 2 - 64), (int)(myCanvas.Height / 2 - 64), 128, 128, drawing);
            addToUpdate(ball);
            list.Add(ball);

            myCanvas.Children.Add(ball.getBallImage());

            inkCanvas.Strokes.Clear();
        }

        private void drawingDone(object sender, RoutedEventArgs e)
        {
            this.addDrawingFromInkCanvas((SurfaceInkCanvas)sender);
        }



    }

}
