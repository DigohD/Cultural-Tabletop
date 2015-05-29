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
using Microsoft.Surface.Presentation.Input;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for TokenSensor.xaml
    /// </summary>
    public partial class TokenSensor : UserControl
    {
        public event RoutedEventHandler TokenDown;
        public event RoutedEventHandler TokenUp;

        public TokenSensor()
        {
            InitializeComponent();
            CompositionTarget.Rendering += update;
        }

        private void ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            // Capture the touch device and handle the event 
            IInputElement element = sender as IInputElement;
            if (element != null && e.Device.Capture(element))
            {
                TokenDown(sender, e);
                e.Handled = true;
            }
        }

        private void ellipse_LostTouchCapture(object sender, TouchEventArgs e)
        {
            TokenUp(sender, e);
        }


        float sizeMul, ticker;
        public void update(object sender, EventArgs e)
        {
            sizeMul = (float)Math.Abs(Math.Sin(ticker++ / 100) * 0.2f) + 0.8f;
            Matrix sizingMatrix = Matrix.Identity;
            sizingMatrix.ScaleAt(sizeMul, sizeMul, ellipse.Width / 2, ellipse.Height / 2);

            ellipse.RenderTransform = new MatrixTransform(sizingMatrix);
        }
    }
}
