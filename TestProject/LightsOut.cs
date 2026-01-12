//Author: Mark Rozin
//File Name: LightsOut.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Initiliazes, updates, and draws Tower of Hanoi game with all of the clicking logic and checking if its solved

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
    public class LightsOut : Puzzle
    {
        // Define gridsize, cellsize, rectangles, states, and images for the lights, moveCount, and the campaign flag
        private int gridSize;
        private Rectangle[,] lightRecs;
        private bool[,] lightStates;
        private Texture2D lightImg;
        private int cellSize;
        private int moveCount;
        private bool inCampaign;

        //Pre: lightImg is a valid Texture2D object, gridSize, gridStartX, gridStartY, and cellSize are positive integers,
        //     and inCampaign is a boolean indicating the game mode.
        //Post: Initializes the Lights Out puzzle with a grid of togglable lights and generates a solvable puzzle state.
        //Description: Constructor to initialize the Lights Out puzzle, setting up grid properties and states.
        public LightsOut(Texture2D lightImg, int gridSize, int gridStartX, int gridStartY, int cellSize, bool inCampaign)
        {
            //Set the inputted parameters to the class variables and set move count to -1 to account for click on entry
            this.inCampaign = inCampaign;
            this.lightImg = lightImg;
            this.gridSize = gridSize;
            this.cellSize = cellSize;
            this.moveCount = -1;

            // Initialize the grid rectangles and states
            lightRecs = new Rectangle[gridSize, gridSize];
            lightStates = new bool[gridSize, gridSize];

            //Populate the light rectangles with a nested for loop
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    lightRecs[i, j] = new Rectangle(
                        gridStartX + (j * cellSize),
                        gridStartY + (i * cellSize),
                        cellSize,
                        cellSize
                    );
                }
            }

            // Generate a randomized solvable puzzle state (for what lights are on and off)
            GenerateSolvablePuzzle(); 
        }

        //Pre: gameTime is the current GameTime object, mouse and kb are the current input states,
        //     and prevKb is the previous keyboard state.
        //Post: Updates the Lights Out puzzle based on player input and game state changes.
        //Description: Handles player input for toggling lights and tracks puzzle progress.
        public override void Update(GameTime gameTime, MouseState mouse, KeyboardState kb, KeyboardState prevKb)
        {
            // Handle what happens if mouse was clicked
            if (mouse.LeftButton == ButtonState.Pressed && !wasMousePressed)
            {
                //Setting mouse pressed to prevent multiple toggles from a single mouse click
                wasMousePressed = true;

                // Check which light cell was clicked
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        //If a light was clicked, changes its state along with its neighbours (through ToggleLights method), and update move count
                        if (lightRecs[i, j].Contains(mouse.Position))
                        {
                            //Toggles the click to its neighbours, increments the move count, and returns to ensure one click only
                            ToggleLights(i, j, 0); 
                            moveCount++;
                            return;
                        }
                    }
                }
            }

            //If they release the button, reset mouse press state
            else if (mouse.LeftButton == ButtonState.Released)
            {
                //State mouse hasn't been pressed
                wasMousePressed = false; 
            }
        }

        //Pre: spriteBatch is a valid SpriteBatch object.
        //Post: Renders the Lights Out puzzle grid with current light states.
        //Description: Draws the Lights Out puzzle grid, displaying lights in their on/off states.
        public override void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    // Draw each light cell with appropriate color based on its state
                    Color lightColor = lightStates[i, j] ? Color.Pink : Color.Purple;
                    spriteBatch.Draw(lightImg, lightRecs[i, j], lightColor);
                }
            }
        }

        //Pre: x and y are valid grid indices, and depth is an integer tracking the current recursion level.
        //Post: Toggles the state of the light at (x, y) and its immediate neighbors, stopping recursion after one level.
        //Description: Recursively toggles the clicked light and its neighbors, using a depth parameter to prevent infinite recursion.
        private void ToggleLights(int x, int y, int depth)
        {
            // Base case: Stop recursion if out of bounds or depth exceeds 1
            if (x < 0 || x >= gridSize || y < 0 || y >= gridSize || depth > 1)
            {
                return;
            }

            // Toggle the current light
            lightStates[x, y] = !lightStates[x, y];

            // Recursive calls to lights above, below, left, right, and a depth tracker to avoid infinite recursion
            ToggleLights(x - 1, y, depth + 1);
            ToggleLights(x + 1, y, depth + 1);
            ToggleLights(x, y - 1, depth + 1);
            ToggleLights(x, y + 1, depth + 1);
        }


        //Pre: None.
        //Post: Randomizes the light grid to create a solvable Lights Out puzzle.
        //Description: Generates a random starting state for the puzzle that can be solved.
        private void GenerateSolvablePuzzle()
        {
            // Create a random number generator to determine random light toggles
            Random random = new Random();

            // Randomly determine how many lights to toggle to create the puzzle
            int toggleCount = random.Next(gridSize * gridSize / 2, gridSize * gridSize);

            // Perform the toggles at random grid positions to ensure the puzzle is solvable
            for (int i = 0; i < toggleCount; i++)
            {
                //Toggle the light at random X and Y coordinates to ensure a solvable puzzle
                int x = random.Next(0, gridSize);
                int y = random.Next(0, gridSize); 
                ToggleLights(x, y, 0); 
            }
        }

        //Pre: None.
        //Post: Returns the total number of moves made by the player.
        //Description: Accessor to return the player's move count.
        public int GetMoveCount()
        {
            return moveCount;
        }

        //Pre: None.
        //Post: Returns true if all lights are turned off, otherwise false.
        //Description: Checks if the Lights Out puzzle is solved.
        public override bool IsSolved()
        {
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //Cycle through all of the lights and if ANY of them are off, its not solved
                    if (!lightStates[i, j])
                    {
                        return false;
                    }
                }
            }

            //All lights are indeed off
            return true;
        }
    }
}