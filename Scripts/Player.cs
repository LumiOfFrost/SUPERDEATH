using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SUPERDEATH.Scripts
{
    public class Player : GameObject
    {

        float mouseSpeed = 0.01f;

        float moveSpeed = 15f;

        public Matrix viewToWorld;

        public Camera camera;

        Vector3 cameraLocalPosition;

        public Player(Transform tform, Vector3 relativeCameraPos) : base(tform, null, RenderType.None, "Player", false)
        {

            transform = tform;

            cameraLocalPosition = relativeCameraPos;

            camera = new Camera(Vector3.Forward, Vector3.Up, new Vector3(transform.position.X + relativeCameraPos.X, transform.position.Y - transform.scale.Y / 2 + relativeCameraPos.Y, transform.position.Z + relativeCameraPos.Z));

            model = null;

            renderType = RenderType.None;

            tag = "Player";

            solid = false;

            velocity = Vector3.Zero;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

        }

        public override void Update(Main main, GameTime gameTime)
        {

            viewToWorld = Matrix.Invert(main.viewMatrix);

            Vector3 cameraYOnly = new Vector3(camera.forward.X, 0, camera.forward.Z);
            cameraYOnly.Normalize();
            camera.position = new Vector3(transform.position.X + cameraLocalPosition.X, transform.position.Y - transform.scale.Y / 2 + cameraLocalPosition.Y, transform.position.Z + cameraLocalPosition.Z);
            Matrix movementVTW = Matrix.Invert(Matrix.CreateLookAt(camera.position, camera.position + cameraYOnly, camera.up));

            Vector3 movementVelocity = Vector3.Zero;

            Vector2 mouseMovement = (Mouse.GetState().Position.ToVector2() - new Vector2(main._graphics.GraphicsDevice.Viewport.Width / 2, main._graphics.GraphicsDevice.Viewport.Height / 2)) * Main.gameSpeed;
            
            if (InputManager.IsMovingLeft() && !InputManager.IsMovingRight())
            {

                movementVelocity += -movementVTW.Right * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if (InputManager.IsMovingRight() && !InputManager.IsMovingLeft())
            {

                movementVelocity += movementVTW.Right * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if (InputManager.IsMovingUp() && !InputManager.IsMovingDown())
            {

                movementVelocity += movementVTW.Forward * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            if (InputManager.IsMovingDown() && !InputManager.IsMovingUp())
            {

                movementVelocity += -movementVTW.Forward * moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

            if (InputManager.Jump())
            {

                velocity.Y = 0.35f * Main.gameSpeed;

            }

            velocity.Y -= 9.8f * 4 * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 direction = camera.forward;
            direction.Normalize();

            Vector3 normal = Vector3.Cross(direction, camera.up);

            mouseMovement.Y *= main.GraphicsDevice.Viewport.Height / 800.0f;
            mouseMovement.X *= main.GraphicsDevice.Viewport.Width / 1280.0f;

            camera.forward += mouseMovement.X * mouseSpeed * normal;

            camera.forward -= mouseMovement.Y * mouseSpeed * camera.up;
            camera.forward.Normalize();

            camera.forward.X = MathHelper.Clamp(camera.forward.X, MathHelper.ToRadians(-75), MathHelper.ToRadians(75));
            camera.forward.Z = MathHelper.Clamp(camera.forward.Z, MathHelper.ToRadians(-75), MathHelper.ToRadians(75));

            movementVelocity.Y = velocity.Y;

            velocity = Vector3.Lerp(velocity, movementVelocity, 0.16f) * Main.gameSpeed;

            CheckForCollision(main.gameObjects);

            if (main.IsActive && !Main.paused)
            {

                Mouse.SetPosition(main._graphics.GraphicsDevice.Viewport.Width / 2, main._graphics.GraphicsDevice.Viewport.Height / 2);

            }

            base.Update(main, gameTime);
        
        }

        void CheckForCollision(List<GameObject> gameObjects)
        {

            transform.position.X += velocity.X;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector3 depth = ColliderUtils.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.X) > 0)
                    {

                        transform.position.X += depth.X * 1.001f;

                        velocity.X = 0;

                        collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

                    }

                }

            }

            transform.position.Y += velocity.Y;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector3 depth = ColliderUtils.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.Y) > 0)
                    {

                        transform.position.Y += depth.Y * 1.001f;

                        if (velocity.Y > 0)
                        {

                            velocity.Y = -velocity.Y / 2;

                        }
                        else
                        {

                            velocity.Y = 0;

                        }

                        collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

                    }

                }

            }

            transform.position.Z += velocity.Z;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this)
                {

                    Vector3 depth = ColliderUtils.GetIntersectionDepth(collider, other.collider);

                    if (Math.Abs(depth.Z) > 0)
                    {

                        transform.position.Z += depth.Z * 1.001f;

                        velocity.Z = 0;

                        collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

                    }

                }

            }

        }

    }
}
