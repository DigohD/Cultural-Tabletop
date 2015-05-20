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
    public class CreatePlanet : Updateable
    {
        public float posX, posY, width, height, ballXoffset, ballYoffset, viewOffsetX, viewOffsetY;
        float scaleFactor;
        Canvas createWorldCanvas;
        int planetID;

        ArrayList ballList = new ArrayList();
        ArrayList updateList = new ArrayList();
        List<WorldDrawing> drawings;

        World world;

        public Ellipse planet = new Ellipse();
        CreateWorldView parent;

        public CreatePlanet(float newX, float newY, float newScale, Canvas newCanvas, CreateWorldView newParent, int newID)
        {
            posX = newX;
            posY = newY;
            scaleFactor = newScale;
            createWorldCanvas = newCanvas;
            parent = newParent;
            planetID = newID;

            width = 800 * scaleFactor;
            height = 800 * scaleFactor;

            ballXoffset = -(1920 / 2) + newX + width / 2;
            ballYoffset = -(1080 / 2) + newY + height / 2;

            initPlanet();
        }

        public void addBall(Ball newBall)
        {
            addToUpdate(newBall);
            ballList.Add(newBall);

            newBall.setToScale(scaleFactor);

            createWorldCanvas.Children.Add(newBall.getBallImage());
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

            createWorldCanvas.Children.Add(planet);
        }

        public void setToScale(float newScale)
        {
            scaleFactor = newScale;

            planet.Width = 800 * scaleFactor;
            planet.Height = 800 * scaleFactor;

            width = 800 * scaleFactor;
            height = 800 * scaleFactor;

            ballXoffset = -(1920 / 2) + posX + width / 2;
            ballYoffset = -(1080 / 2) + posY + height / 2;

            foreach (Ball b in ballList)
            b.setToScale(scaleFactor);
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
