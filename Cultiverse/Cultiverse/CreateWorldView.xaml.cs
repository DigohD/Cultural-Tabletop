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
using Cultiverse.UI;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for CreateWorldView.xaml
    /// </summary>
    public partial class CreateWorldView : UserControl
    {

        public event RoutedEventHandler CreateWorldDone;

        public World currentWorld;
        Planet planet;

        Dispatcher mainDespatch;

        ArrayList updateList = new ArrayList();
        float deltaTime;
        Stopwatch watch = new Stopwatch();
        Image bg = new Image();


        public CreateWorldView()
        {
            InitializeComponent();

            mainDespatch = Dispatcher.CurrentDispatcher;

            CompositionTarget.Rendering += update;

            initBackground();
        }

        private void initBackground()
        {
            Stars background = new Stars("bg.png", 0.03f);
            Stars stars = new Stars("stars.png", 0.1f);
            Stars stars2 = new Stars("stars2.png", 0.1f);
            Stars stars3 = new Stars("stars3.png", 0.1f);
            addToUpdate(background);
            addToUpdate(stars);
            addToUpdate(stars2);
            addToUpdate(stars3);

            myCanvas.Children.Add(background);
            myCanvas.Children.Add(stars);
            myCanvas.Children.Add(stars2);
            myCanvas.Children.Add(stars3);
        }

        int count = 0;
        float rotation, rotSpeed = 0.4f, alphaCount = 0;
        SolidColorBrush solidC = new SolidColorBrush();
        byte r;
        public void update(object sender, EventArgs e)
        {
            watch.Stop();
            deltaTime = watch.ElapsedMilliseconds;

            foreach (Updateable u in updateList)
                u.update(deltaTime);

            rotation += rotSpeed / 100.00000f * deltaTime;
            Matrix matrix = ((MatrixTransform)createWorldText.RenderTransform).Matrix;
            matrix.RotateAt(rotSpeed / 100.00000f * deltaTime, 500, 500);
            createWorldText.RenderTransform = new MatrixTransform(matrix);

            alphaCount += 0.02f;
            createWorldText.Opacity = (Math.Sin(alphaCount) * 0.5f) + 0.5f;

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

        public void setWorld(World world)
        {
            currentWorld = world;
            removeFromUpdate(planet);
            myCanvas.Children.Remove(planet);

            planet = new Planet(0, 0, 1.0f, currentWorld, 0);
            planetCanvas.Children.Add(planet);
            addToUpdate(planet);

            restart();
        }

        void restart()
        {
            
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

            Ball ball = new Ball(count, (int)(800 / 2 - 64), (int)(800 / 2 - 64), 128, 128, drawing, true, 800);
            planet.addBall(ball);

            inkCanvas.Strokes.Clear();
        }

        private void drawingDone(object sender, RoutedEventArgs e)
        {
            this.addDrawingFromInkCanvas((SurfaceInkCanvas)sender);
        }

        private bool _saveCheck1Checked = false;
        private bool saveCheck1Checked
        {
            get
            {
                return _saveCheck1Checked;
            }
            set
            {
                _saveCheck1Checked = value;
                if (_saveCheck1Checked)
                {
                    saveCheck1Border.Background = new SolidColorBrush(Colors.GreenYellow);
                }
                else
                {
                    saveCheck1Border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        private bool _saveCheck2Checked = false;
        private bool saveCheck2Checked
        {
            get
            {
                return _saveCheck2Checked;
            }
            set
            {
                _saveCheck2Checked = value;
                if (_saveCheck2Checked)
                {
                    saveCheck2Border.Background = new SolidColorBrush(Colors.GreenYellow);
                }
                else
                {
                    saveCheck2Border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        private bool _saveCheck3Checked = false;
        private bool saveCheck3Checked
        {
            get
            {
                return _saveCheck3Checked;
            }
            set
            {
                _saveCheck3Checked = value;
                if (_saveCheck3Checked)
                {
                    saveCheck3Border.Background = new SolidColorBrush(Colors.GreenYellow);
                }
                else
                {
                    saveCheck3Border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }
        private bool _saveCheck4Checked = false;
        private bool saveCheck4Checked
        {
            get
            {
                return _saveCheck4Checked;
            }
            set
            {
                _saveCheck4Checked = value;
                if (_saveCheck4Checked)
                {
                    saveCheck4Border.Background = new SolidColorBrush(Colors.GreenYellow);
                }
                else
                {
                    saveCheck4Border.Background = new SolidColorBrush(Colors.Transparent);
                }
            }
        }


        private void saveCheck1_TouchDown(object sender, TouchEventArgs e)
        {
            saveCheck1Checked = true;
            trySave();
        }
        private void saveCheck1_TouchUp(object sender, TouchEventArgs e)
        {
            saveCheck1Checked = false;
        }
        private void saveCheck2_TouchDown(object sender, TouchEventArgs e)
        {
            saveCheck2Checked = true;
            trySave();
        }
        private void saveCheck2_TouchUp(object sender, TouchEventArgs e)
        {
            saveCheck2Checked = false;
        }
        private void saveCheck3_TouchDown(object sender, TouchEventArgs e)
        {
            saveCheck3Checked = true;
            trySave();
        }
        private void saveCheck3_TouchUp(object sender, TouchEventArgs e)
        {
            saveCheck3Checked = false;
        }
        private void saveCheck4_TouchDown(object sender, TouchEventArgs e)
        {
            saveCheck4Checked = true;
            trySave();
        }
        private void saveCheck4_TouchUp(object sender, TouchEventArgs e)
        {
            saveCheck4Checked = false;
        }
        private void trySave()
        {
            if (drawingSpace1.Visibility == Visibility.Visible && !saveCheck1Checked)
            {
                return;
            }
            if (drawingSpace2.Visibility == Visibility.Visible && !saveCheck2Checked)
            {
                return;
            }
            if (drawingSpace3.Visibility == Visibility.Visible && !saveCheck3Checked)
            {
                return;
            }
            if (drawingSpace4.Visibility == Visibility.Visible && !saveCheck4Checked)
            {
                return;
            }

            Console.WriteLine("Save!");
            if (CreateWorldDone != null)
            {
                MainWindow.worldCreated = false;
                CreateWorldDone(this, new RoutedEventArgs());
            }
        }




    }

}
