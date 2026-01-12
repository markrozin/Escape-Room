//Author: Mark Rozin
//File Name: Clock.cs
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025
//Description: Represents a clock with adjustable hour and minute hands, checking if it reaches a specific target time.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace TestProject
{
    public class Clock
    {
        // Define the clock's visual attributes, including its center position, radius, and textures for the face and hands
        private Vector2 clockCenter;
        private float clockRadius;
        private Texture2D clockImg;
        private Texture2D handImg;

        // Define the angles for the hour and minute hands, which determine their current positions on the clock
        private float hourAngle;
        private float minuteAngle;

        // Booleans to manage the clock's behavior and state, including selection, whether the clock is fixed, and whether it's solved
        private bool isHourSelected = true;
        private bool isFixed;
        private bool isSolved;

        // Predefined angles for the target time (8:20), with the hour and minute hands pointing to their respective positions
        private float targetHourAngle = -MathHelper.PiOver2 + 8 * MathHelper.Pi / 6;
        private float targetMinuteAngle = -MathHelper.PiOver2 + 4 * MathHelper.Pi / 6;

        // Small tolerance value for comparing angles to handle floating-point imprecision
        private const float AngleTolerance = 0.01f;

        // Stores the previous state of the keyboard to detect changes in input for clock manipulation
        private KeyboardState prevKeyboardState;

        //Pre: Valid textures, center, and radius are provided; isFixed determines whether the clock has a fixed minute hand.
        //Post: Initializes the clock with default angles and textures.
        //Description: Constructs a clock with its visual and logical properties.
        public Clock(Vector2 center, float radius, Texture2D clockImg, Texture2D handImg, bool isFixed)
        {
            //Setting constructor parameters equal to class variables
            clockCenter = center;
            clockRadius = radius;
            this.clockImg = clockImg;
            this.handImg = handImg;
            this.isFixed = isFixed;

            //Initializing the hour and minute hands to point to 12 o'oclock
            hourAngle = -MathHelper.PiOver2;
            minuteAngle = -MathHelper.PiOver2;
        }

        //Pre: gameTime represents the time since the last update; keyboardState and prevKeyboardState track current and previous keyboard states.
        //Post: Updates the rotation of the clock hands and switches hand selection if applicable.
        //Description: Handles user input to adjust clock hands and checks if the clock is solved.
        public void Update(GameTime gameTime, KeyboardState keyboardState)
        {
            // Check if the Up arrow key is pressed and was not pressed previously
            if (keyboardState.IsKeyDown(Keys.Up) && !prevKeyboardState.IsKeyDown(Keys.Up))
            {
                if (isHourSelected)

                    // Increment the hour hand angle by 30 degrees (as there are 12 sections with 360 degrees total) and wrap around if it exceeds 360 degrees (2*Pi)
                    hourAngle = (hourAngle + MathHelper.Pi / 6) % MathHelper.TwoPi;
                else
                    // Increment the minute hand angle by 6 degrees (as there are 60 sections with 360 degrees total) and wrap around if it exceeds 360 degrees
                    minuteAngle = (minuteAngle + MathHelper.Pi / 30) % MathHelper.TwoPi;
            }

            // Check if the Down arrow key is pressed and was not pressed previously
            if (keyboardState.IsKeyDown(Keys.Down) && !prevKeyboardState.IsKeyDown(Keys.Down))
            {
                if (isHourSelected)
                    // Decrement the hour hand angle by 30 degrees (Pi/6 radians) and ensure it remains positive by adding 360 degrees (2*Pi)
                    hourAngle = (hourAngle - MathHelper.Pi / 6 + MathHelper.TwoPi) % MathHelper.TwoPi;
                else
                    // Decrement the minute hand angle by 6 degrees (Pi/30 radians) and ensure it remains positive
                    minuteAngle = (minuteAngle - MathHelper.Pi / 30 + MathHelper.TwoPi) % MathHelper.TwoPi;
            }

            // If the clock is fixed, check if the Spacebar key is pressed and was not pressed previously
            if (isFixed && keyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                // Toggle between selecting the hour and minute hands
                isHourSelected = !isHourSelected;
            }

            // Check if the current time on the clock matches the target time of 8:20
            if (CheckIfTimeIs820())
            {
                // Mark the clock as solved
                isSolved = true; 
            }

            // Update the previous keyboard state for comparison in the next frame
            prevKeyboardState = keyboardState;
        }

        //Pre: spriteBatch is a valid SpriteBatch object.
        //Post: Renders the clock face and its hands.
        //Description: Draws the clock in its current state with appropriate rotation of the hands.
        public void Draw(SpriteBatch spriteBatch)
        {
            //Draw the clock itself as well as the hour hand
            spriteBatch.Draw(
                clockImg,
                new Rectangle((int)(clockCenter.X - clockRadius), (int)(clockCenter.Y - clockRadius), (int)(clockRadius * 2), (int)(clockRadius * 2)),
                Color.White
            );
            DrawHand(spriteBatch, hourAngle, clockRadius * 0.5f, Color.Blue);

            //If the clock is fixed, draw the minute hand
            if (isFixed)
            {
                DrawHand(spriteBatch, minuteAngle, clockRadius * 0.8f, Color.Red);
            }
        }

        //Pre: None.
        //Post: Returns true if the hour and minute hands are at the target positions.
        //Description: Determines if the clock is set to the target time.
        private bool CheckIfTimeIs820()
        {
            //Bool to check if the hour angles and minute angles are aligning with the target 8:20 time, using angle tolerance as well to acount for floating point imprecision
            return Math.Abs(hourAngle - targetHourAngle) < AngleTolerance &&
                   Math.Abs(minuteAngle - targetMinuteAngle) < AngleTolerance;
        }

        //Pre: spriteBatch is a valid SpriteBatch object; angle and length define the hand's orientation and size; color specifies the hand's color.
        //Post: Renders the specified clock hand at the given angle and size.
        //Description: Draws a clock hand as a colored rectangle rotated around the clock center.
        private void DrawHand(SpriteBatch spriteBatch, float angle, float length, Color color)
        {
            // Calculate the endpoint of the hand using trigonometry (angle determines direction)
            Vector2 handEnd = new Vector2(
                clockCenter.X + length * (float)Math.Cos(angle), 
                clockCenter.Y + length * (float)Math.Sin(angle) 
            );

            // Calculate the direction vector from the center to the endpoint
            Vector2 direction = handEnd - clockCenter;

            // Determine the actual length of the hand based on the direction vector
            float handLength = direction.Length();

            // Draw the hand as a thin rectangle rotated around the clock center
            spriteBatch.Draw(handImg,new Rectangle((int)clockCenter.X, (int)clockCenter.Y, (int)handLength, 4), null, color,  angle, new Vector2(0, 2), SpriteEffects.None, 0);
        }

        //Pre: None.
        //Post: Returns true if the clock has a fixed minute hand.
        //Description: Checks if the clock's minute hand is fixed.
        public bool IsFixed()
        {
            return isFixed;
        }

        //Pre: None.
        //Post: Resets the clock to its initial state, with only the hour hand pointing up.
        //Description: Reinitializes the clock to have one hand pointing at 12 o'clock and resets its state.
        public void ResetClock()
        {
            // Reset the angles for the hands
            hourAngle = -MathHelper.PiOver2; // 12 o'clock position
            minuteAngle = -MathHelper.PiOver2; // 12 o'clock position

            // Ensure the minute hand is not fixed
            isFixed = false;

            // Mark the clock as unsolved
            isSolved = false;

            // Revert to hour hand being selected
            isHourSelected = true;
        }


        //Pre: None.
        //Post: Sets the clock's minute hand to a fixed state.
        //Description: Fixes the clock's minute hand to prevent further movement.
        public void FixClock()
        {
            isFixed = true;
        }

        //Pre: None.
        //Post: Returns true if the clock is solved.
        //Description: Checks if the clock has been set to the target time.
        public bool IsSolved()
        {
            return isSolved;
        }
    }
}