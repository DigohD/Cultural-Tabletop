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

namespace Cultiverse.Model
{
    class Planet : Updateable
    {
        public float posX, posY, width, height, ballXoffset, ballYoffset, viewOffsetX, viewOffsetY;
        float scaleFactor;
        Canvas universeCanvas;

        ArrayList ballList = new ArrayList();
        ArrayList updateList = new ArrayList();
        List<WorldDrawing> drawings;

        World world;

        public Ellipse planet = new Ellipse();
        UniverseView parent;

        public Planet(float newX, float newY, float newScale, Canvas newCanvas, World newWorld, UniverseView newParent)
        {
            posX = newX;
            posY = newY;
            scaleFactor = newScale;
            universeCanvas = newCanvas;
            parent = newParent;

            world = newWorld;

            width = 800 * scaleFactor;
            height = 800 * scaleFactor;

            ballXoffset = -(1920 / 2) + newX + width / 2;
            ballYoffset = -(1080 / 2) + newY + height / 2;

            initPlanet();

            drawings = world.getDrawings();
            foreach (WorldDrawing d in drawings)
            {
                Ball ball = new Ball(1, (int)(universeCanvas.Width / 2 - 64), (int)(universeCanvas.Height / 2 - 64), 128, 128, d, this);
                addToUpdate(ball);
                ballList.Add(ball);

                ball.setToScale(scaleFactor);

                universeCanvas.Children.Add(ball.getBallImage());
            }

            planet.TouchDown += planetClicked;
        }

        private void initPlanet()
        {
            planet.Fill = new SolidColorBrush(Colors.LightGray);
            planet.Width = 800 * scaleFactor;
            planet.Height = 800 * scaleFactor;

            //Canvas.SetLeft(planet, universeCanvas.Width / 2 - planet.Width / 2);
            //Canvas.SetTop(planet, universeCanvas.Height / 2 - planet.Height / 2);

            Canvas.SetLeft(planet, posX);
            Canvas.SetTop(planet, posY);

            universeCanvas.Children.Add(planet);
        }

        public void planetClicked(object sender, TouchEventArgs e)
        {
            
        }

        public void setToScale(float newScale)
        {
            /*scaleFactor = newScale;

            planet.Width = 800 * scaleFactor;
            planet.Height = 800 * scaleFactor;

            width = 800 * scaleFactor;
            height = 800 * scaleFactor;

            ballXoffset = -(1920 / 2) + posX + width / 2;
            ballYoffset = -(1080 / 2) + posY + height / 2;

            foreach (Ball b in ballList)
            b.setToScale(scaleFactor);*/
        }

        public override void update(float deltaTime)
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
