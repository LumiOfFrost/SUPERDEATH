using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using static System.Formats.Asn1.AsnWriter;
using System;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Shapes;

namespace SUPERDEATH.Scripts
{
    public class Main : Game
    {

        //Graphics

        public GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        RenderTarget2D mainRT;
        RenderTarget2D uiRT;
        RenderTarget2D staminaRT;
        RenderTarget2D staminaMask;

        DepthStencilState s1;
        DepthStencilState s2;

        Matrix worldMatrix = Matrix.CreateTranslation(0, 0, 0);
        public Matrix viewMatrix;
        Matrix projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 800f / 480f, 0.001f, 300f);

        //Assets

        BasicEffect bEffect;

        //Game

        public static float gameSpeed = 1;

        public static bool paused = false;

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

            base.Initialize();
        
        }

        protected override void BeginRun()
        {

            LevelManager.Load("levelTest.json");
            LevelManager.currentLevel.gameObjects.Add(new Player(new Transform(new Vector3(0,3,0), new Vector3(0.75f, 1.75f, 0.75f), Vector3.Zero), new Vector3(0, 1.66f, 0)));
            player = LevelManager.currentLevel.gameObjects.OfType<Player>().First();

            currentCamera = player.camera;

            foreach (GameObject g in LevelManager.currentLevel.gameObjects)
            {

                g.Init();

            }

            base.BeginRun();

        }

        protected override void LoadContent()
        {

            Assets.Load(Content);

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mainRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 3, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);
            uiRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 3, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            staminaRT = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 3, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            staminaMask = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width / 3, GraphicsDevice.Viewport.Height / 3, true, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

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

            foreach (GameObject g in LevelManager.currentLevel.gameObjects)
            {

                g.Update(this, gameTime);

            }

            InputManager.prevPadState = GamePad.GetState(PlayerIndex.One);

            InputManager.prevKeyState = Keyboard.GetState();

            LevelManager.currentLevel.gameObjects.Sort((a, b) => a.GetDistance(currentCamera.position).CompareTo(b.GetDistance(currentCamera.position)));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.SetRenderTarget(uiRT);

            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();

            _spriteBatch.DrawCircle(new CircleF(new Point2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), 1), 8, Color.White, 1);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(staminaRT);

            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin(samplerState:SamplerState.PointClamp);

            _spriteBatch.FillRectangle(new Vector2(5, GraphicsDevice.Viewport.Height - 15), new Size2(105 * player.stamina / 3, 10), Color.Aquamarine);

            _spriteBatch.End();

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
            Vector2 textOrigin = Assets.arialFont.MeasureString(message) / 2;
            const float textSize = 0.05f;

            _spriteBatch.Begin(0, null, SamplerState.PointClamp, null, RasterizerState.CullNone, basicEffect);

            _spriteBatch.DrawString(Assets.arialFont, message, Vector2.Zero, Color.Yellow, 0, textOrigin, textSize, 0, 0);

            _spriteBatch.End();

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (GameObject g in LevelManager.currentLevel.gameObjects)
            {

                switch (g.renderType)
                {

                    case RenderType.Model:
                        DrawModel(g.texture == "untextured" ? new Texture2D(GraphicsDevice, 1, 1) : Assets.GetTextureFromName(g.texture), Assets.GetModelFromName(g.model), worldMatrix * Matrix.CreateTranslation(g.transform.position) * Matrix.CreateScale(g.transform.scale.X / 2f, g.transform.scale.Y / 2f, g.transform.scale.Z / 2f) * Matrix.CreateFromYawPitchRoll(g.transform.rotation.Y, g.transform.rotation.X, g.transform.rotation.Z), viewMatrix, projectionMatrix);
                        break;
                    case RenderType.None:
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

            _spriteBatch.Draw(staminaRT, GraphicsDevice.Viewport.Bounds, Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        
        }

        private void DrawModel(Texture2D texture, Model model, Matrix world, Matrix view, Matrix projection)
        {

            if (model != null)
            {
                foreach (ModelMesh mesh in model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.Texture = texture;
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
