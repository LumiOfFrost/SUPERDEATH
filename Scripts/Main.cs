using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SUPERDEATH.Scripts
{
    public class Main : Game
    {

        //Graphics

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Matrix worldMatrix = Matrix.CreateTranslation(0, 0, 0);
        public Matrix viewMatrix;
        Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), 800f / 480f, 0.01f, 100f);

        //Assets

        BasicEffect bEffect;

        //Game

        public static float gameSpeed = 1;

        public List<GameObject> gameObjects;

        //Player

        Player player;

        public Main()
        {

            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {

            gameObjects = new List<GameObject>();

            base.Initialize();
        
        }

        protected override void BeginRun()
        {

            gameObjects.Add(new GameObject(new Transform(new Vector3(0, -1, 0), new Vector3(10, 1, 10), Vector3.Zero), PrimitiveMesh.GetCuboid(GraphicsDevice, new Vector3(0, -1, 0), new Vector3(10, 1, 10), Color.White), RenderType.Primitive, "Solid"));

            gameObjects.Add(new Player(new Transform(new Vector3(0, 1, 0), new Vector3(0.75f, 1.75f, 0.75f), Vector3.Zero)));

            player = gameObjects.OfType<Player>().First();

            base.BeginRun();
        }

        protected override void LoadContent()
        {

            _spriteBatch = new SpriteBatch(GraphicsDevice);

           bEffect = new BasicEffect(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (GameObject g in gameObjects)
            {

                g.Update(this, gameTime);

            }

            viewMatrix = Matrix.CreateLookAt(player.transform.position, player.transform.position + Vector3.Forward, Vector3.Up) * Matrix.CreateFromYawPitchRoll(player.transform.rotation.Y, player.transform.rotation.X, player.transform.rotation.Z);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);

            foreach (GameObject g in gameObjects)
            {

                switch(g.renderType)
                {

                    case RenderType.Model:
                        DrawModel(g.model, Matrix.CreateTranslation(g.transform.position) * Matrix.CreateScale(g.transform.scale.X, g.transform.scale.Y, g.transform.scale.Z), viewMatrix, projectionMatrix);
                        break;
                    case RenderType.None:
                        break;
                    case RenderType.Primitive:
                        bEffect.VertexColorEnabled = true;
                        bEffect.AmbientLightColor = new Vector3(0.5f, 0.5f, 0.5f);
                        bEffect.World = worldMatrix;
                        bEffect.View = viewMatrix;
                        bEffect.Projection = projectionMatrix;
                        g.mesh.Draw(GraphicsDevice, bEffect);
                        break;
                    default:
                        Debug.WriteLine("Error! No RenderType found!");
                        break;

                }
                

            }

            base.Draw(gameTime);
        
        }

        private void DrawModel(Model model, Matrix world, Matrix view, Matrix projection)
        {

            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = world;
                        effect.View = view;
                        effect.Projection = projection;
                    }

                    mesh.Draw();
                }
            }
            
        }

    }
}
