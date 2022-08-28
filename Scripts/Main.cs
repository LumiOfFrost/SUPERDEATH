using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apos.Shapes;
using MonoGame.Extended.ViewportAdapters;
using static System.Formats.Asn1.AsnWriter;

namespace SUPERDEATH.Scripts
{
    public class Main : Game
    {

        //Graphics

        public GraphicsDeviceManager _graphics;
        private ShapeBatch _shapeBatch;
        private SpriteBatch _spriteBatch;

        RenderTarget2D mainRT;
        RenderTarget2D uiRT;

        Matrix worldMatrix = Matrix.CreateTranslation(0, 0, 0);
        public Matrix viewMatrix;
        Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.01f, 100f);

        //Assets

        BasicEffect bEffect;

        //Game

        public static float gameSpeed = 1;

        public static bool paused = false;

        public List<GameObject> gameObjects;

        BoxingViewportAdapter va;

        //Player

        Player player;

        Camera currentCamera;

        public Main()
        {

            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            va = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;

        }

        protected override void Initialize()
        {

            gameObjects = new List<GameObject>();

            base.Initialize();
        
        }

        protected override void BeginRun()
        {

            gameObjects.Add(new GameObject(new Transform(new Vector3(0, -1f, 0), new Vector3(10, 1, 10), Vector3.Zero), PrimitiveMesh.GetCuboid(GraphicsDevice, new Vector3(0, -1, 0), new Vector3(10, 1, 10), Color.White), RenderType.Primitive, "Solid"));

            gameObjects.Add(new Player(new Transform(new Vector3(0, 1, 0), new Vector3(0.5f, 1.8f, 0.5f), Vector3.Zero), new Vector3(0, 0.75f, 0)));

            gameObjects.Add(new GameObject(new Transform(new Vector3(-9.5f, 2f, 0), new Vector3(5, 6, 10), Vector3.Zero), PrimitiveMesh.GetCuboid(GraphicsDevice, new Vector3(-9.5f, 2f, 0), new Vector3(5, 6, 10), Color.White), RenderType.Primitive, "Solid"));

            player = gameObjects.OfType<Player>().First();

            currentCamera = player.camera;

            base.BeginRun();
        }

        protected override void LoadContent()
        {

            _shapeBatch = new ShapeBatch(GraphicsDevice, Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mainRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            uiRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            bEffect = new BasicEffect(GraphicsDevice);

        }

        protected override void Update(GameTime gameTime)
        {

            if (InputManager.prevKeyState == null)
            {
                InputManager.prevKeyState = Keyboard.GetState();
            }

            if (InputManager.prevPadState == null)
            {
                InputManager.prevPadState = GamePad.GetState(PlayerIndex.One);
            }

            if (InputManager.Pause())
            {

                paused = !paused;
                gameSpeed = paused ? 0 : 1;
                IsMouseVisible = paused;
                Mouse.SetPosition(_graphics.GraphicsDevice.Viewport.Width / 2, _graphics.GraphicsDevice.Viewport.Height / 2);

            }

            if (InputManager.Fullscreen())
            {

                _graphics.ToggleFullScreen();

            }

            viewMatrix = Matrix.CreateLookAt(currentCamera.position, currentCamera.LookAt(), currentCamera.up);

            foreach (GameObject g in gameObjects)
            {

                g.Update(this, gameTime);

            }

            InputManager.prevPadState = GamePad.GetState(PlayerIndex.One);

            InputManager.prevKeyState = Keyboard.GetState();

            gameObjects.Sort((a, b) => a.GetDistance(currentCamera.LookAt()).CompareTo(b.GetDistance(currentCamera.LookAt())));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(uiRT);

            GraphicsDevice.Clear(Color.Transparent);

            _shapeBatch.Begin();

            _shapeBatch.DrawCircle(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), 2f, Color.White, Color.White);

            _shapeBatch.End();

            GraphicsDevice.SetRenderTarget(mainRT);

            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (GameObject g in gameObjects)
            {

                switch (g.renderType)
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

            GraphicsDevice.SetRenderTarget(null);

            _spriteBatch.Begin();

            _spriteBatch.Draw(mainRT, GraphicsDevice.Viewport.Bounds, Color.White);

            _spriteBatch.Draw(uiRT, GraphicsDevice.Viewport.Bounds, Color.White);

            _spriteBatch.End();

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
