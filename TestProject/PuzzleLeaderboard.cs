//Author: Mark Rozin
//File Name: PuzzleLeaderboard.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Handles all the leaderboard logic of saving, loading, and displaying the leaderboard

using GameUtility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TestProject
{
    public class PuzzleLeaderboard
    {
        // File path for saving and loading leaderboard data
        private string filePath;

        //Object to prevent access to the leaderboard file
        private readonly object fileLock = new object();

        // Lists to store the top times and moves for the leaderboard
        private List<LeaderboardEntry> topTimes; // Holds top times sorted in ascending order
        private List<LeaderboardEntry> topMoves; // Holds top moves sorted in ascending order

        // Constructor to initialize the leaderboard with a file path and empty lists
        public PuzzleLeaderboard(string filePath)
        {
            this.filePath = filePath;
            topTimes = new List<LeaderboardEntry>();
            topMoves = new List<LeaderboardEntry>();
        }

        // Pre: The file path and leaderboard data are valid.
        // Post: Saves the top times and moves to the specified file in an ordered format.
        // Description: Writes the leaderboard entries to a file, ensuring thread safety to prevent concurrent access issues.
        public void SaveLeaderboard()
        {
            // Locking access to the file during the write operation to avoid crashes
            lock (fileLock) 
            {
                try
                {
                    using (StreamWriter writer = new StreamWriter(filePath, false)) // Overwrite the file with new leaderboard data.
                    {
                        // Save the top times section
                        writer.WriteLine("Best Times:");

                        for (int i = 0; i < topTimes.Count; i++) // Iterate through the top times list.
                        {
                            var entry = topTimes[i];
                            writer.WriteLine($"{i + 1}. {entry.GetPlayerName()}, {entry.GetTime():F2}s"); // Write each entry with numbering.
                        }

                        // Separator between times and moves.
                        writer.WriteLine("-------------------------"); 

                        // Write best moves
                        writer.WriteLine("Best Moves:");
                        for (int i = 0; i < topMoves.Count; i++) // Iterate through the top moves list.
                        {
                            var entry = topMoves[i];
                            writer.WriteLine($"{i + 1}. {entry.GetPlayerName()}, {entry.GetMoves()} moves"); // Write each entry with numbering.
                        }
                    }
                }

                // Handle any IO exceptions that occur during the write operation.
                catch (IOException ex) 
                {
                    Console.WriteLine($"IOException in SaveLeaderboard: {ex.Message}");
                }
            }
        }


        // Pre: data is a valid string formatted as leaderboard entries, such as "1. (PlayerName, 15.5s)" or "2. (PlayerName, 10 moves)".
        // Post: Adds the parsed data to either the top times or moves list, depending on the isTime flag.
        // Description: Parses a leaderboard entry string, removes unnecessary formatting, and adds the player's score to the appropriate list.
        private void ParseLeaderboardEntries(string data, bool isTime)
        {
            // Remove numbering prefix (e.g., "1. ") if it exists in the data string.
            if (data.Contains(". "))
            {
                data = data.Substring(data.IndexOf(". ") + 2).Trim(); // Trim the prefix and whitespace.
            }

            // Split the data string into parts using ',' as the separator. 
            // Expected format: (PlayerName, 15.5s) or (PlayerName, 10 moves).
            var parts = data.Trim('(', ')').Split(',');

            // Ensure the data has exactly two parts: PlayerName and Score/Move.
            if (parts.Length == 2)
            {
                // Extract the player name, trimming any extra whitespace.
                string playerName = parts[0].Trim();

                if (isTime) // If the entry is for the "Best Times" section.
                {
                    // Attempt to parse the score (time in seconds) and add it to the top times list.
                    if (double.TryParse(parts[1].Replace("s", "").Trim(), out double time))
                    {
                        AddTimeEntry(playerName, Math.Round(time, 2)); // Add the time entry rounded to 2 decimal places.
                    }
                }
                else // If the entry is for the "Best Moves" section.
                {
                    // Attempt to parse the score (number of moves) and add it to the top moves list.
                    if (int.TryParse(parts[1].Replace("moves", "").Trim(), out int moves))
                    {
                        AddMovesEntry(playerName, moves); // Add the move entry.
                    }
                }
            }
        }

        // Pre: None
        // Post: Loads the leaderboard data from the specified file into memory, populating the top times and moves lists.
        // Description: Reads the file line by line, identifies leaderboard sections (times or moves),
        //              and parses valid entries to add them to the respective lists.
        public void LoadLeaderboard()
        {
            // If the file does not exist, exit early as there is nothing to load.
            if (!File.Exists(filePath)) return;

            // Clear the current leaderboard data to avoid duplicates or stale data.
            topTimes.Clear();
            topMoves.Clear();

            try
            {
                // Open the file for reading using a StreamReader.
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string line;      // Variable to hold the current line being read.
                    bool isTime = true; // Tracks whether the current section is for times or moves.

                    // Read the file line by line until the end.
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("Best Times:"))
                        {
                            // Switch to processing "Best Times" entries.
                            isTime = true;
                        }
                        else if (line.StartsWith("Best Moves:"))
                        {
                            // Switch to processing "Best Moves" entries.
                            isTime = false;
                        }
                        else if (!string.IsNullOrWhiteSpace(line)) // Ignore empty or whitespace-only lines.
                        {
                            // Parse the entry based on whether it belongs to times or moves.
                            ParseLeaderboardEntries(line, isTime);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                // Log any file I/O exceptions that occur during the reading process.
                Console.WriteLine($"IOException in LoadLeaderboard: {ex.Message}");
            }
        }

        // Pre: filePath exists, spriteBatch and font are valid, startPosition is within the screen bounds
        // Post: Displays the leaderboard entries on the screen, with customized colors for different puzzles
        // Description: Reads the leaderboard file and renders its contents, applying unique color schemes based on the puzzle type
        public void DisplayLeaderboard(string filePath, SpriteBatch spriteBatch, SpriteFont font, Vector2 startPosition)
        {
            // If the file does not exist, display a warning message and exit
            if (!File.Exists(filePath))
            {
                spriteBatch.DrawString(font, "NO DATA", startPosition, Color.Red);
                return;
            }

            // Define color schemes based on the puzzle associated with the file
            Color headerColor, entryColor, dividerColor;

            //Change the colors based on what column they're in (what puzzle it is)
            switch (filePath)
            {
                case "hanoi_leaderboard.txt":
                    headerColor = Color.DarkRed; // Header color for Tower of Hanoi
                    entryColor = Color.Black;    // Entry color for player scores
                    dividerColor = Color.DarkRed; // Divider color for section breaks
                    break;

                case "lights_leaderboard.txt":
                    headerColor = Color.Goldenrod; // Header color for Lights Out
                    entryColor = Color.Black;
                    dividerColor = Color.Goldenrod;
                    break;

                case "sliding_leaderboard.txt":
                    headerColor = Color.LightCoral; // Header color for Sliding Puzzle
                    entryColor = Color.Black;
                    dividerColor = Color.LightCoral;
                    break;

                    //Default colors just in case
                default:
                    headerColor = Color.White;  
                    entryColor = Color.Gray;
                    dividerColor = Color.Red;
                    break;
            }

            // Open the file for reading and display its content line by line
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line; // Holds the current line being read from the file

                // Process each line in the file until the end
                while ((line = reader.ReadLine()) != null)
                {
                    // Skip empty or whitespace-only lines
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Color textColor;

                        // Determine the color based on the line content
                        if (line.StartsWith("Best Times:") || line.StartsWith("Best Moves:"))
                        {
                            textColor = headerColor; // Use the header color for section titles
                        }
                        else if (line.StartsWith("-------------------------"))
                        {
                            textColor = dividerColor; // Use the divider color for separators
                        }
                        else
                        {
                            textColor = entryColor; // Use the entry color for player scores
                        }

                        // Draw the line of text with the determined color
                        spriteBatch.DrawString(font, line, startPosition, textColor);

                        // Move the position down for the next line of text
                        startPosition.Y += 20;
                    }
                }
            }
        }

        // Pre: playerName is valid, time is a positive value
        // Post: Adds the player to the top times list, keeping only the top 3
        // Description: Adds a new time entry and ensures the list is sorted and capped at 3 entries
        public void AddTimeEntry(string playerName, double time)
        {
            topTimes.Add(new LeaderboardEntry(playerName, time, 0)); // Add new entry
            topTimes = topTimes.OrderBy(e => e.GetTime()).Take(3).ToList(); // Sort and cap at 3 entries
        }

        // Pre: playerName is valid, moves is a positive value
        // Post: Adds the player to the top moves list, keeping only the top 3
        // Description: Adds a new moves entry and ensures the list is sorted and capped at 3 entries
        public void AddMovesEntry(string playerName, int moves)
        {
            if (moves <= 0) return; // Ignore invalid move counts

            topMoves.Add(new LeaderboardEntry(playerName, 0, moves)); // Add new entry
            topMoves = topMoves.OrderBy(e => e.GetMoves()).Take(3).ToList(); // Sort and cap at 3 entries
        }

        // Pre: time is a positive value
        // Post: Returns true if the time qualifies as a top 3 score
        // Description: Checks if a given time is among the top scores
        public bool IsTopTime(double time)
        {
            // Return true if there are fewer than 3 scores or the provided time is better than the highest recorded time
            return topTimes.Count < 3 || time < topTimes.Max(e => e.GetTime());
        }

        // Pre: moves is a positive value
        // Post: Returns true if the moves count qualifies as a top 3 score
        // Description: Checks if a given move count is among the top scores
        public bool IsTopMove(int moves)
        {
            // Return true if there are fewer than 3 scores or the provided move count is better than the highest recorded moves
            return topMoves.Count < 3 || moves < topMoves.Max(e => e.GetMoves());
        }
    }
}