//Author: Mark Rozin
//File Name: Pole.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Represents a pole in the Tower of Hanoi game, managing a stack of rings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class Pole
    {
        // A stack to hold the rings on the pole
        private MyStack<Ring> rings;

        // Constructor initializes an empty stack for the rings
        public Pole()
        {
            //Initialize the ring stack
            rings = new MyStack<Ring>();
        }


        //Pre: Ring is a valid ring that is being moved 
        //Post: Return a bool stating if the ring can be moved
        //Description: Checks if a ring can be added to the pole with a parameter of the ring being moved
        public bool CanAddRing(Ring ring)
        {
            //Returns a true boolean if the pole is empty or the top ring is larger than the new ring
            return rings.count == 0 || rings.Peek().GetSize() > ring.GetSize();
        }

         
        //Pre: Ring is a valid ring object that is being moved
        //Post: None
        //Description: Adds a ring to the pole if the rules allow it, otherwise throws an exception
        public void AddRing(Ring ring)
        {
            //If it can add a ring, it pushes that ring onto a new stack
            if (!CanAddRing(ring))
                throw new InvalidOperationException("Cannot place a larger ring on a smaller ring.");
            rings.Push(ring);
        }

        //Pre: None
        //Post: Returns the top ring of a pole and removes it
        //Description: Removes and returns the top ring of the pole
        public Ring RemoveRing()
        {
            //If the stack is empty throw an exception if not remove the ring
            if (rings.count == 0)
                throw new InvalidOperationException("No rings on this pole.");
            return rings.Pop();
        }

        //Pre: The pole is a valid pole object
        //Post: Returns the top ring of a pole
        //Description: Gets the top ring of a pole if there is one
        public Ring GetTopRing(Pole pole)
        {
            //If the the pole isn't empty, return the top ring
            if (pole.rings.count > 0)
            {
                // Return the top ring
                return pole.rings.Peek(); 
            }

            // Return null if no rings are on the pole
            return null; 
        }


        //Pre: None
        //Post: Returns a list of rings on the pole in bottom-to-top order
        //Description: Retrieves all rings from the pole while preserving the stack's original order
        public IEnumerable<Ring> GetRings()
        {
            // Temporary list to store rings in reverse order
            List<Ring> reversedRings = new List<Ring>();

            //Remove each ring from the stack and add it to the temporary list
            while (rings.count > 0)
            {
                // Remove each ring from the stack and add it to the temporary list
                Ring ring = rings.Pop();      
                reversedRings.Add(ring);     
            }

            // Push the rings back onto the stack in their original order
            for (int i = reversedRings.Count - 1; i >= 0; i--)
            {
                // Restore rings to their original order
                rings.Push(reversedRings[i]);
            }

            // Return the list of rings in bottom-to-top order
            return reversedRings; 
        }

        // Property to get the number of rings on the pole
        public int RingCount => rings.count;
    }
}
