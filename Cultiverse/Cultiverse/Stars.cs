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

namespace Cultiverse
{
    class Stars : Updateable
    {
        Image starsImage = new Image();
        BitmapImage bitMap;
        Canvas canvas;
        float rotspeed;

        public Stars(Canvas newCanvas, string name, float newRotSpeed)
        {
            this.canvas = newCanvas;
            this.rotspeed = newRotSpeed;

            bitMap = new BitmapImage();
            bitMap.BeginInit();
            bitMap.UriSource = new Uri(@"C:\Users\DigohD\Documents\GitHub\Cultural-Tabletop\Cultiverse\Cultiverse\Resources\" + name, UriKind.Absolute);
            bitMap.EndInit();

            starsImage.Stretch = Stretch.Fill;
            starsImage.Source = bitMap;

            canvas.Children.Add(starsImage);
            Canvas.SetLeft(starsImage, 0);
            Canvas.SetTop(starsImage, 0);
        }

        float rotation;
        RotateTransform rotateTransform1 = new RotateTransform();

        public override void update(float deltatime)
        {
            rotation += rotspeed / 100.00000f * deltatime;
            rotateTransform1.CenterX = 800;
            rotateTransform1.CenterY = 450;
            rotateTransform1.Angle = rotation;
            starsImage.RenderTransform = rotateTransform1;
        }
    }
}
