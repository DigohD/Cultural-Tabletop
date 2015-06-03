using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.ComponentModel;
using Cultiverse.UI;
using System.IO;
using System.Windows.Ink;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for DrawingSpace.xaml
    /// </summary>
    /// 

    public partial class DrawingSpace : UserControl
    {
        public event RoutedEventHandler DrawingDone;

        private int currentDrawingI = 0;

        private List<string> drawingNames = new List<string>();

        public DrawingSpace()
        {
            InitializeComponent();

            inkCanvas.DefaultDrawingAttributes.Color = Colors.Black;

            Storyboard sb = (Storyboard)FindResource("pulseAnimation");
            sb.Begin();

            drawingNames.Add("FAVOURITE ANIMAL");
            drawingNames.Add("A VITAL ITEM");
            drawingNames.Add("THE BEST QUALITY");
            drawingNames.Add("LIFE");
            drawingNames.Add("NEAT THINGS");

            this.Reset();

            TouchDragDrop.Drop += new EventHandler<TouchEventArgs>(TouchDragDrop_Drop);
            TouchDragDrop.Drag += new EventHandler<TouchEventArgs>(TouchDragDrop_Drag);
        }

        public void Show()
        {
            if (this.Visibility != Visibility.Visible)
            {
                Storyboard sb = (Storyboard)FindResource("scaleUp");
                sb.Completed += delegate
                {
                    this.IsHitTestVisible = true;
                    this.Visibility = Visibility.Visible;
                };
                sb.Begin();
                this.Visibility = Visibility.Visible;
            }
        }

        public void Hide()
        {

            if (this.Visibility != Visibility.Hidden)
            {
                this.Visibility = Visibility.Visible;
                this.IsHitTestVisible = false;
                Storyboard sb = (Storyboard)FindResource("scaleDown");
                sb.Completed += delegate
                {
                    this.Visibility = Visibility.Hidden;
                };
                sb.Begin();
            }
        }

        void TouchDragDrop_Drag(object sender, TouchEventArgs e)
        {
            Point touchPoint = e.GetTouchPoint(inkCanvas).Position;

            if (this.Visibility == Visibility.Visible && touchPoint.X > 0 && touchPoint.Y > 0 && touchPoint.X < inkCanvasBorder.ActualWidth && touchPoint.Y < inkCanvasBorder.ActualHeight)
            {
                if (sender is Ball)
                {
                    Ball ball = (Ball)sender;
                    inkCanvasBorder.Background = new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                Color color = new Color();
                color.A = 0xAA;
                color.R = 0xFF;
                color.G = 0xFF;
                color.B = 0xFF;
                inkCanvasBorder.Background = new SolidColorBrush(color);
            }
        }

        void TouchDragDrop_Drop(object sender, TouchEventArgs e)
        {
            Point touchPoint = e.GetTouchPoint(inkCanvas).Position;

            if (this.Visibility == Visibility.Visible && touchPoint.X > 0 && touchPoint.Y > 0 && touchPoint.X < inkCanvasBorder.ActualWidth && touchPoint.Y < inkCanvasBorder.ActualHeight)
            {
                if (sender is Ball)
                {
                    Ball ball = (Ball)sender;
                    var fs = new FileStream(ball.Drawing.StrokesFilePath, FileMode.Open, FileAccess.Read);
                    StrokeCollection strokes = new StrokeCollection(fs);
                    inkCanvas.Strokes = strokes;
                    ball.Dropped = true;
                }
            }

            Color color = new Color();
            color.A = 0xAA;
            color.R = 0xFF;
            color.G = 0xFF;
            color.B = 0xFF;
            inkCanvasBorder.Background = new SolidColorBrush(color);
        }

        public void Reset()
        {
            currentDrawingI = 0;
            drawLabel.Content = drawingNames[currentDrawingI];
        }

        /// <summary>
        /// The current editing mode for the DrawingPadCanvas. 
        /// When set, it updates the UI accordingly.
        /// </summary>
        private SurfaceInkEditingMode CurrentEditingMode
        {
            get
            {
                return inkCanvas.EditingMode;
            }
            set
            {
                if (value == SurfaceInkEditingMode.Ink)
                {
                    inkCanvas.EditingMode = SurfaceInkEditingMode.Ink;

                    // Update button image to show that we are now in ink mode.
                    BrushBorder.Background = new SolidColorBrush(inkCanvas.DefaultDrawingAttributes.Color);
                    RubberBorder.Background = new SolidColorBrush(Colors.Transparent);
                    rubberSelectButton.Content = RubberIcon;
                }
                else
                {
                    inkCanvas.EditingMode = SurfaceInkEditingMode.EraseByPoint;

                    // Update button image to show that we are now in erase mode.
                    RubberBorder.Background = new SolidColorBrush(Colors.White);
                    BrushBorder.Background = new SolidColorBrush(Colors.Transparent);
                    
                    rubberSelectButton.Content = FindResource("RubberIconBlack");
                }

            }
        }

        private void brushSelectButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentEditingMode = SurfaceInkEditingMode.Ink;
        }

        private void rubberSelectButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentEditingMode = SurfaceInkEditingMode.EraseByPoint;
        }

        private void colorPick_Click(object sender, RoutedEventArgs e)
        {
            SurfaceButton sb = (SurfaceButton)sender;

            inkCanvas.DefaultDrawingAttributes.Color = ((SolidColorBrush)sb.Background).Color;
        }

        private void addDrawingButton_Click(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.Strokes.Count() > 0)
            {
                if (++currentDrawingI < drawingNames.Count)
                {
                    drawLabel.Content = drawingNames[currentDrawingI];
                }
                if (DrawingDone != null)
                {
                    DrawingDone(inkCanvas, e);
                }
            }
        }

        /// <summary>
        /// Gets the color of a specific pixel.
        /// </summary>
        /// <param name="pt">The point from which to get a color.</param>
        /// <returns>The color of the point.</returns>
        private System.Windows.Media.Color GetPixelColor(InputDevice inputDevice)
        {
            // Translate the input point to bitmap coordinates
            double transformFactor = ColorWheel.Source.Width / ColorWheel.ActualWidth;
            Point inputPoint = inputDevice.GetPosition(ColorWheel);
            Point bitmapPoint = new Point(inputPoint.X * transformFactor, inputPoint.Y * transformFactor);

            // The point is outside the color wheel. Return black.
            if (bitmapPoint.X < 0 || bitmapPoint.X >= ColorWheel.Source.Width ||
                bitmapPoint.Y < 0 || bitmapPoint.Y >= ColorWheel.Source.Height)
            {
                return Colors.Black;
            }

            // The point is inside the color wheel. Find the color at the point.
            CroppedBitmap cb = new CroppedBitmap(ColorWheel.Source as BitmapSource, new Int32Rect((int)bitmapPoint.X, (int)bitmapPoint.Y, 1, 1));
            byte[] pixels = new byte[4];
            cb.CopyPixels(pixels, 4, 0);
            return Color.FromRgb(pixels[2], pixels[1], pixels[0]);
        }

        private void OnColorWheelLostMouseCapture(object sender, MouseEventArgs e)
        {
            HandleColorWheelLostCapture(sender, e);
        }

        private void OnColorWheelLostTouchCapture(object sender, TouchEventArgs e)
        {
            HandleColorWheelLostCapture(sender, e);
        }


        private void HandleColorWheelLostCapture(object sender, InputEventArgs args)
        {
            UIElement element = sender as UIElement;
            // If there are input devices captured to the color wheel,
            // choose a color based on the position of the last touch device
            if (!element.GetAreAnyInputDevicesCaptured())
            {
                // Select a color if hit, but keep the color wheel open if not
                if (ChooseColor(args, true))
                {
                }
            }
        }


        /// <summary>
        /// Select a color based on the postion of args.TouchDevice if hit.
        /// </summary>
        /// <param name="args">The arguments for the input event.</param>
        /// <param name="closeOnlyOnHit">Indicates if the ColorWheel should
        /// be kept open when an actual color is chosen.</param>
        /// <returns> true if a color was actually chosen.</returns>
        private bool ChooseColor(InputEventArgs args, bool closeOnlyOnHit)
        {
            // If the color wheel is not visible, bail out
            if (ColorWheel.Visibility == Visibility.Hidden)
            {
                return false;
            }

            // Set the color on the CurrentColor indicator and on the SurfaceInkCanvas
            Color color = GetPixelColor(args.Device);

            // Black means the user touched the transparent part of the wheel. In that 
            // case, leave the color set to its current value
            bool hit = color != Colors.Black;

            if (hit)
            {
                inkCanvas.DefaultDrawingAttributes.Color = color;
                CurrentEditingMode = SurfaceInkEditingMode.Ink;
            }

            args.Handled = true;
            return hit;
        }

        private void Border_LostMouseCapture(object sender, MouseEventArgs e)
        {
            e.Handled = false;
        }

        private void Border_LostTouchCapture(object sender, TouchEventArgs e)
        {
            e.Handled = false;
        }

        private void inkCanvas_LostMouseCapture(object sender, MouseEventArgs e)
        {
            e.Handled = false;
        }

        private void inkCanvas_LostTouchCapture(object sender, TouchEventArgs e)
        {
            e.Handled = false;
        }

        private void ColorWheel_TouchDown(object sender, TouchEventArgs e)
        {
            // Capture the touch device and handle the event 
            IInputElement element = sender as IInputElement;
            if (element != null && e.Device.Capture(element))
            {
                e.Handled = true;
            }

        }

        private void ColorWheel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Capture the touch device and handle the event 
            IInputElement element = sender as IInputElement;
            if (element != null && e.Device.Capture(element))
            {
                e.Handled = true;
            }
        }

    }
}
