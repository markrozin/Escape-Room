//Author: Mark Rozin
//File Name: Puzzle.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Parent class for all the puzzles with general Update, Draw, and IsSolved methods

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

    //Provides a structure for implementing common puzzle functionality
    public abstract class Puzzle
    {
        // Tracks whether the mouse was pressed in the previous frame
        protected bool wasMousePressed;

        //Initializes all puzzles to false
        public Puzzle()
        {
            wasMousePressed = false; // Initialize to false
        }

        // Determines whether the puzzle is solved.
        public abstract bool IsSolved();

        // Updates the puzzle's state based on user input and game time.
        public abstract void Update(GameTime gameTime, MouseState mouse, KeyboardState kb, KeyboardState prevKb);

        // Draws the puzzle elements to the screen.
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}
