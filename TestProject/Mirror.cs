//Author: Mark Rozin
//File Name: Mirror.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Represents a mirror object in the game, with functionality for attaching to edges, detecting collisions, and rendering.

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class Mirror
    {
        // Define the attributes of the mirror, including its position, image, bounds, and the screen dimensions.
        private Vector2 position;
        private Texture2D image;
        private Rectangle bounds;
        private int screenWidth;
        private int screenHeight;

        //Pre: texture is a valid Texture2D object, position is a Vector2 representing the starting position,
        //     screenWidth and screenHeight are positive integers defining the screen dimensions.
        //Post: Initializes a Mirror object with the given texture, position, and screen dimensions.
        //Description: Constructor to initialize the mirror with its texture, position, and screen dimensions.
        public Mirror(Texture2D image, Vector2 position, int screenWidth, int screenHeight)
        {
            //Sets the inputter constructor paramters to the class variables and sets the initial bounds of the mirror
            this.image = image;
            this.position = position;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            UpdateBounds(); 
        }

        //Pre: None.
        //Post: Updates the bounding rectangle of the mirror to match its position and texture size.
        //Description: Updates the mirror's bounding rectangle based on its current position and texture size.
        private void UpdateBounds()
        {
            //Updates bounds by making a new rectangle of the mirror
            bounds = new Rectangle((int)position.X, (int)position.Y, image.Width, image.Height);
        }

        //Pre: newPos is a valid Vector2 representing the new position of the mirror.
        //Post: Sets the position of the mirror to the specified value.
        //Description: Sets the position of the mirror to a new value.
        public void SetPosition(Vector2 newPos)
        {
            // Sets the new position & updates the bounds to match the new position
            position = newPos;
            UpdateBounds(); 
        }

        //Pre: None.
        //Post: Returns true if the specified point is within the mirror's bounds, otherwise false.
        //Description: Checks if a given point is within the mirror's bounds.
        public bool ContainsPoint(Vector2 point)
        {
            //Returns true is the point intersected the mirror (used with the lasers)
            return bounds.Contains(point);
        }

        //Pre: None.
        //Post: Returns the bounding rectangle of the mirror.
        //Description: Accessor to return the mirror's bounding rectangle.
        public Rectangle GetBounds()
        {
            //Returns bounds of mirror
            return bounds;
        }

        //Pre: None.
        //Post: Returns the current position of the mirror.
        //Description: Accessor to return the position of the mirror.
        public Vector2 GetPosition()
        {
            //Returns position of mirror
            return position;
        }

        //Pre: newRect is a valid Rectangle object representing the new bounds.
        //Post: Sets the mirror's bounding rectangle to the specified value.
        //Description: Updates the mirror's bounding rectangle.
        public void SetBounds(Rectangle newRec)
        {
            //Sets the bounds of the mirror
            bounds = newRec;
        }

        //Pre: spriteBatch is a valid SpriteBatch object, color is a valid Color,
        //     scale is a positive float value representing the scale of the drawing.
        //Post: Renders the mirror on the screen at its current position and scale.
        //Description: Draws the mirror to the screen with the specified scale and color.
        public void Draw(SpriteBatch spriteBatch, Color color, float scale = 1f)
        {
            //Draws the mirror
            spriteBatch.Draw(image, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
    }
}

