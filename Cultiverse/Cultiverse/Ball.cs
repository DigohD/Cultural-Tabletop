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
using System.Collections;
using Cultiverse.Model;

namespace Cultiverse
{
    class Ball : Updateable
    {
        Image image;
        WorldDrawing drawing;
        public float x, y, vX, vY;
        
        public int width, height;
        public float spring = 0.001f, friction = 0.995f, gravity = 0.0002f, inertia = 0.0005f, wallDampening = 0.65f;

        bool isPushEnabled;

        public Ball(int count)
        {
            image = new Image();

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(@"Resources\particle1.png", UriKind.Relative);
            bi3.EndInit();

            image.Stretch = Stretch.Fill;
            image.Source = bi3;
            image.Name = "image" + count;
            image.Width = 32;

            Random rnd = new Random();

            x = rnd.Next(800);
            y = rnd.Next(800);
            vX = (float) ((rnd.NextDouble() * 3) + 1) / 100.00000f;
            vY = (float) ((rnd.NextDouble() * 3) + 1) / 100.00000f;

            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
        }

        public Ball(int count, int newX, int newY, int newWidth, int newHeight, WorldDrawing drawing)
        {
            //this(count,newX,newY,newWith,newHeight,
                
            this.width = newWidth;
            this.height = newHeight;

            isPushEnabled = true;

            image = new Image();

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            if (drawing != null)
            {
                bi3.UriSource = new Uri(drawing.BitmapFilePath, UriKind.Absolute);
            }
            else
            {
                bi3.UriSource = new Uri(@"Resources\particle1.png", UriKind.Relative);
            }
            bi3.EndInit();

            image.Stretch = Stretch.Fill;
            image.Source = bi3;
            image.Name = "image" + count;
            image.Width = width;
            image.Height = height;

            Random rnd = new Random();

            x = newX + width / 2;
            y = newY + height / 2;
            vX = (float)((rnd.NextDouble() * 3) + 1.000000f) / 10.00000f;
            vY = (float)((rnd.NextDouble() * 3) + 1.000000f) / 10.00000f;
            vX = vX / 10.000000f;
            vY = vY / 10.000000f;

            Canvas.SetLeft(image, x - width / 2);
            Canvas.SetTop(image, y - height / 2);
        }

        public Image getBallImage()
        {
            return image;
        }

        public override void update(float deltaTime){
            collideWall(deltaTime);

            move(deltaTime);
        }

        public void setVelocity(float vX, float vY)
        {
            this.vX = vX;
            this.vY = vY;
        }

        public void push(float pushX, float pushY, float touchX, float touchY)
        {
            if (!isPushEnabled)
                return;
            float dx = touchX - x;
            float dy = touchY - y;
            float distance = (float) Math.Sqrt(dx * dx + dy * dy);
            if(distance < width){
                float distMul = width / distance;
                vX += pushX * inertia;
                vY += pushY * inertia;
            }
        }

        private void move(float deltaTime)
        {
            vX *= friction;
            vY *= friction;

            x += vX * deltaTime;
            y += vY * deltaTime;

            float modX = x - 1920/2;
            float modY = y - 1080/2;
            if (modX > 0)
                vX -= gravity;
            else if (modX < 0)
                vX += gravity;
            if (modY > 0)
                vY -= gravity;
            else if (modY < 0)
                vY += gravity;


            Canvas.SetLeft(image, x - width / 2);
            Canvas.SetTop(image, y - height / 2);
        }

        public void collide(ArrayList others)
        {
            int numBalls = others.Count;
            foreach(Ball ball in others)
            {
                if (ball.Equals(this))
                    continue;
                float dx = ball.x - x;
                float dy = ball.y - y;
                float distance = (float) Math.Sqrt(dx * dx + dy * dy);
                float minDist = ball.width / 2 + width / 2;
                if (distance < minDist)
                {
                    float angle = (float) Math.Atan2(dy, dx);
                    float targetX = x + (float) Math.Cos(angle) * minDist;
                    float targetY = y + (float) Math.Sin(angle) * minDist;
                    float ax = (targetX - ball.x) * spring;
                    float ay = (targetY - ball.y) * spring;
                    vX -= ax;
                    vY -= ay;
                    ball.vX += ax;
                    ball.vY += ay;
                }
            }
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
                double angle = Vector.AngleBetween(vThis, vOther);
                Vector rv = RotateVector2d(vThis, 90 + angle);
                vX = (float) rv.X;
                vY = (float) rv.Y;

                return;
            }
        }

        private void collideWall(float deltaTime)
        {
            float modX, modY;
            modX = x - 1920/2;
            modY = y - 1080/2;
            if (Math.Sqrt((modX * modX) + (modY * modY)) > 400)
            {
                vX = -vX * wallDampening;
                vY = -vY * wallDampening;
                move(deltaTime);
                move(deltaTime);
            }
            
            /* Square Shaped Col
             * 
             * if(x >= 768)
                vX = -vX;
            if(x <= 0)
                vX = -vX;
            if (y >= 768)
                vY = -vY;
            if(y <= 0)
                vY = -vY;*/
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
