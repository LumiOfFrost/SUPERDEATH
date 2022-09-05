using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SUPERDEATH.Scripts
{

    [System.Serializable]
    public class Level
    {

        public Vector3 playerStart;

        public List<GameObject> gameObjects;

        public Level()
        {
            this.playerStart = Vector3.Zero;
            this.gameObjects = new List<GameObject>();
        }

    }

    public class LevelManager
    {

        public static Level currentLevel;

        public static void Save(Level sg)
        {

            string converted = JsonConvert.SerializeObject(sg, Formatting.Indented, new JsonSerializerSettings{TypeNameHandling = TypeNameHandling.All});

            File.WriteAllText(@"Content\levelTest.json", converted);

        }

        public static void Load(string path)
        {

            currentLevel = JsonConvert.DeserializeObject<Level>(File.ReadAllText(@"Content\" + path));

        }

    }

}
