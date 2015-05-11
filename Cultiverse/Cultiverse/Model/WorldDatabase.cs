using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return new World();
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
