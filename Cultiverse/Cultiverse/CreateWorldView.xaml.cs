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

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for CreateWorldView.xaml
    /// </summary>
    public partial class CreateWorldView : UserControl
    {
        private World currentWorld;

        public CreateWorldView()
        {
            InitializeComponent();
        }

        private void saveWorldButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addDrawingButton_Click(object sender, RoutedEventArgs e)
        {
            WorldDrawing drawing = currentWorld.createNewDrawing();

            //Save strokes
            FileStream fileStream = new FileStream(drawing.StrokesFilePath, FileMode.Create, FileAccess.Write);
            inkCanvas1.Strokes.Save(fileStream);
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
            world.Child = image;

            inkCanvas1.Strokes.Clear();
        }


        internal void setWorld(World world)
        {
            currentWorld = world;
        }
    }

}
