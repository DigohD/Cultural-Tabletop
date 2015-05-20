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
using Microsoft.Surface.Presentation.Controls;

namespace Cultiverse
{
    /// <summary>
    /// Interaction logic for DrawingSpace.xaml
    /// </summary>
    public partial class DrawingSpace : UserControl
    {
        public event RoutedEventHandler DrawingDone;

        public DrawingSpace()
        {
            InitializeComponent();

            inkCanvas.DefaultDrawingAttributes.Color = Colors.Black;
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
                    brushSelectButton.Content = BrushIcon;

                }
                else
                {
                    inkCanvas.EditingMode = SurfaceInkEditingMode.EraseByPoint;

                    // Update button image to show that we are now in erase mode.
                    brushSelectButton.Content = FindResource("RubberIcon");
                }

            }
        }

        private void brushSelectButton_Click(object sender, RoutedEventArgs e)
        {
            this.CurrentEditingMode = this.CurrentEditingMode == SurfaceInkEditingMode.Ink ? SurfaceInkEditingMode.EraseByPoint : SurfaceInkEditingMode.Ink;
        }

        private void colorPick_Click(object sender, RoutedEventArgs e)
        {
            SurfaceButton sb = (SurfaceButton)sender;

            inkCanvas.DefaultDrawingAttributes.Color = ((SolidColorBrush)sb.Background).Color;
        }

        private void addDrawingButton_Click(object sender, RoutedEventArgs e)
        {
            if (DrawingDone != null)
            {
                DrawingDone(inkCanvas, e);
            }
        }
    }
}
