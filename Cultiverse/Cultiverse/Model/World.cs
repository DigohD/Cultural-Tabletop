using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Cultiverse.Model
{
    public class World
    {
        public string FolderPath;
        private List<WorldDrawing> drawings;

        public World(string folderPath)
        {
            drawings = new List<WorldDrawing>();
            FolderPath = folderPath;

            IEnumerable<string> drawingsFolder = Directory.EnumerateFiles(folderPath + "\\drawings");
            foreach (string file in drawingsFolder)
            {
                string strId = Path.GetFileNameWithoutExtension(file);
                int id = int.Parse(strId);
                string extension = Path.GetExtension(file);

                if (extension.Equals(".png"))
                {
                    drawings.Add(new WorldDrawing(FolderPath + "\\drawings", id));
                }

            }
        }

        public WorldDrawing createNewDrawing()
        {
            int newId = 0;
            if (drawings.Count > 0)
            {
                newId = drawings.Last().Id + 1;
            }
            WorldDrawing newDrawing = new WorldDrawing(FolderPath + "\\drawings", newId);
            drawings.Add(newDrawing);
            return newDrawing;
        }

        public void deleteDrawing(WorldDrawing drawing)
        {
            //Delete the file after 1 second
            Task.Factory.StartNew(() => Thread.Sleep(1 * 1000))
            .ContinueWith((t) =>
            {
                File.Delete(drawing.BitmapFilePath);
                File.Delete(drawing.StrokesFilePath);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            drawings.Remove(drawing);
        }

        public List<WorldDrawing> getDrawings()
        {
            return drawings;
        }
    }
}
