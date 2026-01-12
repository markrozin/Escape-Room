//Author: Mark Rozin
//File Name: PickupableItem.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Represents a pickupable item with logic for picking it up, updating its bounds, and drawing it

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class PickupableItem
    {
        // Image representation of the item
        private Texture2D img;

        // Position of the item on the screen
        private Vector2 position;

        // Collision rectangle for the item
        private Rectangle bounds;

        // Tracks whether the item is currently picked up by the player
        private bool isPickedUp;

        // Name of the item
        private string name;

        //Pre: img is a valid image; position is the initial position of the item; width and height define the item's dimensions.
        //Post: Initializes a PickupableItem with its image, position, size, and name.
        //Description: Creates an item that can be interacted with, including its attributes and bounding box.
        public PickupableItem(Texture2D img, Vector2 position, int width, int height, string name)
        {
            //Sets inputted paramters to class variables and sets the item to is not picked up
            this.img = img;
            this.position = position;
            this.name = name;
            isPickedUp = false;

            // Set the item's collision bounds based on its position and size
            bounds = new Rectangle((int)position.X, (int)position.Y, width, height);
        }

        //Pre: None.
        //Post: Updates the item's collision bounds to match its current position.
        //Description: Synchronizes the collision rectangle with the item's position.
        public void UpdateBounds()
        {
            bounds = new Rectangle((int)position.X, (int)position.Y, bounds.Width, bounds.Height);
        }

        //Pre: spriteBatch is a valid SpriteBatch object.
        //Post: Draws the item if it is not picked up.
        //Description: Renders the item's image to the screen when it is active.
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!isPickedUp)
            {
                spriteBatch.Draw(img, bounds, Color.White);
            }
        }

        //Pre: None.
        //Post: Returns the item's current pickup state.
        //Description: Checks whether the item is currently picked up.
        public bool GetItemState()
        {
            return isPickedUp;
        }

        //Pre: state is a boolean indicating the new pickup state.
        //Post: Updates the item's pickup state.
        //Description: Sets whether the item is picked up or not.
        public void SetItemState(bool state)
        {
            isPickedUp = state;
        }

        //Pre: None.
        //Post: Returns the item's collision rectangle.
        //Description: Provides access to the item's bounding box for collision checks.
        public Rectangle GetItemBounds()
        {
            return bounds;
        }

        //Pre: None.
        //Post: Returns the item's name
        //Description: Provides access to the item's name for display purposes
        public string GetItemName()
        {
            return name;
        }

        //Pre: None.
        //Post: Returns the item's current position.
        //Description: Provides access to the item's position.
        public Vector2 GetItemPosition()
        {
            return position;
        }

        //Pre: pos is a position indicating a new position
        //Post: Updates the item's position
        //Description: Sets the item's new position
        public void SetItemPosition(Vector2 pos)
        {
            position = pos;
        }

        //Pre: None.
        //Post: Returns the item's image.
        //Description: Provides access to the image used to represent the item.
        public Texture2D GetItemImg()
        {
            return img;
        }
    }
}
