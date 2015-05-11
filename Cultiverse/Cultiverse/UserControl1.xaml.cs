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
using System.Windows.Ink;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
            FileStream fileStream = new FileStream(@"drawing.strokes", FileMode.Open);

            DrawingPadCanvas.Strokes = new StrokeCollection(fileStream);
            fileStream.Close();
        }

        private void OnStrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
        }

        private void UserControl_TouchDown(object sender, TouchEventArgs e)
        {
            this.Background = new SolidColorBrush(Colors.Red);

            //Save strokes
            FileStream fileStream = new FileStream(@"drawing.strokes", FileMode.Create, FileAccess.Write);

            DrawingPadCanvas.Strokes.Save(fileStream);
            fileStream.Close();
        }
    }
}
