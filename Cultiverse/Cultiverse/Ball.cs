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
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;

namespace Cultiverse
{
    class Ball
    {
        Image image;
        public float x, y, vX, vY;

        public Ball(int count)
        {
            image = new Image();

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(@"C:\Users\DigohD\Documents\GitHub\Cultural-Tabletop\Cultiverse\Cultiverse\Resources\particle1.png", UriKind.Absolute);
            bi3.EndInit();

            image.Stretch = Stretch.Fill;
            image.Source = bi3;
            image.Name = "image" + count;
            image.Width = 32;

            Random rnd = new Random();

            x = rnd.Next(800);
            y = rnd.Next(800);
            vX = (float) (rnd.NextDouble() * 3) + 1;
            vY = (float) (rnd.NextDouble() * 3) + 1;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
        }

        public Ball(int count, int newX, int newY)
        {
            image = new Image();

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(@"C:\Users\DigohD\Documents\GitHub\Cultural-Tabletop\Cultiverse\Cultiverse\Resources\particle1.png", UriKind.Absolute);
            bi3.EndInit();

            image.Stretch = Stretch.Fill;
            image.Source = bi3;
            image.Name = "image" + count;
            image.Width = 32;

            Random rnd = new Random();

            x = newX;
            y = newY;
            vX = (float)(rnd.NextDouble() * 3) + 1;
            vY = (float)(rnd.NextDouble() * 3) + 1;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
        }

        public Image getBallImage()
        {
            return image;
        }

        public void update(Dispatcher dispatch){
            dispatch.BeginInvoke((Action)(() =>
            {
                collideWall();

                x += vX;
                y += vY;

                Canvas.SetLeft(image, x);
                Canvas.SetTop(image, y);
            }));
        }

        public void collide(Ball other)
        {
            if (this.Equals(other))
                return;

            Vector pThis = new Vector(x, y);
            Vector pOther = new Vector(other.x, other.y);

            Vector vThis = new Vector(vX, vY);
            Vector vOther = new Vector(other.vX, other.vY);

            if (Vector.Subtract(pThis, pOther).Length <= 32.0000f)
            {
                Debug.WriteLine("Col: Length " + Vector.Subtract(pThis, pOther).Length);

                double angle = Vector.AngleBetween(vThis, vOther);
                Vector rv = RotateVector2d(vThis, 90 + angle);
                vX = (float) rv.X;
                vY = (float) rv.Y;

                return;
            }
        }

        private void collideWall()
        {
            Debug.WriteLine("wallCol " + x + " : " + y);
            if(x >= 768)
                vX = -vX;
            if(x <= 0)
                vX = -vX;
            if (y >= 768)
                vY = -vY;
            if(y <= 0)
                vY = -vY;
        }

        static Vector RotateVector2d(Vector v, double degrees)
        {
            double[] result = new double[2];
            v.X = v.X * Math.Cos((Math.PI / 180) * degrees) - v.Y * Math.Sin((Math.PI / 180) * degrees);
            v.Y = v.X * Math.Sin((Math.PI / 180) * degrees) + v.Y * Math.Cos((Math.PI / 180) * degrees);
            return v;
        }

    }
}
