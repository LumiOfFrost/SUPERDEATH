using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace SUPERDEATH.Scripts
{
    public class InputManager
    {

        public static bool inputActive = true;

        public static float leftDeadzone = 0.15f;

        public static KeyboardState prevKeyState;
        public static GamePadState prevPadState;

        public static float movementControl = 1;

        public static bool Reset()
        {

            if (
                
                Keyboard.GetState().IsKeyDown(Keys.NumPad0) && !prevKeyState.IsKeyDown(Keys.NumPad0) && inputActive
               
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        public static bool Dash()
        {

            if (

                Keyboard.GetState().IsKeyDown(Keys.LeftShift) && !prevKeyState.IsKeyDown(Keys.LeftShift) && inputActive

                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        public static bool IsMovingUp()
        {

            if ((Keyboard.GetState().IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y > leftDeadzone) && inputActive)
            {

                return true;

            }
            else
            {

                return false;

            }

        }
        public static bool IsMovingDown()
        {

            if ((Keyboard.GetState().IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y < -leftDeadzone) && inputActive)
            {

                return true;

            }
            else
            {

                return false;

            }

        }
        public static bool IsMovingLeft()
        {

            if ((Keyboard.GetState().IsKeyDown(Keys.A) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < -leftDeadzone) && inputActive)
            {

                return true;

            }
            else
            {

                return false;

            }

        }
        public static bool IsMovingRight()
        {

            if ((Keyboard.GetState().IsKeyDown(Keys.D) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > leftDeadzone) && inputActive)
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        public static bool Jump()
        {

            if (
                (
                (Keyboard.GetState().IsKeyDown(Keys.Space) && !prevKeyState.IsKeyDown(Keys.Space)) || 
                (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) && !prevPadState.IsButtonDown(Buttons.A))) && 
                inputActive
                )
            {

                return true;

            }else
            {

                return false;

            }

        }

        public static bool Pause()
        {

            if (
                (
                (Keyboard.GetState().IsKeyDown(Keys.Escape) && !prevKeyState.IsKeyDown(Keys.Escape)) ||
                (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.Start) && !prevPadState.IsButtonDown(Buttons.Start))) &&
                inputActive
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        public static bool Fullscreen()
        {

            if (
                (
                (Keyboard.GetState().IsKeyDown(Keys.F11) && !prevKeyState.IsKeyDown(Keys.F11))
                ) &&
                inputActive
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

        public static bool Fall()
        {

            if (
                (
                (!Keyboard.GetState().IsKeyDown(Keys.Space) && prevKeyState.IsKeyDown(Keys.Space)) ||
                (!GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A) && prevPadState.IsButtonDown(Buttons.A))) &&
                inputActive
                )
            {

                return true;

            }
            else
            {

                return false;

            }

        }

    }
}
