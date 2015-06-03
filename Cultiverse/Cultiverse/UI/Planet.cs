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
using Cultiverse.Model;
using System.Globalization;
using System.Collections;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;
using Microsoft.Surface.Presentation.Controls;


namespace Cultiverse.UI
{
    public class Planet : Canvas, Updateable
    {
        public float posX, posY, width, height, ballXoffset, ballYoffset, viewOffsetX, viewOffsetY;

        float lastX, lastY;

        float scaleFactor;
        int planetID;

        ArrayList ballList = new ArrayList();
        ArrayList updateList = new ArrayList();

        World world;

        public Image planet = new Image();

        public Planet(float newX, float newY, float newScale, World newWorld, int newID) : base()
        {
            posX = newX;
            posY = newY;
            scaleFactor = newScale;
            planetID = newID;

            world = newWorld;

            ballXoffset = 0;
            ballYoffset = 0;

            this.Width = 800;
            this.Height = 800;

            this.RenderTransformOrigin = new Point(0.5, 0.5);

            initPlanet();

            setToScale(scaleFactor);

            foreach (WorldDrawing d in world.getDrawings())
            {
                Ball ball = new Ball(1, (int)(800 / 2 - 64), (int)(800 / 2 - 64), 128, 128, d, false, 800);
                ball.OnDrop += new EventHandler<TouchEventArgs>(ball_OnDrop);
                //addToUpdate(ball);
                ballList.Add(ball);

                this.Children.Add(ball);
            }
        }

        public void setToScale(float scaleFactor)
        {
            Matrix matrix = Matrix.Identity;

            matrix.Scale(scaleFactor, scaleFactor);
            matrix.Translate(posX, posY);

            this.RenderTransform = new MatrixTransform(matrix);
        }

        void ball_OnDrop(object sender, TouchEventArgs e)
        {
            if (sender is Ball)
            {
                Ball ball = (Ball)sender;

                ballList.Remove(ball);
                this.Children.Remove(ball);

                ball.Source = null;

                world.deleteDrawing(ball.Drawing);
            }
        }

        private void initPlanet()
        {
            var uriSource = new Uri(@"/Cultiverse;component/Resources/earth.png", UriKind.Relative);
            planet.Source = new BitmapImage(uriSource);
            planet.Width = 800;
            planet.Height = 800;

            this.Children.Add(planet);
        }


        private void touchMove(object sender, TouchEventArgs e)
        {
            float touchX = (float)e.GetTouchPoint(planet).Position.X;
            float touchY = (float)e.GetTouchPoint(planet).Position.Y;
            float dX = touchX - lastX;
            float dY = touchY - lastY;
            foreach (Ball b in ballList)
                b.push(dX, dY, touchX, touchY);
            lastX = touchX;
            lastY = touchY;
        }

        public void pushInertedBalls(float dx, float dy)
        {
            foreach (Ball b in ballList)
                b.pushSimple(dx, dy);
        }

        public void addBall(Ball newBall)
        {
            ballList.Add(newBall);

            newBall.OnDrop += new EventHandler<TouchEventArgs>(ball_OnDrop);

            this.Children.Add(newBall);
        }


        public void update(float deltaTime)
        {
            foreach (Ball b in ballList)
            {
                b.update(deltaTime);
                b.collide(ballList);
            }
        }

        /*public void updateViewOffset(float newX, float newY)
        {
            foreach (Ball b in ballList)
            {
                b.pushSimple(newX - viewOffsetX, newY - viewOffsetY);
            }

            viewOffsetX = newX;
            viewOffsetY = newY;

            Canvas.SetLeft(planet, posX + newX);
            Canvas.SetTop(planet, posY + newY);
        }*/

        public void addToUpdate(object updateable)
        {
            updateList.Add(updateable);
        }

        public void removeFromUpdate(object updateable)
        {
            updateList.Remove(updateable);
        }
    }
}
