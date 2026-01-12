//Author: Mark Rozin
//File Name: Scroll.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Represents a scroll object with states for lying on the floor and close-up inspection, including symbol revealing functionality.

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TestProject
{
    public class Scroll
    {
        // Define the images for the scroll's different states and its symbol overlay
        private Texture2D lyingImage;
        private Texture2D closeUpImage;
        private Texture2D symbolsImage;

        // Tracks whether the scroll's symbols have been revealed
        private bool isRevealed;

        // Represents the scroll's position on the screen
        private Rectangle scrollRec;

        //Pre: Valid images are provided for the scroll's lying state, close-up, and symbols; position is a valid rectangle.
        //Post: Initializes the scroll with its images, position, and default revealed state.
        //Description: Constructs a scroll object with images for different states and a bounding rectangle.
        public Scroll(Texture2D lyingImage, Texture2D closeUpImage, Texture2D symbolsImage, Rectangle position)
        {
            this.lyingImage = lyingImage;
            this.closeUpImage = closeUpImage;
            this.symbolsImage = symbolsImage;
            this.scrollRec = position;
            isRevealed = false;
        }

        //Pre: None.
        //Post: Sets the scroll's state to revealed.
        //Description: Marks the scroll as having its symbols revealed.
        public void RevealSymbols()
        {
            isRevealed = true;
        }

        //Pre: None.
        //Post: Returns true if the scroll's symbols have been revealed.
        //Description: Checks if the scroll's symbols are revealed.
        public bool IsFixed()
        {
            return isRevealed;
        }

        //Pre: spriteBatch is a valid SpriteBatch object; isInspecting is true if the scroll is being viewed close-up.
        //Post: Draws the scroll in its current state (lying or close-up with/without symbols).
        //Description: Renders the scroll depending on its current state and inspection status.
        public void Draw(SpriteBatch spriteBatch, bool isInspecting)
        {
            if (isInspecting)
            {
                // Draw the close-up images of the scroll
                spriteBatch.Draw(closeUpImage, new Rectangle(50, 50, (int)(closeUpImage.Width * 0.38), (int)(closeUpImage.Height * 0.22)), Color.White);

                // If revealed, draw the symbols images
                if (isRevealed)
                {
                    spriteBatch.Draw(symbolsImage, new Rectangle(140, 150, (int)(symbolsImage.Width * 1.4), (int)(symbolsImage.Height * 1.4)), Color.White);
                }
            }
            else
            {
                // Draw the scroll in its lying state
                spriteBatch.Draw(lyingImage, scrollRec, Color.White);
            }
        }

        //Pre: point is a valid Point object.
        //Post: Returns true if the point is inside the scroll's bounding rectangle.
        //Description: Determines if the scroll has been clicked by the player.
        public bool Contains(Point point)
        {
            return scrollRec.Contains(point);
        }

        //Pre: None.
        //Post: Returns the scroll's bounding rectangle.
        //Description: Provides the current screen position and dimensions of the scroll.
        public Rectangle GetScrollRec()
        {
            return scrollRec;
        }
    }
}