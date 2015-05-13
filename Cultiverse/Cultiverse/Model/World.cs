using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cultiverse.Model
{
    class World
    {
        public string FolderPath;
        private List<WorldDrawing> drawings;

        public World(string folderPath)
        {
            drawings = new List<WorldDrawing>();
            FolderPath = folderPath;
        }

        public WorldDrawing createNewDrawing()
        {
            WorldDrawing newDrawing = new WorldDrawing(FolderPath + "\\drawings",drawings.Count);
            drawings.Add(newDrawing);
            return newDrawing;
        }
    }
}
