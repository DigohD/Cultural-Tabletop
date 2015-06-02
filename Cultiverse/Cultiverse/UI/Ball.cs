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

namespace Cultiverse.UI
{
    public class Ball : Image, Updateable
    {
        public float x, y, vX, vY;
        
        public int width, height;
        public float spring = 0f, maxSpring = 0.001f, friction = 0.995f, gravity = 0.0005f, inertia = 0.0005f, wallDampening = 0.65f, colWidthMod = 0.7f;

        bool isPushEnabled;
        bool touchMove;

        private float containerSize = 800; //Size of planet (containing circle)

        public WorldDrawing Drawing;

        public bool Dropped;

        public event EventHandler<TouchEventArgs> OnDrop;

        public Ball(int count, int newX, int newY, int newWidth, int newHeight, WorldDrawing drawing, bool isPushEnabled, int containerSize)
        {
            this.width = newWidth;
            this.height = newHeight;
            this.isPushEnabled = isPushEnabled;
            this.containerSize = containerSize;

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            this.Drawing = drawing;
            if (drawing != null)
            {
                bi3.UriSource = new Uri(drawing.BitmapFilePath, UriKind.Absolute);
            }
            else
            {
                bi3.UriSource = new Uri(@"Resources\particle1.png", UriKind.Relative);
            }
            bi3.CacheOption = BitmapCacheOption.OnLoad;
            bi3.EndInit();

            this.Stretch = Stretch.Fill;
            this.Source = bi3;
            this.Name = "image" + count;
            this.Width = width;
            this.Height = height;

            Random rnd = new Random();

            x = newX + (width / 2);
            y = newY + (height / 2);

            vX = (float)((rnd.NextDouble() * 3) + 1.000000f) / 10.00000f;
            vY = (float)((rnd.NextDouble() * 3) + 1.000000f) / 10.00000f;
            vX = vX / 10.000000f;
            vY = vY / 10.000000f;

            Canvas.SetLeft(this, x - width / 2);
            Canvas.SetTop(this, y - height / 2);


            this.IsHitTestVisible = true;

            this.TouchDown += new EventHandler<TouchEventArgs>(Ball_TouchDown);
            this.TouchMove += new EventHandler<TouchEventArgs>(Ball_TouchMove);
            this.TouchUp += new EventHandler<TouchEventArgs>(Ball_TouchUp);
            this.TouchLeave += new EventHandler<TouchEventArgs>(Ball_TouchLeave);
        }

        void Ball_TouchLeave(object sender, TouchEventArgs e)
        {
            if (touchMove)
            {
                touchMove = false;
                this.vX = (this.x - this.lastTouchMoveX) / 40f;
                this.vY = (this.y - this.lastTouchMoveY) / 40f;
            }
        }

        void Ball_TouchUp(object sender, TouchEventArgs e)
        {
            if (touchMove)
            {
                touchMove = false;
                this.vX = (this.x - this.lastTouchMoveX) / 40f;
                this.vY = (this.y - this.lastTouchMoveY) / 40f;
                EventArgs args = new EventArgs();
                TouchDragDrop.FireDrop(this, e);
                if (Dropped)
                {
                    if (OnDrop != null)
                    {
                        OnDrop(this, e);
                    }
                }
            }

        }


        float lastTouchMoveX = 0.0f;
        float lastTouchMoveY = 0.0f;
        void Ball_TouchMove(object sender, TouchEventArgs e)
        {
            if (touchMove)
            {
                lastTouchMoveX = this.x;
                lastTouchMoveY = this.y;
                this.x = (float)e.GetTouchPoint((IInputElement)this.Parent).Position.X;
                this.y = (float)e.GetTouchPoint((IInputElement)this.Parent).Position.Y;

                Canvas.SetLeft(this, x - width / 2);
                Canvas.SetTop(this, y - height / 2);

                DataObject data = new DataObject();
                data.SetData("Object", this);

                TouchDragDrop.FireDrag(this, e);
            }
        }

        void Ball_TouchDown(object sender, TouchEventArgs e)
        {
            touchMove = true;
        }

        public void update(float deltaTime){
            if (!touchMove)
            {
                collideWall(deltaTime);

                move(deltaTime);

                spring += 0.00001f;
                if (spring >= maxSpring)
                    spring = maxSpring;
            }
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

        public void pushSimple(float pushX, float pushY)
        {
            vX += pushX * inertia / 5;
            vY += pushY * inertia / 5;
        }

        private void move(float deltaTime)
        {
            vX *= friction;
            vY *= friction;

            x += vX * deltaTime;
            y += vY * deltaTime;

            float modX = x - (containerSize / 2);
            float modY = y - (containerSize / 2);
           

            if (modX > 0)
                vX -= gravity;
            else if (modX < 0)
                vX += gravity;
            if (modY > 0)
                vY -= gravity;
            else if (modY < 0)
                vY += gravity;
            
            Canvas.SetLeft(this, x - width / 2);
            Canvas.SetTop(this, y - height / 2);
            
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
                float minDist = ball.width * colWidthMod / 2 + width * colWidthMod / 2;
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
            float modX = 0, modY = 0;
            modX = x - (containerSize / 2);
            modY = y - (containerSize / 2);

            if (Math.Sqrt((modX * modX) + (modY * modY)) > containerSize / 2 - (width * colWidthMod * colWidthMod))
            {
                saveFromVacuum();
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

        private void saveFromVacuum()
        {
            float modX = x - (containerSize / 2);
            float modY = y - (containerSize / 2);

            if (modX > 0)
                vX -= gravity * 20;
            else if (modX < 0)
                vX += gravity * 20;
            if (modY > 0)
                vY -= gravity * 20;
            else if (modY < 0)
                vY += gravity * 20;
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
