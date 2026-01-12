//Author: Mark Rozin
//File Name: TowerOfHanoi.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Initiliazes, updates, and draws Tower of Hanoi game with all of the pole and clicking logic and checking if its solved

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class TowerOfHanoi : Puzzle
    {
        // Array of poles used in the puzzle
        private Pole[] poles;

        // Number of rings in the puzzle
        private int ringCount;

        // Tracks the currently selected pole (null if no pole is selected)
        private int? selectedPole = null;

        // Rectangles representing the visual positions of the poles
        private Rectangle[] poleRecs;

        // Image for rendering poles
        private Texture2D poleImg;

        // Image for rendering rings
        private Texture2D ringImg;

        // Colors assigned to rings
        private Color[] ringColors;

        // Flag to indicate if the puzzle is part of a campaign
        private bool inCampaign;

        // Tracks the number of moves made by the player
        private int moveCount;

        // Flag to indicate if the middle pole is fixed or interactable
        private bool isMiddlePoleFixed;



        //Pre: ringCount is a positive integer, poleImg and ringImg are valid Texture2D objects,
        //     ringColors is a valid array of Color objects, and inCampaign and isMiddlePoleFixed are boolean values.
        //Post: Initializes the Tower of Hanoi puzzle with rings and poles in their default positions.
        //Description: Constructor to create and configure the Tower of Hanoi puzzle.
        public TowerOfHanoi(int ringCount, Texture2D poleImg, Texture2D ringImg, Color[] ringColors, bool inCampaign, bool isMiddlePoleFixed)
        {

            //Set the campaign flag, number of rings, pole and ring images, array of ring colors, middle pole's fixed state, and move count to 0, 
            this.inCampaign = inCampaign;
            this.ringCount = ringCount;
            this.poleImg = poleImg; 
            this.ringImg = ringImg;
            this.ringColors = ringColors; 
            this.moveCount = 0; 
            this.isMiddlePoleFixed = isMiddlePoleFixed;

            //Create the poles and their corresponding rectangles
            poles = new Pole[3];
            poleRecs = new Rectangle[3];

            //Cycle through for loop to initialize the poles and their rectangles
            for (int i = 0; i < 3; i++)
            {
                //Initialize the poles and their rectangles
                poles[i] = new Pole(); 
                poleRecs[i] = new Rectangle(100 + 250 * i, 150, 20, 200); 
            }

            // Add rings to the first pole, with the largest at the bottom
            for (int size = ringCount; size >= 1; size--)
            {
                //Cycle through the ring colors and add a ring to the first pole
                Color ringColor = ringColors[(size - 1) % ringColors.Length]; 
                poles[0].AddRing(new Ring(size, ringColor)); 
            }
        }

        //Pre: Valid GameTime, MouseState, and KeyboardState objects.
        //Post: Updates the game state based on user interaction.
        //Description: Handles user input for selecting poles and moving rings.
        public override void Update(GameTime gameTime, MouseState mouse, KeyboardState kb, KeyboardState prevKb)
        {
            // Check if the left mouse button is pressed and no action is already in progress
            if (mouse.LeftButton == ButtonState.Pressed && !wasMousePressed)
            {
                // Mark the button as pressed and create a bool to track if a valid click occurred
                wasMousePressed = true; 
                bool validClick = false; 

                // Check if any pole was clicked
                for (int i = 0; i < 3; i++)
                {
                    //If any of the poles were clicked handle the click and set the flag for the action
                    if (poleRecs[i].Contains(mouse.Position))
                    {
                        HandleClick(i); // Handle the pole click
                        validClick = true; // Set flag for valid action
                        break;
                    }
                }

                // Reset selection if the click was outside any pole
                if (!validClick)
                {
                    //No pole was selected
                    selectedPole = null;
                }
            }
            //If they let go of the button reset the button state
            else if (mouse.LeftButton == ButtonState.Released)
            {
                // Reset the button state
                wasMousePressed = false; 
            }
        }

        //Pre: None.
        //Post: Returns the number of moves made.
        //Description: Accessor to return the current move count.
        public int GetMoveCount()
        {
            return moveCount;
        }

        //Pre: None.
        //Post: Returns true if the puzzle is solved.
        //Description: Determines if the puzzle is solved by checking the third pole.
        


        //Pre: None.
        //Post: Returns true if the puzzle is solved.
        //Description: Determines if the puzzle is solved by checking the third pole.
        public override bool IsSolved()
        {
            // The puzzle is solved when all rings are on the third pole
            if (poles[2].RingCount == ringCount)
            {
                //Return yes its been solved
                return true;
            }

            //Return no it hasnt been solved
            return false;
        }

        //Pre: poleIndex is a valid index of the poles array.
        //Post: None.
        //Description: Handles logic for selecting or moving a ring based on the clicked pole.
        private void HandleClick(int poleIndex)
        {
            // Ensure the poleIndex is valid and within range
            if (poleIndex < 0 || poleIndex >= poles.Length) return;

            // Ignore clicks on the middle pole if it's not fixed
            if (poleIndex == 1 && !isMiddlePoleFixed) return;

            //If a pole hasn't been selected
            if (selectedPole == null)
            {
                // Only select a pole if it has rings
                if (poles[poleIndex].RingCount > 0)
                {
                    // Set the selected pole
                    selectedPole = poleIndex; 
                }
            }

            //If a pole has been selected control the moving rings logic
            else
            {
                // Get the previously selected pole and the destination pole
                int fromPole = selectedPole.Value;
                int toPole = poleIndex;

                // If there are rings on the selected pole, attempt the move
                if (poles[fromPole].RingCount > 0)
                {
                    moveCount++; // Increment the move count
                    Ring ringToMove = poles[fromPole].RemoveRing();

                    // Add the ring to the destination if the move is valid; otherwise, undo the move
                    if (poles[toPole].CanAddRing(ringToMove))
                    {
                        poles[toPole].AddRing(ringToMove);
                    }
                    else
                    {
                        poles[fromPole].AddRing(ringToMove);
                    }
                }

                // Clear the selected pole after the action
                selectedPole = null; 
            }
        }

        //Pre: None.
        //Post: Fixes the middle pole, making it interactable.
        //Description: Sets the middle pole to a fixed state.
        public void FixMiddlePole()
        {
            isMiddlePoleFixed = true;
        }

        //Pre: None.
        //Post: Returns true if the middle pole is fixed.
        //Description: Accessor to determine if the middle pole is fixed.
        public bool IsFixed()
        {
            return isMiddlePoleFixed;
        }

        //Pre: Valid SpriteBatch object.
        //Post: Draws the poles and rings to the screen.
        //Description: Renders the Tower of Hanoi puzzle.
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < 3; i++)
            {
                // Skip drawing the middle pole if it's not fixed
                if (i == 1 && !isMiddlePoleFixed) continue;

                // Draw the pole
                spriteBatch.Draw(poleImg, poleRecs[i], Color.White);

                // Draw the rings on the pole
                int yOffset = poleRecs[i].Y + poleRecs[i].Height - 20;
                foreach (var ring in poles[i].GetRings().Reverse())
                {
                    int ringWidth = 50 + ring.GetSize() * 15; // Calculate ring width based on size
                    int ringHeight = 20;
                    int xPos = poleRecs[i].X + (poleRecs[i].Width / 2) - (ringWidth / 2); // Center the ring on the pole

                    // Determine ring color based on selection
                    Color ringColor = (selectedPole == i && ring == poles[i].GetTopRing(poles[i]))
                        ? Color.Gray // Highlight color
                        : ring.GetColor(); // Default ring color

                    // Draw the ring
                    spriteBatch.Draw(ringImg, new Rectangle(xPos, yOffset, ringWidth, ringHeight), ringColor);

                    // Move to the next ring position
                    yOffset -= ringHeight; 
                }
            }
        }
    }
}
