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
using System.Timers;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for TokenSensor.xaml
    /// </summary>
    public partial class TokenSensor : UserControl
    {
        public event RoutedEventHandler TokenDown;
        public event RoutedEventHandler TokenUp;

        private bool _tokenDown = false;
        public bool IsTokenDown
        {
            get
            {
                return _tokenDown;
            }
        }

        private Timer holdTimer;
        private Timer releaseTimer;

        public TokenSensor()
        {
            InitializeComponent();
            CompositionTarget.Rendering += update;

            holdTimer = new Timer(1000);
            holdTimer.AutoReset = false;
            holdTimer.Elapsed += new ElapsedEventHandler(holdTimer_Elapsed);

            releaseTimer = new Timer(1000);
            releaseTimer.AutoReset = false;
            releaseTimer.Elapsed += new ElapsedEventHandler(releaseTimer_Elapsed);
        }

        void holdTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TokenDown(this, new RoutedEventArgs());
            })
            );
        }

        void releaseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                TokenUp(this, new RoutedEventArgs());
            })
            );
        }

        private void ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            // Capture the touch device and handle the event 
            IInputElement element = sender as IInputElement;
            if (element != null && e.Device.Capture(element))
            {
                releaseTimer.Stop();
                holdTimer.Stop();
                holdTimer.Start();

                _tokenDown = true;
                label1.Visibility = System.Windows.Visibility.Hidden;
                label2.Visibility = System.Windows.Visibility.Hidden;
                ellipse.Fill = new SolidColorBrush(Colors.White);
                e.Handled = true;
            }
        }

        private void ellipse_LostTouchCapture(object sender, TouchEventArgs e)
        {
            holdTimer.Stop();
            releaseTimer.Stop();
            releaseTimer.Start();

            _tokenDown = false;
            label1.Visibility = System.Windows.Visibility.Visible;
            label2.Visibility = System.Windows.Visibility.Visible;
            Color stdColor = new Color();
            stdColor.A = 0xAA;
            stdColor.R = 0x1A;
            stdColor.G = 0x12;
            stdColor.B = 0x22;
            ellipse.Fill = new SolidColorBrush(stdColor);
        }


        float sizeMul, ticker;
        public void update(object sender, EventArgs e)
        {
            if (_tokenDown)
            {
                sizeMul = 1.0f;
            }
            else
            {
                sizeMul = (float)Math.Abs(Math.Sin(ticker++ / 100) * 0.2f) + 0.8f;
            }
            Matrix sizingMatrix = Matrix.Identity;
            sizingMatrix.ScaleAt(sizeMul, sizeMul, ellipse.Width / 2, ellipse.Height / 2);

            ellipse.RenderTransform = new MatrixTransform(sizingMatrix);
        }
    }
}
