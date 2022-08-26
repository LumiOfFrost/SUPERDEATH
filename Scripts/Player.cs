using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SUPERDEATH.Scripts
{
    public class Player : GameObject
    {

        KeyboardState prevKeyState;
        GamePadState prevPadState;

        public Player(Transform tform) : base(tform, null, RenderType.None, "Player", false)
        {

            transform = tform;

            model = null;

            renderType = RenderType.None;

            tag = "Player";

            solid = false;

            velocity = Vector3.Zero;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

        }

        public override void Update(Main main, GameTime gameTime)
        {

            if (prevKeyState == null)
            {
                prevKeyState = Keyboard.GetState();
            }

            if (prevPadState == null)
            {
                prevPadState = GamePad.GetState(PlayerIndex.One);
            }

            Matrix viewToWorld = Matrix.Invert(main.viewMatrix);

            Vector3 tempVeloc = Vector3.One;

            velocity.X = 0;
            velocity.Z = 0;

            if (InputManager.IsMovingLeft() && !InputManager.IsMovingRight())
            {

                velocity = -viewToWorld.Right * 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if (InputManager.IsMovingRight() && !InputManager.IsMovingLeft())
            {

                velocity = viewToWorld.Right * 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if (InputManager.IsMovingUp() && !InputManager.IsMovingDown())
            {

                velocity = viewToWorld.Forward * 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if (InputManager.IsMovingDown() && !InputManager.IsMovingUp())
            {

                velocity = -viewToWorld.Forward * 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if(InputManager.Jump(prevKeyState, prevPadState))
            {

                velocity.Y = 0.75f;

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {

                transform.rotation.X += (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

            velocity.Y -= 1.35f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            CheckForCollision(main.gameObjects);

            prevKeyState = Keyboard.GetState();

            base.Update(main, gameTime);
        
        }

        void CheckForCollision(List<GameObject> gameObjects)
        {

            transform.position.X += velocity.X * Main.gameSpeed;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector3 depth = ColliderUtils.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.X) > 0)
                    {

                        transform.position.X += depth.X;

                        velocity.X = 0;

                        collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

                    }

                }

            }

            transform.position.Y += velocity.Y * Main.gameSpeed;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector3 depth = ColliderUtils.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.Y) > 0)
                    {

                        if ((Math.Abs(depth.X) < 0.25f || Math.Abs(depth.Z) < 0.25f) && velocity.Y > 0)
                        {

                            transform.position.X += depth.X;

                        }
                        else
                        {

                            transform.position.Y += depth.Y;

                            if (velocity.Y > 0)
                            {

                                velocity.Y = -velocity.Y / 2;

                            }
                            else
                            {

                                velocity.Y = 0;

                            }

                        }

                        collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

                    }

                }

            }

            transform.position.Z += velocity.Z * Main.gameSpeed;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector3 depth = ColliderUtils.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.Z) > 0)
                    {

                        transform.position.Z += depth.Z;

                        velocity.Z = 0;

                        collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

                    }

                }

            }

        }

    }
}
