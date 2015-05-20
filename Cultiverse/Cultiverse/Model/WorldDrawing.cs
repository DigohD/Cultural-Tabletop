using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cultiverse.Model
{
    public class WorldDrawing
    {
        public string StrokesFilePath;
        public string BitmapFilePath;

        public WorldDrawing(string folderPath, int id)
        {
            StrokesFilePath = folderPath + "\\" + id + ".isf";
            BitmapFilePath = folderPath + "\\" + id + ".png";
        }
    }
}
