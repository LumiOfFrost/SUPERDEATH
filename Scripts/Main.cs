using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Apos.Shapes;
using MonoGame.Extended.ViewportAdapters;
using static System.Formats.Asn1.AsnWriter;
using System;
using Microsoft.Xna.Framework.Audio;

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
        Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.001f, 300f);

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

            _graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            va = new BoxingViewportAdapter(Window, GraphicsDevice, 1280, 720);

            Content.RootDirectory = "Content";
            IsMouseVisible = false;

        }

        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = SurfaceFormat.Color;
            e.GraphicsDeviceInformation.PresentationParameters.DepthStencilFormat = DepthFormat.Depth24;
        }

        protected override void Initialize()
        {

            gameObjects = new List<GameObject>();

            base.Initialize();
        
        }

        protected override void BeginRun()
        {

            gameObjects.Add(new GameObject(new Transform(new Vector3(0, -1f, 0), new Vector3(10, 1, 10), Vector3.Zero), PrimitiveMesh.GetCuboid(GraphicsDevice, new Vector3(0, -1, 0), new Vector3(10, 1, 10), Color.White), RenderType.Primitive, "Solid"));

            gameObjects.Add(new GameObject(new Transform(new Vector3(75, -1f, 0), new Vector3(10, 1, 10), Vector3.Zero), PrimitiveMesh.GetCuboid(GraphicsDevice, new Vector3(75, -1, 0), new Vector3(10, 1, 10), Color.White), RenderType.Primitive, "Solid"));

            gameObjects.Add(new Player(new Transform(new Vector3(0, 1, 0), new Vector3(0.5f, 1.8f, 0.5f), Vector3.Zero), new Vector3(0, 0.75f, 0)));

            gameObjects.Add(new GameObject(new Transform(new Vector3(-9.5f, 2f, 0), new Vector3(5, 6, 10), Vector3.Zero), PrimitiveMesh.GetCuboid(GraphicsDevice, new Vector3(-9.5f, 2f, 0), new Vector3(5, 6, 10), Color.White), RenderType.Primitive, "Solid"));

            gameObjects.Add(new LineObject(PrimitiveMesh.GetLine(GraphicsDevice, new Vector3(5, -1f, 0), new Vector3(70, -1f, 0), Color.Aquamarine)));
            gameObjects.Add(new LineObject(PrimitiveMesh.GetLine(GraphicsDevice, new Vector3(5, -1f, -1), new Vector3(70, -1f, 1), Color.Aquamarine)));
            gameObjects.Add(new LineObject(PrimitiveMesh.GetLine(GraphicsDevice, new Vector3(5, -1f, 1), new Vector3(70, -1f, -1), Color.Aquamarine)));
            gameObjects.Add(new LineObject(PrimitiveMesh.GetLine(GraphicsDevice, new Vector3(5, -1f, -2), new Vector3(70, -1f, 2), Color.Aquamarine)));
            gameObjects.Add(new LineObject(PrimitiveMesh.GetLine(GraphicsDevice, new Vector3(5, -1f, 2), new Vector3(70, -1f, -2), Color.Aquamarine)));

            player = gameObjects.OfType<Player>().First();

            currentCamera = player.camera;

            base.BeginRun();
        }

        protected override void LoadContent()
        {

            AssetManager.Load(Content);

            _shapeBatch = new ShapeBatch(GraphicsDevice, Content);
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mainRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 3, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            uiRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 3, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

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

            if (!IsActive)
            {

                paused = true;
                gameSpeed = 0;
                IsMouseVisible = true;

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

            gameObjects.Sort((a, b) => a.GetDistance(currentCamera.position).CompareTo(b.GetDistance(currentCamera.position)));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(uiRT);

            GraphicsDevice.Clear(Color.Transparent);

            _shapeBatch.Begin();

            _shapeBatch.DrawCircle(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), 1f, Color.White, Color.White);

            if (player.dashCount > 0)
            {
                _shapeBatch.DrawCircle(new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2 - 6), 1f, Color.Aquamarine, Color.Aquamarine);
            }

            _shapeBatch.DrawCircle(new Vector2(20, 20), 15f, player.dashCount > 0 ? player.dashCount > 2 ? Color.Aquamarine : player.dashCount > 1 ? Color.MediumAquamarine : Color.DarkSlateGray : new Color(0.1f, 0.1f, 0.1f), Color.White, 2);

            if (player.dashCount > 1)
            {

                _shapeBatch.DrawCircle(new Vector2(GraphicsDevice.Viewport.Width / 2 - 5, GraphicsDevice.Viewport.Height / 2 + 4), 1f, Color.Aquamarine, Color.Aquamarine);

                _shapeBatch.DrawCircle(new Vector2(20, 20), 10f, player.dashCount > 2 ? Color.Aquamarine : player.dashCount > 1 ? Color.MediumAquamarine : Color.DarkSlateGray, Color.White, 2);

            }
            if (player.dashCount > 2)
            {

                _shapeBatch.DrawCircle(new Vector2(GraphicsDevice.Viewport.Width / 2 + 5, GraphicsDevice.Viewport.Height / 2 + 4), 1f, Color.Aquamarine, Color.Aquamarine);

                _shapeBatch.DrawCircle(new Vector2(20, 20), 5f, player.dashCount > 2 ? Color.Aquamarine : player.dashCount > 1 ? Color.MediumAquamarine : Color.DarkSlateGray, Color.White, 2);

            }

            _shapeBatch.End();

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SetRenderTarget(mainRT);

            GraphicsDevice.Clear(Color.Black);

            BasicEffect basicEffect = new BasicEffect(GraphicsDevice)
            {

                TextureEnabled = true,
                VertexColorEnabled = true

            };

            Viewport viewport = GraphicsDevice.Viewport;

            Vector3 textPosition = new Vector3(6, 5, 0);

            basicEffect.World = Matrix.CreateConstrainedBillboard(textPosition, textPosition - currentCamera.forward, Vector3.Down, null, null);
            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            const string message = "WASD or LEFT STICK to move\r\nSPACE or A BUTTON to jump\r\nSHIFT or LEFT BUMPER to dash\r\nJUMP out of a DASH for extra height\r\nNumpad 0 to reset position";
            Vector2 textOrigin = AssetManager.arial.MeasureString(message) / 2;
            const float textSize = 0.05f;

            _spriteBatch.Begin(0, null, null, null, RasterizerState.CullNone, basicEffect);

            _spriteBatch.DrawString(AssetManager.arial, message, Vector2.Zero, Color.Yellow, 0, textOrigin, textSize, 0, 0);

            _spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
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

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

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
