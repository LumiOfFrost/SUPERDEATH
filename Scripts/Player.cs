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

        public bool isGrounded = false;

        float coyoteTime = 0f;

        bool dashJumping = false;
        
        public float stamina = 3f;

        public float staminaCooldown = 0f;

        float jumpBuffer = 0f;

        public Camera camera;

        Vector3 cameraLocalPosition;

        public Player(Transform tform, Vector3 relativeCameraPos) : base(tform, null, null, RenderType.None, "Player", false)
        {

            transform = tform;

            cameraLocalPosition = relativeCameraPos;

            camera = new Camera(Vector3.Forward, Vector3.Up, new Vector3(transform.position.X + relativeCameraPos.X, transform.position.Y + relativeCameraPos.Y, transform.position.Z + relativeCameraPos.Z));

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
            
            if(velocity.Y > 0)
            {

                isGrounded = false;

            }

            if (coyoteTime < 0 || isGrounded)
            {

                dashJumping = false;

            }

            if (InputManager.Reset())
            {

                transform.position = new Vector3(0, 30, 0);
                velocity = Vector3.Zero;

            }

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

                jumpBuffer = 0.15f;

                

            }

            if (isGrounded)
            {

                coyoteTime = 0.2f;

            }

            if (jumpBuffer > 0 && coyoteTime > 0)
            {

                velocity.Y = (dashJumping ? 15 : 20) * Main.gameSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                coyoteTime = 0;
                jumpBuffer = 0;
                Assets.jumpSound.Play(0.25f, 0, 0);
                isGrounded = false;

            }

            if (!isGrounded && velocity.X < moveSpeed * 2 && velocity.Z < moveSpeed * 2)
            {

                InputManager.movementControl = 0.35f;

            } else if (!isGrounded)
            {

                InputManager.movementControl = 0.15f;

            } else
            {

                InputManager.movementControl = 1;

            }

            velocity.Y -= 9.8f * 4 * (float)gameTime.ElapsedGameTime.TotalSeconds * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 direction = camera.forward;
            direction.Normalize();

            Vector3 normal = Vector3.Cross(direction, camera.up);

            mouseMovement.Y *= main.GraphicsDevice.Viewport.Height / 800.0f;
            mouseMovement.X *= main.GraphicsDevice.Viewport.Width / 1280.0f;

            if (InputManager.LookVector() != Vector2.Zero)
            {
                mouseMovement = InputManager.LookVector();
            }

            camera.forward += mouseMovement.X * mouseSpeed * normal;

            camera.forward -= mouseMovement.Y * mouseSpeed * camera.up;
            camera.forward.Normalize();

            camera.forward.X = MathHelper.Clamp(camera.forward.X, MathHelper.ToRadians(-75), MathHelper.ToRadians(75));
            camera.forward.Z = MathHelper.Clamp(camera.forward.Z, MathHelper.ToRadians(-75), MathHelper.ToRadians(75));

            if (InputManager.Dash() && stamina >= 1.5f)
            {

                if (movementVelocity.X != 0 || movementVelocity.Z != 0)
                {

                    velocity += MathUtils.NormalizeVector3(movementVelocity) * 50 * (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

                } else
                {

                    velocity += movementVTW.Forward * 50 * (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

                }

                velocity.Y = 5 * (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

                Assets.dashSound.Play(0.25f, 0, 0);

                stamina -= 1.5f;

                staminaCooldown = 1;

            }

            movementVelocity.Y = velocity.Y;

            velocity = Vector3.Lerp(velocity, movementVelocity, 0.16f * InputManager.movementControl * Main.gameSpeed) * Main.gameSpeed;

            jumpBuffer -= (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

            coyoteTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

            staminaCooldown -= (float)gameTime.ElapsedGameTime.TotalSeconds * Main.gameSpeed;

            if(staminaCooldown <= 0) stamina += (float)gameTime.ElapsedGameTime.TotalSeconds * 1.5f * Main.gameSpeed;

            stamina = Math.Clamp(stamina, 0, 3);

            CheckForCollision(LevelManager.currentLevel.gameObjects);

            if (main.IsActive && !Main.paused)
            {

                Mouse.SetPosition(main._graphics.GraphicsDevice.Viewport.Width / 2, main._graphics.GraphicsDevice.Viewport.Height / 2);

            }
        
        }

        void CheckForCollision(List<GameObject> gameObjects)
        {

            transform.position.X += velocity.X;

            collider = new BoundingBox(transform.position - transform.scale / 2, transform.position + transform.scale / 2);

            foreach (GameObject other in gameObjects)
            {

                if (collider.Intersects(other.collider) && other != this && other.solid)
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

                if (collider.Intersects(other.collider) && other != this && other.solid)
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

                            isGrounded = true;

                            staminaCooldown = 0;

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

                if (collider.Intersects(other.collider) && other != this && other.solid)
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

            Debug.WriteLine(transform.position.ToString());

        }

    }
}
