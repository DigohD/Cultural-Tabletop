using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cultiverse.Model
{
    class WorldDatabase
    {

        private List<World> universe;

        public WorldDatabase()
        {
            //TODO: load from folders/files.
            universe = new List<World>();
        }

        public World createNewWorld()
        {
            //TODO: Increment world folder path

            string worldPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Cultiverse\\worlds\\1";

            if (!Directory.Exists(worldPath))
            {
                Directory.CreateDirectory(worldPath);
                Directory.CreateDirectory(worldPath + "\\drawings");
            }
            return new World(worldPath);
        }

        public bool saveWorld(World world)
        {
            universe.Add(world);
            return true;
        }

        public List<World> getUniverse()
        {
            //Warning This returns a mutable list.
            return universe;
        }
    }
}
