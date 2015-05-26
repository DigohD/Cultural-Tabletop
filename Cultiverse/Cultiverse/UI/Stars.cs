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
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Collections;
using System.Diagnostics;

namespace Cultiverse.UI
{
    public class Stars : Image, Updateable
    {
        BitmapImage bitMap;
        float rotspeed;

        public Stars(string name, float newRotSpeed)
        {
            this.rotspeed = newRotSpeed;

            bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"Resources\" + name, UriKind.Relative);
            bitMap.EndInit();

            this.Width = 3000;
            this.Height = 3000;

            this.Stretch = Stretch.UniformToFill;
            this.Source = bitMap;

            this.VerticalAlignment = VerticalAlignment.Center;
            this.HorizontalAlignment = HorizontalAlignment.Center;
        }

        float rotation;

        public void update(float deltatime)
        {
            rotation += rotspeed / 100.00000f * deltatime;

            Matrix matrix = ((MatrixTransform)this.RenderTransform).Matrix;

            matrix.RotateAt(rotspeed / 100.00000f * deltatime, 1500, 1500);
            /*
            rotateTransform1.CenterX = 1500;
            rotateTransform1.CenterY = 1500;
            rotateTransform1.Angle = rotation;
            this.RenderTransform = rotateTransform1;
             * */

            this.RenderTransform = new MatrixTransform(matrix);
        }
    }
}
