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

        public static bool Jump(KeyboardState prevKeyState, GamePadState prevPadState)
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

        public static bool Fall(KeyboardState prevKeyState, GamePadState prevPadState)
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
