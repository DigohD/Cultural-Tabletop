﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Cultiverse.Model
{
    public class WorldDatabase
    {
        private List<World> universe;
        string cultiverseFolder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Cultiverse";
        string worldsFolder;
        public WorldDatabase()
        {
            worldsFolder = cultiverseFolder + "\\worlds";

            if (!Directory.Exists(cultiverseFolder))
            {
                Directory.CreateDirectory(cultiverseFolder);
            }
            if (!Directory.Exists(worldsFolder))
            {
                Directory.CreateDirectory(worldsFolder);
            }
            universe = new List<World>();

            IEnumerable<string> worldFolders = Directory.EnumerateDirectories(worldsFolder);
            foreach (string worldFolder in worldFolders)
            {
                universe.Add(new World(worldFolder));
            }
        }

        public World createNewWorld()
        {
            string worldPath = worldsFolder + "\\" + universe.Count;

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
