﻿using System;
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
        float scaleFactor;
        int planetID;

        ArrayList ballList = new ArrayList();
        ArrayList updateList = new ArrayList();

        World world;

        public Ellipse planet = new Ellipse();

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

            Matrix matrix = Matrix.Identity;

            matrix.Scale(scaleFactor, scaleFactor);
            matrix.Translate(newX, newY);

            this.RenderTransform = new MatrixTransform(matrix);

            initPlanet();

            foreach (WorldDrawing d in world.getDrawings())
            {
                Ball ball = new Ball(1, (int)(800 / 2 - 64), (int)(800 / 2 - 64), 128, 128, d, this);
                //addToUpdate(ball);
                ballList.Add(ball);

                this.Children.Add(ball.getBallImage());
            }

            planet.TouchDown += planetClicked;
        }

        private void initPlanet()
        {
            planet.Fill = new SolidColorBrush(Colors.LightGray);
            planet.Width = 800;
            planet.Height = 800;

            this.Children.Add(planet);
        }

        public void planetClicked(object sender, TouchEventArgs e)
        {
        }

        public void addBall(Ball newBall)
        {
            ballList.Add(newBall);

            this.Children.Add(newBall.getBallImage());
        }


        public void update(float deltaTime)
        {
            foreach (Ball b in ballList)
            {
                b.update(deltaTime);
                b.collide(ballList);
            }
        }

        public void updateViewOffset(float newX, float newY)
        {
            foreach (Ball b in ballList)
            {
                b.pushSimple(newX - viewOffsetX, newY - viewOffsetY);
            }

            viewOffsetX = newX;
            viewOffsetY = newY;

            Canvas.SetLeft(planet, posX + newX);
            Canvas.SetTop(planet, posY + newY);
        }

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
