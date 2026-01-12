//Author: Mark Rozin
//File Name: Sliding.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Initiliazes, updates, and draws Sliding game with all of the clicking logic, moving the tiles, and checking if its solved

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
    public class Sliding : Puzzle
    {
        // Define the grid related attributes for the Sliding puzzle, including grid size, tile layout, and graphics
        private int gridSize;
        private int[,] tiles;
        private Point blankTile;
        private Rectangle[,] tileRects;
        private Rectangle[,] imgSlices;
        private Texture2D tileImg;
        private Texture2D blankImg;
        private int tileSize;

        //Define the campaign tracker, move count, and if a mouse has been pressed
        private bool inCampaign;
        private int moveCount;

        //Pre: tileImg and blankImg are valid images; initialLayout is a grid configuration of integers; 
        //     startX and startY are the starting coordinates for the grid; tileSize is a positive integer.
        //Post: Initializes the Sliding puzzle with a predefined layout and image configurations.
        //Description: Sets up the Sliding puzzle, including grid tiles, blank tile tracking, and image slicing.
        public Sliding(Texture2D tileImg, Texture2D blankImg, int[,] initialLayout, int startX, int startY, int tileSize, bool inCampaign)
        {
            //Sets the inputted paramters to the class variables, intializes move count to 0
            this.inCampaign = inCampaign;
            this.tileImg = tileImg;
            this.blankImg = blankImg;
            this.gridSize = initialLayout.GetLength(0);
            this.tileSize = tileSize;
            this.moveCount = 0;

            // Initialize the grid structure for tiles, their rectangles, and image slices
            tiles = new int[gridSize, gridSize];
            tileRects = new Rectangle[gridSize, gridSize];
            imgSlices = new Rectangle[gridSize, gridSize];

            // Calculate the dimensions of each tile in the image
            int tileWidth = tileImg.Width / gridSize;
            int tileHeight = tileImg.Height / gridSize;

            // Loop through the grid to assign tile values, rectangles, and image slices
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    tiles[i, j] = initialLayout[i, j]; // Assign tile value
                    tileRects[i, j] = new Rectangle(startX + j * tileSize, startY + i * tileSize, tileSize, tileSize); // Define tile rectangle

                    if (tiles[i, j] == 0)
                        blankTile = new Point(i, j); // Identify the blank tile's position

                    imgSlices[i, j] = new Rectangle(j * tileWidth, i * tileHeight, tileWidth, tileHeight); // Define the image slice
                }
            }
        }

        //Pre: gameTime is the time elapsed since the last update; mouse is the current mouse state; 
        //     kb and prevKb represent the current and previous keyboard states.
        //Post: Updates the game logic, including handling tile movement on mouse input.
        //Description: Tracks mouse clicks to identify and move tiles if adjacent to the blank tile.
        public override void Update(GameTime gameTime, MouseState mouse, KeyboardState kb, KeyboardState prevKb)
        {
            //If the mouse was pressed 
            if (mouse.LeftButton == ButtonState.Pressed && !wasMousePressed && !IsSolved())
            {
                //Avoid multiple entrys in one click
                wasMousePressed = true;

                //Loop through all of the tiles and if one is clicked, try moving it
                for (int i = 0; i < gridSize; i++)
                {
                    for (int j = 0; j < gridSize; j++)
                    {
                        //If they clicked a tile that its blank, try the move it
                        if (tileRects[i, j].Contains(mouse.Position) && tiles[i, j] != 0)
                        {
                            //Try to move the tile if it was adjacent to the blank
                            TryMoveTile(i, j);
                            return;
                        }
                    }
                }
            }

            //If its released reset mouse state
            else if (mouse.LeftButton == ButtonState.Released)
            {
                wasMousePressed = false;
            }
        }

        //Pre: spriteBatch is a valid SpriteBatch object.
        //Post: Draws all the tiles on the screen, including the blank tile.
        //Description: Renders the Sliding puzzle grid, with each tile drawn in its current position.
        public override void Draw(SpriteBatch spriteBatch)
        {
            // Iterate through the grid to draw each tile
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    //Draw the numbered tiles (not the blank one)
                    if (tiles[i, j] != 0)
                    {
                        // Convert tile value to zero-based index
                        int tileNumber = tiles[i, j] - 1;

                        // Determine the row in the image slice
                        int sourceRow = tileNumber / gridSize;

                        // Determine the column in the image slice
                        int sourceCol = tileNumber % gridSize;

                        //Draw each tile with allocated rectangle and image slice
                        spriteBatch.Draw(
                            tileImg,
                            tileRects[i, j],
                            imgSlices[sourceRow, sourceCol],
                            Color.White // Draw color
                        );
                    }
                    else

                        // Draw the blank tile
                        spriteBatch.Draw(blankImg, tileRects[i, j], Color.White);
                }
            }
        }
    

        //Pre: row and col are valid grid indices within the bounds of the puzzle grid.
        //Post: Swaps the selected tile with the blank tile if they are adjacent.
        //Description: Moves a tile to the blank space and updates the blank tile's position.
        private void TryMoveTile(int row, int col)
        {
            //If the clicked tile is adjacent to the blank, move the tile, swap them and increment the move count
            if (IsAdjacentToBlank(row, col))
            {
                tiles[blankTile.X, blankTile.Y] = tiles[row, col];
                tiles[row, col] = 0;
                blankTile = new Point(row, col);
                moveCount++;
            }
        }

        //Pre: row and col are valid grid indices.
        //Post: Returns true if the specified tile is adjacent to the blank tile, otherwise false.
        //Description: Determines if a tile is eligible to move by checking adjacency to the blank tile.
        private bool IsAdjacentToBlank(int row, int col)
        {
            // Check if the clicked tile is adjacent to the blank tile (either horizontally or vertically)
            return (Math.Abs(blankTile.X - row) == 1 && blankTile.Y == col) ||
                   (Math.Abs(blankTile.Y - col) == 1 && blankTile.X == row);
        }

        //Pre: None.
        //Post: Returns the total number of moves made by the player.
        //Description: Provides access to the player's current move count.
        public int GetMoveCount()
        {
            return moveCount;
        }

        //Pre: None.
        //Post: Returns the tile image used in the Sliding puzzle.
        //Description: Provides access to the image of the Sliding puzzle tiles.
        public Texture2D GetSlidingImg()
        {
            return tileImg;
        }

        //Pre: None.
        //Post: Returns true if the tiles are arranged in ascending order, otherwise false.
        //Description: Checks if the puzzle is solved by verifying the arrangement of tiles.
        public override bool IsSolved()
        {
            // Tracks the expected tile value
            int count = 1; 

            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    // If the last tile (blank) is reached, the puzzle is solved
                    if (i == gridSize - 1 && j == gridSize - 1) return true;

                    // If a tile is out of order, the puzzle is not solved
                    if (tiles[i, j] != count++) return false;
                }
            }
            // All tiles are in order
            return true; 
        }
    }
}