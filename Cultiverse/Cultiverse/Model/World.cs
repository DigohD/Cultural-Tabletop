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
            int drawingsCount = drawingsFolder.Count() / 2; //Each drawing is 2 files.
            for (int i = 0; i < drawingsCount; i++) 
            {
                drawings.Add(new WorldDrawing(FolderPath + "\\drawings", drawings.Count));
            }
        }

        public WorldDrawing createNewDrawing()
        {
            WorldDrawing newDrawing = new WorldDrawing(FolderPath + "\\drawings", drawings.Count);
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
