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

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for CreateWorldView.xaml
    /// </summary>
    public partial class CreateWorldView : UserControl
    {
        public CreateWorldView()
        {
            InitializeComponent();
        }

        private void saveWorldButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void addDrawingButton_Click(object sender, RoutedEventArgs e)
        {
            //Save strokes
            FileStream fileStream = new FileStream(@"drawing.strokes", FileMode.Create, FileAccess.Write);

            inkCanvas1.Strokes.Save(fileStream);
            fileStream.Close();
        }

    }
}
