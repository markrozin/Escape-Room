//Author: Mark Rozin
//File Name: PickupableItem.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Represents a leaderboard entry with the player's name, time score, and move score as attributes

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class LeaderboardEntry
    {
        //Attributes of the player's name, time score, and move count
        private string playerName;
        private double time;
        private int moves;   

        public LeaderboardEntry(string playerName, double time, int moves)
        {
            //Setting the inputted constructor parameters to the class variables
            this.playerName = playerName;
            this.time = time;
            this.moves = moves;
        }

        //Pre: None.
        //Post: Returns the player's name.
        //Description: Provides access to the player's name.
        public string GetPlayerName()
        {
            return playerName;
        }

        //Pre: name is a string representing the new player name.
        //Post: Updates the player's name.
        //Description: Sets the player's name to the provided value.
        public void SetPlayerName(string name)
        {
            playerName = name;
        }

        //Pre: None.
        //Post: Returns the recorded game time.
        //Description: Provides access to the game's elapsed time.
        public double GetTime()
        {
            return time;
        }

        //Pre: timeScore is a double representing the new game time.
        //Post: Updates the recorded game time.
        //Description: Sets the game's elapsed time to the provided value.
        public void SetTime(double timeScore)
        {
            time = timeScore;
        }

        //Pre: None.
        //Post: Returns the number of moves made by the player.
        //Description: Provides access to the player's move count.
        public int GetMoves()
        {
            return moves;
        }

        //Pre: moveCount is an integer representing the new move count.
        //Post: Updates the player's move count.
        //Description: Sets the player's move count to the provided value.
        public void SetMoves(int moveCount)
        {
            moves = moveCount;
        }

    }
}
