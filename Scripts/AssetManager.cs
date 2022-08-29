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
    public class AssetManager
    {

        //Fonts

        public static SpriteFont arial;

        //Sounds

        public static SoundEffect dashSound;
        public static SoundEffect jumpSound;

        //Methods

        public static void Load(ContentManager content)
        {

            //Fonts
            arial = content.Load<SpriteFont>("Fonts/Arial");

            //Sounds
            dashSound = content.Load<SoundEffect>("Sounds/Dash");
            jumpSound = content.Load<SoundEffect>("Sounds/Jump");

        }

    }
}
