//Author: Mark Rozin
//File Name: Ring.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Represents a Ring in Tower of Hanoi, with size and color as built is attributes

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class Ring
    {
        //Size and color attributes of the ring
        private int Size;
        private Color Color;

        //Pre: Size is a positive integer and color is a valid Color.
        //Post: Initializes a Ring object with the given size and color.
        //Description: Constructor to create a new Ring object.
        public Ring(int size, Color color)
        {
            //Sets the attributes passed into the constructor as the class variables to be used in the class
            Size = size;
            Color = color;
        }

        //Pre: None.
        //Post: Returns the size of the ring.
        //Description: Accessor to return the size of the ring.
        public int GetSize()
        {
            return Size;
        }

        //Pre: None.
        //Post: Returns the color of the ring.
        //Description: Accessor to return the color of the ring.
        public Color GetColor()
        {
            return Color;
        }
    }
}
