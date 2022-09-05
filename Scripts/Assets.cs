using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUPERDEATH.Scripts
{
    public static class Assets
    {

        //Fonts

        public static SpriteFont arialFont;

        //Sounds

        public static SoundEffect dashSound;
        public static SoundEffect jumpSound;

        //Models
        //Primitives
        public static Model primitiveCube;

        //Methods

        public static void Load(ContentManager content)
        {

            //Fonts
            arialFont = content.Load<SpriteFont>("Fonts/Arial");

            //Sounds
            dashSound = content.Load<SoundEffect>("Sounds/Dash");
            jumpSound = content.Load<SoundEffect>("Sounds/Jump");

            //Models
            primitiveCube = content.Load<Model>("Models/Primitives/Cube");

        }

        public static Model GetModelFromName(string name)
        {

            return (Model)typeof(Assets).GetField(name).GetValue(typeof(Assets));

        }

        public static Texture2D GetTextureFromName(string name)
        {

            return (Texture2D)typeof(Assets).GetField(name).GetValue(typeof(Assets));

        }

    }
}
