using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cultiverse.Model
{
    public class WorldDrawing
    {
        private string _strokesFilePath;
        public string StrokesFilePath
        {
            get {
                return _strokesFilePath;
            }
        }
        private string _bitmapFilePath;
        public string BitmapFilePath
        {
            get
            {
                return _bitmapFilePath;
            }
        }
        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
        }

        public WorldDrawing(string folderPath, int id)
        {
            _id = id;
            _strokesFilePath = folderPath + "\\" + id + ".isf";
            _bitmapFilePath = folderPath + "\\" + id + ".png";
        }
    }
}
