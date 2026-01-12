//Author: Mark Rozin
//File Name: Game1.cs 
//Project Name: Puzzle Escape Room
//Creation Date: Nov. 19, 2024
//Modified Date: Jan. 16, 2025 
//Description: Escape room game where you solve challenging puzzles to escape, or dive into endless puzzle modes for unlimited play

//2D Arrays & Lists - Utilized 2D arrays for sliding puzzle, lights out, lock system. Utilized lists for pickupable items system, mirrors, various text positions
//Recursion - Utilized Recursion in the Lights Out! Puzzle to track what puzzles are toggled in each click
//Stacks - Utilized stack in tower of hanoi to push and pop rings off the poles
//File I/O - Utilized File I/O in the puzzle leadboard system, where the scores are saved and read off file 
//Sorting - Used a little sorting for the puzzle leaderboard system to order the top scores and had two variables kept track of: # of moves and time

using GameUtility;
using Microsoft.Win32.SafeHandles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Policy;

namespace TestProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Define the sliding gridsize constant
        const int SLIDING_GRIDSIZE = 3;

        //Define gamestate constants
        const int MENU = 0;
        const int PUZZLE_MENU = 1;
        const int ROOM1 = 2;
        const int LEADERBOARD = 3;
        const int HANOI = 4;
        const int LIGHTS = 5;
        const int SLIDING = 6;
        const int CLOCK = 7;
        const int PROMPTING = 8;
        const int INSTRUCTIONS = 9;
        const int VICTORY = 10;

        //Define moving constants
        const int STOPPED = 0;
        const int POSITIVE = 1;
        const int NEGATIVE = -1;

        //Create objects for each of the game's puzzles
        LightsOut lightsOutPuzzle;
        Sliding slidingPuzzle;
        TowerOfHanoi hanoiPuzzle;

        //Variable to track whether the button has been pressed of not
        bool mousePressed = false;

        //Create all the timers in my game
        Timer hanoiTimer;
        Timer lightsTimer;
        Timer slidingTimer;
        Timer lightsCooldownTimer;
        Timer afterPuzzleTimer;
        Timer instructionsPreviewCooldownTimer;

        //Create all the images used in my main class
        Texture2D brownStickFloorImg;
        Texture2D appleImg;
        Texture2D poleImg;
        Texture2D ringImg;
        Texture2D playerImg;
        Texture2D safeImg;
        Texture2D whiteRoomBg;
        Texture2D blankSquareImg;
        Texture2D lightsImg;
        Texture2D tableImg;
        Texture2D pieImg;
        Texture2D ePanelImg;
        Texture2D rectangleBorderImg;
        Texture2D scrollCloseUpImg;
        Texture2D scrollGroundImg;
        Texture2D puzzleBoxImg;
        Texture2D buttonImg;
        Texture2D clockImg;
        Texture2D clockWallImg;
        Texture2D clockHandImg;
        Texture2D inClockHandImg;
        Texture2D bullsImg;
        Texture2D eyeImg;
        Texture2D monkeyImg;
        Texture2D carImg;
        Texture2D qrCodeImg;
        Texture2D whiteSquareImg;
        Texture2D doorImg;
        Texture2D mirrorImg;
        Texture2D[] slidingImages = new Texture2D[5];
        Texture2D lyingScrollImg;
        Texture2D closeUpScrollImg;
        Texture2D symbolsImg;
        Texture2D hanoiImg;
        Texture2D mrLaneHeadImg;
        Texture2D dustPolisherImg;

        //Create the sound effects in the game
        SoundEffect buttonSnd;
        SoundEffect buzzerSnd;
        SoundEffect fixSnd;
        SoundEffect pickUpSnd;
        SoundEffect dingSnd;
        SoundEffect applauseSnd;
        SoundEffect winSnd;
        SoundEffect laserSnd;
        SoundEffect dustSnd;

        //Booleans to track when the ding and laser sound played (to only let them play once)
        bool[] dingSoundPlayed = { false, false, false, false, false, false };
        bool laserSoundPlayed;

        //Create the songs in the game
        Song roomMusic;
        Song puzzleMusic;

        //Riddle for the clock puzzle and their locations
        string[] riddle = {"When the bells toll loud and clear", "Young minds gather far and near", "The sun climbs high, a brand-new day", "The moment to learn, work, and play"};
        Vector2[] riddleTextLocs = new Vector2[4];

        //Create the prompt type after a highscore (either a top time score, a top move score, or both)
        string promptType = "test";

        //Define the current image index for the rotation of images in the sliding puzzle (start it at the first image)
        int currentImageIndex = 0;

        //Check if they entered their prompt
        bool enterPressed;

        //Bool to track is laser has been solved
        bool laserSolved;
        
        //The images and the rectangles of the letter symbols
        Texture2D[] lettersImgs = new Texture2D[4];
        Rectangle[] lettersRecs = new Rectangle[4];

        //Create the X of the laser used for calculation, and the angle of incidence
        double laserX;
        double angleIncidence;

        //Create all the rectangles used in my main class
        Rectangle pieRec;
        Rectangle clockWallRec;
        Rectangle whiteRoomRec;
        Rectangle playerRec;
        Rectangle lockRec;
        Rectangle tableRec;
        Rectangle ePanelRec;
        Rectangle[] lockComboRecs = new Rectangle[3];
        Rectangle bullsRec;
        Rectangle eyeRec;
        Rectangle doorRec;
        Rectangle hanoiRec;
        Rectangle newLocation;

        //Create an object for each puzzle of its leaderboard
        PuzzleLeaderboard hanoiLeaderboard;
        PuzzleLeaderboard lightsLeaderboard;
        PuzzleLeaderboard slidingLeaderboard;

        //State to check if they got a highscore in the puzzles
        bool highscoreAcheived;

        //Vector2 to help with leaderboard display
        Vector2 leaderboardStartingPosition;

        //Variable to hold user's name on highscore board
        string playerName = "";
        string input = "";

        //Make the array for the locations of the instructions messages
        Vector2[] instructionsTextLocs = new Vector2[8];

        //Create the list of the items that can be picked up, as well as a variable to see which one is currently being held
        PickupableItem currentlyHeldItem;
        List<PickupableItem> pickupableItems;

        //Create an object for each item allowing it to have mechanics to be picked up
        PickupableItem missingHandItem;
        PickupableItem puzzleBoxItem;
        PickupableItem missingPoleItem;
        PickupableItem pieItem;
        PickupableItem dustPolisherItem;

        //Create the mirror objects list and a variable for what mirror is currently being held
        List<Mirror> mirrors; 
        Mirror heldMirror;

        //Create a variable to check if they are checking the lock
        bool pickingLock = false;

        //Create all the sprite fonts used in the main class
        SpriteFont TempFont;
        SpriteFont ButtonFont;
        SpriteFont NewFont;
        SpriteFont TitleFont;
        SpriteFont MediumFont;

        //Create all the text locations Vector2s (encompass in lists for simplicity)
        List<Vector2> textPos = new List<Vector2>();
        List<Vector2> titlePos = new List<Vector2>();

        //A list of Vector2s of the positions of the numbers in the number lock, and an integer to represent what slot of the number lock the user is on
        int selectedLockSlot = 0;
        Vector2[] comboLockLoc = new Vector2[3];

        //A 2D array of all of the numbers in the lock, an array to track what number is chosen, and a bool to track if its been opened
        int[,] lockComboNums = new int[3, 10];
        int[] selectedLockNum = new int[3];
        bool lockSolved = false;

        //Create the clock object to handle the player choosing a time
        Clock clock;

        //A variable to track whether they are in campaign or in puzzle mode, and one to track if they have played the campaign
        bool inCampaign = false;
        bool campaignPlayed = false;

        //Variable for passcode input, the correct passcode, and a bool to see if they are entering
        string passcodeInput = "";
        string correctPasscode = "BOLT";
        bool isEnteringPasscode = false; 

        //A list of the button rectangles
        List<Rectangle> buttonRecs = new List<Rectangle>();
        
        //A list of the title texts and the button texts
        List<string> titleStrings = new List<string>();
        List<String> buttonStrings = new List<String>();

        //Original sliding number arrangment 
        int[,] slidingNums = new int[SLIDING_GRIDSIZE,SLIDING_GRIDSIZE];
            
        //Holds the image of the currently held item
        Texture2D currentlyHeldItemImg; 

        //The bottom left currently held box to show what item is being held
        Rectangle currentlyHeldBox;

        //Strings to display the timers in the puzzles
        string displayHanoiTimer;
        string displayLightsTimer;
        string displaySlidingTimer;

        //The pie message after solving sliding puzzle in campaign, and a press SPACE to continue message
        string pieMessage = "Oooooh what a nice circular pi";
        string pressSpaceMessage = "Press Escape To Continue";

        //Defining the intitial X and Y directions of the player
        int dirXPlayer = POSITIVE;
        int dirYPlayer = POSITIVE;

        //Defining the maximum speed of my player, the Vector2 monitoring my player's speed, and my player's position
        float maxSpeedPlayer = 130f;
        Vector2 speedPlayer = new Vector2(POSITIVE, POSITIVE);
        Vector2 playerPos;

        //Create the gamelines for the lasers
        GameLine newLaser;
        GameLine reflectedLaser;
        GameLine reflectedLaser2;

        //Name of the current puzzle being played
        string currentPuzzleName;

        //Create scroll object
        Scroll scroll;

        //Track scroll inspection mode
        bool isInspectingScroll = false;

        //Create current mouse state, current keyboard state, previous keyboard state, and mouse position variables
        MouseState mouse;
        Vector2 mousePosition;
        KeyboardState kb;
        KeyboardState prevKb;

        //Create variables for screen width and height
        int screenWidth;
        int screenHeight;

        //Track if the player has chosen to move from puzzle
        bool moveFromPuzzle = false;

        //Create a variable for game state and set it to menu
        int gameState = MENU;

        //Array to hold the colors of the rings
        Color[] ringColors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Purple };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            //Let the user see the mouse
            IsMouseVisible = true;

            //Set the screen width and height variables
            screenWidth = graphics.PreferredBackBufferWidth;
            screenHeight = graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            //Load in all of my Texture2Ds
            poleImg = Content.Load<Texture2D>("Images/Sprites/brownImg");
            ringImg = Content.Load<Texture2D>("Images/Sprites/ringOfHanoi");
            whiteRoomBg = Content.Load<Texture2D>("Images/Backgrounds/whiteRoomBg2");
            playerImg = Content.Load<Texture2D>("Images/Sprites/playerImg");
            safeImg = Content.Load<Texture2D>("Images/Sprites/safeImg");
            blankSquareImg = Content.Load<Texture2D>("Images/Sprites/blankSquare");
            lightsImg = Content.Load<Texture2D>("Images/Sprites/lightsImg");
            tableImg = Content.Load<Texture2D>("Images/Sprites/tableImg");
            pieImg = Content.Load<Texture2D>("Images/Sprites/pieImg1");
            ePanelImg = Content.Load<Texture2D>("Images/Sprites/electricPanelImg");
            rectangleBorderImg = Content.Load<Texture2D>("Images/Sprites/RectangleBorder");
            scrollGroundImg = Content.Load<Texture2D>("Images/Sprites/scrollImg");
            scrollCloseUpImg = Content.Load<Texture2D>("Images/Sprites/goodCloseUpScroll");
            puzzleBoxImg = Content.Load<Texture2D>("Images/Sprites/puzzleBoxImg");
            lettersImgs[0] = Content.Load<Texture2D>("Images/Sprites/letterB");
            lettersImgs[1] = Content.Load<Texture2D>("Images/Sprites/letterO");
            lettersImgs[2] = Content.Load<Texture2D>("Images/Sprites/letterL");
            lettersImgs[3] = Content.Load<Texture2D>("Images/Sprites/letterT");
            appleImg = Content.Load<Texture2D>("Images/Sprites/appleImg");
            buttonImg = Content.Load<Texture2D>("Images/Sprites/blackButton");
            clockImg = Content.Load<Texture2D>("Images/Sprites/clockImg");
            clockWallImg = Content.Load<Texture2D>("Images/Sprites/clockWallImg2");
            clockHandImg = Content.Load<Texture2D>("Images/Sprites/clockHand2Img");
            inClockHandImg = Content.Load<Texture2D>("Images/Sprites/blackRecImage");
            bullsImg = Content.Load<Texture2D>("Images/Sprites/bullImg");
            eyeImg = Content.Load<Texture2D>("Images/Sprites/whiteCircleImg");
            brownStickFloorImg = Content.Load<Texture2D>("Images/Sprites/brownStickFloorImg");
            monkeyImg = Content.Load<Texture2D>("Images/Sprites/monkeyImg");
            carImg = Content.Load<Texture2D>("Images/Sprites/carImg"); 
            qrCodeImg = Content.Load<Texture2D>("Images/Sprites/qrCodeImg");
            doorImg = Content.Load<Texture2D>("Images/Sprites/doorImg");
            lyingScrollImg = Content.Load<Texture2D>("Images/Sprites/scroll3Img");
            closeUpScrollImg = Content.Load<Texture2D>("Images/Sprites/goodCloseUpScroll");
            symbolsImg = Content.Load<Texture2D>("Images/Sprites/symbolsImg");
            hanoiImg = Content.Load<Texture2D>("Images/Sprites/hanoiImg2");
            mrLaneHeadImg = Content.Load<Texture2D>("Images/Sprites/mrLaneHead");
            dustPolisherImg = Content.Load<Texture2D>("Images/Sprites/dustPolisherImg");

            //Create the door, clock, currentlyHeldBox, bulls/eye, ePanel, safe, lock, player, room, and lock combo rectangles 
            doorRec = new Rectangle(700, 240, (int)(doorImg.Width * 0.30), (int)(doorImg.Height * 0.25));
            bullsRec = new Rectangle(660, 350, (int)(bullsImg.Width * 0.2), (int)(bullsImg.Height * 0.2));
            eyeRec = new Rectangle(680, 390, (int)(eyeImg.Width * 0.03), (int)(eyeImg.Height * 0.03));
            clockWallRec = new Rectangle(720, 80, (int)(clockWallImg.Width * 0.15), (int)(clockWallImg.Height * 0.15));
            currentlyHeldBox = new Rectangle(20, 40, 100, 100);
            ePanelRec = new Rectangle(600, 15, (int)(blankSquareImg.Width * 0.15), (int)(blankSquareImg.Height * 0.15));
            lockComboRecs[0] = new Rectangle(152, 150, (int)(blankSquareImg.Width * 0.5), (int)(blankSquareImg.Height * 0.5));
            lockComboRecs[1] = new Rectangle(150 + ((int)(blankSquareImg.Width * 0.5)), 150, (int)(blankSquareImg.Width * 0.5), (int)(blankSquareImg.Height * 0.5));
            lockComboRecs[2] = new Rectangle(148 + ((int)(blankSquareImg.Width * 1)), 150, (int)(blankSquareImg.Width * 0.5), (int)(blankSquareImg.Height * 0.5));
            whiteRoomRec = new Rectangle(0, 0, screenWidth, screenHeight);
            playerRec = new Rectangle(300, 150, (int)(playerImg.Width * 0.05), (int)(playerImg.Height * 0.05));
            lockRec = new Rectangle(320, 400, (int)(safeImg.Width * 0.035), (int)(safeImg.Height * 0.035));
            hanoiRec = new Rectangle(400, 200, (int)(hanoiImg.Width * 0.12), (int)(hanoiImg.Height * 0.12));

            //Initialize the pickupable items as a new list
            pickupableItems = new List<PickupableItem>();

            //Initialize each item individually and add them to the list
            missingHandItem = new PickupableItem(clockHandImg, new Vector2(300, 400), (int)(clockHandImg.Width * 0.1), (int)(clockHandImg.Height * 0.1), "?");
            puzzleBoxItem = new PickupableItem(puzzleBoxImg, new Vector2(200, 200), (int)(puzzleBoxImg.Width * 0.1), (int)(puzzleBoxImg.Height * 0.1), "Puzzle Box");
            missingPoleItem = new PickupableItem(brownStickFloorImg, new Vector2(620, 100), (int)(brownStickFloorImg.Width * 0.05), (int)(brownStickFloorImg.Height * 0.05), "Brown Pole");
            pieItem = new PickupableItem(pieImg, new Vector2(200,200), (int)(pieImg.Width * 0.05), (int)(pieImg.Height * 0.05), "Pi");
            dustPolisherItem = new PickupableItem(dustPolisherImg, new Vector2(600, 350), (int)(dustPolisherImg.Width * 0.15), (int)(dustPolisherImg.Height * 0.15), "Brush");
            pickupableItems.Add(puzzleBoxItem);
            pickupableItems.Add(missingHandItem);
            pickupableItems.Add(missingPoleItem);
            pickupableItems.Add(pieItem);
            pickupableItems.Add(dustPolisherItem);

            //Loads in all the sound effects and songs
            buttonSnd = Content.Load<SoundEffect>("Audio/button-202966");
            buzzerSnd = Content.Load<SoundEffect>("Audio/wrong-answer-sound-effect");
            fixSnd = Content.Load<SoundEffect>("Audio/steampunk-gadget-lock-and-unlock-188053");
            pickUpSnd = Content.Load<SoundEffect>("Audio/stiff-wooden-tap-42877");
            dingSnd = Content.Load<SoundEffect>("Audio/ding-126626");
            applauseSnd = Content.Load<SoundEffect>("Audio/applause-cheer-236786");
            winSnd = Content.Load<SoundEffect>("Audio/success-fanfare-trumpets-6185");
            laserSnd = Content.Load<SoundEffect>("Audio/electronic-buzzing-sound-29464");
            dustSnd = Content.Load<SoundEffect>("Audio/crumble-2-82156");
            roomMusic = Content.Load<Song>("Audio/suspense-pulse-tense-music-266060");
            puzzleMusic = Content.Load<Song>("Audio/good-night-lofi-cozy-chill-music-160166");
            
            
            //Changes the volume of the sound effects and songs, and makes the song repeat
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            SoundEffect.MasterVolume = 0.7f;
            
            //Load in my fonts
            TempFont = Content.Load<SpriteFont>("Fonts/TempFont");
            ButtonFont = Content.Load<SpriteFont>("Fonts/ButtonFont");
            NewFont = Content.Load<SpriteFont>("Fonts/NewFont");
            TitleFont = Content.Load<SpriteFont>("Fonts/TitleFont");
            MediumFont = Content.Load<SpriteFont>("Fonts/MediumFont");

            //Initialize the clock object
            clock = new Clock(new Vector2(400, 220), 150, clockImg, inClockHandImg, false);

            //Initialize the timers, setting the puzzle ones as infinite stopwatches 
            hanoiTimer = new Timer(Timer.INFINITE_TIMER, false);
            lightsTimer = new Timer(Timer.INFINITE_TIMER, false);
            slidingTimer = new Timer(Timer.INFINITE_TIMER, false);
            afterPuzzleTimer = new Timer(1500, false);
            instructionsPreviewCooldownTimer = new Timer(5000, false);

            //Create a 1x1 white square pixel for general use
            whiteSquareImg = new Texture2D(GraphicsDevice, 1, 1);
            whiteSquareImg.SetData(new[] { Color.White }); 

            //Create the 50x10 blue mirror image
            mirrorImg = new Texture2D(GraphicsDevice, 50, 10);
            mirrorImg.SetData(Enumerable.Repeat(Color.Blue, 50 * 10).ToArray());

            //Fill the sliding images array with images to cycle through
            slidingImages[0] = appleImg;
            slidingImages[1] = carImg;
            slidingImages[2] = monkeyImg;
            slidingImages[3] = qrCodeImg;
            slidingImages[4] = mrLaneHeadImg;

            //Add button rectangles and corresponding text to the lists
            buttonRecs.Add(new Rectangle(180, 200, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("CAMPAIGN");
            buttonRecs.Add(new Rectangle(420, 200, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("GAMES");
            buttonRecs.Add(new Rectangle(50, 280, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Tower of Hanoi");
            buttonRecs.Add(new Rectangle(300, 280, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Lights Out!");
            buttonRecs.Add(new Rectangle(550, 280, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Sliding Puzzle");
            buttonRecs.Add(new Rectangle(50, 380, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Back To Menu");
            buttonRecs.Add(new Rectangle(570, GetCenteredY((int)(buttonImg.Height * 0.15)),(int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Back To Menu");
            buttonRecs.Add(new Rectangle(80, 380, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Back To Menu");
            buttonRecs.Add(new Rectangle(GetCenteredX((int)(buttonImg.Width * 0.2)), 360, (int)(buttonImg.Width * 0.2), (int)(buttonImg.Height * 0.2)));
            buttonStrings.Add("Back To Main Menu");
            buttonRecs.Add(new Rectangle(GetCenteredX((int)(buttonImg.Width * 0.15)), 5, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Leaderboard");
            buttonRecs.Add(new Rectangle(600, 380, (int)(buttonImg.Width * 0.15), (int)(buttonImg.Height * 0.15)));
            buttonStrings.Add("Back to Menu");

            //Add the text locations based on the length of the strings, the size of the font, the rectangles (use a method to center the text)
            for (int i = 0; i < 8; i++)
            {
                textPos.Add(CenterTextOnButton(buttonStrings[i], ButtonFont, buttonRecs[i]));
            }

            //Initialize the locations of the instructions text with a foor loop
            for (int d = 0; d < 8; d++)
            {
                instructionsTextLocs[d] = new Vector2(20, 40 + (50 * d));
            }

            //Add the text positions for the timers in the puzzles
            textPos.Add(new Vector2(600, 380));
            textPos.Add(new Vector2(600, GetCenteredY((int)(buttonImg.Height * 0.15)) + 90));
            textPos.Add(new Vector2(400, 380));

            //Add the text positions for the move counts in the puzzles
            textPos.Add(new Vector2(590, GetCenteredY((int)(buttonImg.Height * 0.15)) + 120));
            textPos.Add(new Vector2(570, GetCenteredY((int)(buttonImg.Height * 0.15)) + 50));
            textPos.Add(new Vector2(400, 400));

            //Add the text positions for the pie message, press space message, and other messages
            textPos.Add(CenterTextOnButton(buttonStrings[8], ButtonFont, buttonRecs[8]));
            textPos.Add(new Vector2(200, 100));
            textPos.Add(new Vector2(200,120));
            textPos.Add(CenterTextOnButton(buttonStrings[9], ButtonFont, buttonRecs[9]));
            textPos.Add(CenterTextOnButton(buttonStrings[10], ButtonFont, buttonRecs[10]));
            textPos.Add(new Vector2(250, 40));

            //Create the title strings and their locations
            titleStrings.Add("PUZZLE ESCAPE ROOM");
            titlePos.Add(CenterTextHorizontally(TitleFont, titleStrings[0], screenWidth, 100));
            titleStrings.Add("PUZZLE MENU");
            titlePos.Add(CenterTextHorizontally(TitleFont, titleStrings[1], screenWidth, 155));
            titlePos.Add(CenterTextHorizontally(MediumFont, buttonStrings[2], screenWidth, 70));
            titlePos.Add(CenterTextHorizontally(MediumFont, buttonStrings[3], screenWidth, 20));
            titlePos.Add(CenterTextHorizontally(MediumFont, buttonStrings[4], screenWidth, 70));

            //Create the angle of incidence for the laser beams using basic right triangle trigonometry (tan = opposite over adjacent) and convert to degrees from radians
            angleIncidence = Math.Atan(4 / 3) * (180/Math.PI);

            //Create the first laser beam (original path of the laser)
            newLaser = new GameLine(GraphicsDevice, new Vector2(0, 200), new Vector2(150, 480));

            //Calculate the X coordinate of the laser rflection on the next surface again using trigonometry
            laserX = (150 + (screenHeight / (float)Math.Tan(angleIncidence)));

            //Create the first reflected laser beam, starting on the end point of the last one, and ending on the calculation intersection point 
            reflectedLaser = new GameLine(GraphicsDevice, new Vector2(150,screenHeight), new Vector2((int)laserX, 0));

            //Create the second reflected laser beam, starting on the end point of the last one, and ending on the calculation intersection point 
            reflectedLaser2 = new GameLine(GraphicsDevice, new Vector2((int)laserX, 0), new Vector2((float)laserX + (screenHeight / (float)(Math.Tan(angleIncidence))), screenHeight));
            
            //Initialize the list of the mirrors
            mirrors = new List<Mirror>
            {
                new Mirror(mirrorImg, new Vector2(300, -5), screenWidth, screenHeight),
                new Mirror(mirrorImg, new Vector2(400, screenHeight - 5), screenWidth, screenHeight)
            };

            //Initilize the rectangles of the letters
            for (int i = 0; i < 4; i++)
            {
                lettersRecs[i] = new Rectangle(150 + (lettersImgs[0].Width * i * 2), 10, (int)(lettersImgs[0].Width * 1.5), (int)(lettersImgs[0].Height * 1.5));
            }

            //Initialize the scroll object
            scroll = new Scroll(lyingScrollImg, scrollCloseUpImg, symbolsImg, new Rectangle(15, 140, (int)(scrollGroundImg.Width * 0.25), (int)(scrollGroundImg.Height * 0.25)));

            //Initilize the puzzle leaderboards, and load in the data from text files
            hanoiLeaderboard = new PuzzleLeaderboard("hanoi_leaderboard.txt");
            lightsLeaderboard = new PuzzleLeaderboard("lights_leaderboard.txt");
            slidingLeaderboard = new PuzzleLeaderboard("sliding_leaderboard.txt");
            hanoiLeaderboard.LoadLeaderboard();
            lightsLeaderboard.LoadLeaderboard();
            slidingLeaderboard.LoadLeaderboard();

            //Create the sliding nums calling a method to generate the numbers randomly (but still let the puzzle be solvable)
            slidingNums = GenerateRandomSlidingNums(SLIDING_GRIDSIZE);

            //Set all the lock combos to 0 and creating their vectors using a nested for loop (to go through the 2D array)
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    //Set all the numbers to 0 and make the numbers' positions
                    lockComboNums[i, j] = 0;
                    comboLockLoc[i] = new Vector2((148 + (int)(blankSquareImg.Width * 0.25)) + (2 * i * (int)(blankSquareImg.Width * 0.25)), 210);
                    selectedLockNum[i] = 0;
                }
            }

            //Initilize the locations for the locations of the riddle
            for (int i = 0; i < 4; i ++)
            {
                riddleTextLocs[i] = new Vector2(20, 400 + (i * 15));
            }

            //Set the player position equal to its rectangle location
            playerPos = new Vector2(playerRec.X, playerRec.Y);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            //Get the mouse state, keyboard state, previous keyboard state, and mouse position
            mouse = Mouse.GetState();
            prevKb = kb;
            kb = Keyboard.GetState();
            mousePosition = new Vector2(mouse.X, mouse.Y);

            //Update the timers
            hanoiTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            lightsTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            slidingTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            afterPuzzleTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);
            instructionsPreviewCooldownTimer.Update(gameTime.ElapsedGameTime.TotalMilliseconds);

            //Set the button pressed variable to true if they've pressed it and false if they've released it
            if (mouse.LeftButton == ButtonState.Pressed && !mousePressed)
            {
                //Button is being pressed
                mousePressed = true;
            }
            else if (mouse.LeftButton == ButtonState.Released)
            {
                //Button is not being pressed
                mousePressed = false;
            }
            
            //Overarching switch statement to monitor what gamestate we are currently in
            switch (gameState)
            {
                // Handle the updating when in the menu state
                case MENU:

                    //If they press campaign button
                    if (mousePressed && buttonRecs[0].Contains(mouse.Position) && campaignPlayed == false)
                    {
                        //Initialize all the puzzles, set the inCampaign tracking variable to true
                        inCampaign = true;
                        lightsOutPuzzle = new LightsOut(lightsImg, 5, 200, 70, 70, inCampaign);
                        slidingPuzzle = new Sliding(appleImg, blankSquareImg, GenerateRandomSlidingNums(SLIDING_GRIDSIZE), 290, 130, 70, inCampaign);
                        hanoiPuzzle = new TowerOfHanoi(6, poleImg, ringImg, ringColors, inCampaign, false);

                        //Play sound effect and start background music
                        buttonSnd.CreateInstance().Play();
                        MediaPlayer.Play(roomMusic);

                        //Take them to instructions page
                        gameState = INSTRUCTIONS;
                    }

                    //IF they press the puzzle button
                    if (mousePressed && buttonRecs[1].Contains(mouse.Position))
                    {
                        //Set the inCampaign tracking variable to false, and send them to the puzzle menu and play button sound
                        inCampaign = false;

                        //Play sound effect and start background music
                        buttonSnd.CreateInstance().Play();
                        MediaPlayer.Play(puzzleMusic);

                        //Take them to puzzle menu
                        gameState = PUZZLE_MENU;
                    }

                break;

                // Handle the updating when in the puzzle menu state
                case PUZZLE_MENU:

                    //Reset the timer that begins after the player gets a highscore and state that they havent acheived one
                    afterPuzzleTimer.ResetTimer(false);
                    highscoreAcheived = false;

                    //If they choose tower of hanoi
                    if (mousePressed && buttonRecs[2].Contains(mouse.Position))
                    {
                        //Initialize the puzzle, start the timer, and take them to the HANOI gamestate and play button sound
                        hanoiPuzzle = new TowerOfHanoi(6, poleImg, ringImg, ringColors, inCampaign, true);
                        hanoiTimer.ResetTimer(true);
                        displayHanoiTimer = hanoiTimer.GetTimePassedAsString(2);
                        buttonSnd.CreateInstance().Play();
                        gameState = HANOI;
                    }

                    //If they choose lights out puzzle
                    if (mousePressed && buttonRecs[3].Contains(mouse.Position))
                    {
                        //Initialize the puzzle, start the timer, and take them to the lights out gamestate and play button sound
                        lightsOutPuzzle = new LightsOut(lightsImg, 5, 200, 70, 70, inCampaign);
                        lightsTimer.ResetTimer(true);
                        displayLightsTimer = lightsTimer.GetTimePassedAsString(2);
                        buttonSnd.CreateInstance().Play();
                        gameState = LIGHTS;
                    }

                    //If they choose sliding puzzle
                    if (mousePressed && buttonRecs[4].Contains(mouse.Position))
                    {
                        //Initialize the puzzle, start the timer, and take them to sliding puzzle gamestate and play button sound
                        slidingPuzzle = new Sliding(slidingImages[currentImageIndex], blankSquareImg, GenerateRandomSlidingNums(3), 290, 130, 70, inCampaign);
                        currentImageIndex = (currentImageIndex + 1) % slidingImages.Length;
                        slidingTimer.ResetTimer(true);
                        displaySlidingTimer = slidingTimer.GetTimePassedAsString(2);
                        buttonSnd.CreateInstance().Play();
                        gameState = SLIDING;
                    }

                    //If they press the leaderboard
                    if (mousePressed && buttonRecs[9].Contains(mouse.Position))
                    {
                        //Take them to the leaderboard and play button sound
                        buttonSnd.CreateInstance().Play();
                        gameState = LEADERBOARD;
                    }

                    //If they press back to menu
                    if (mousePressed && buttonRecs[8].Contains(mouse.Position))
                    {
                        //Take them back to the menu and play button sound
                        buttonSnd.CreateInstance().Play();
                        MediaPlayer.Stop();
                        gameState = MENU;
                    }

                break;

                    //Leaderboard gamestate
                case LEADERBOARD:

                    //Load all of the leaderboards in
                    hanoiLeaderboard.LoadLeaderboard();
                    lightsLeaderboard.LoadLeaderboard();
                    slidingLeaderboard.LoadLeaderboard();

                    //If they press the menu button
                    if (mousePressed && buttonRecs[10].Contains(mouse.Position))
                    {
                        //Take them back to the menu and play button sound
                        buttonSnd.CreateInstance().Play();
                        gameState = PUZZLE_MENU;
                    }

                break;

                    //Gamestate to handle updates in instructions
                case INSTRUCTIONS:

                    //Activate the timer that lets them see the instructions
                    instructionsPreviewCooldownTimer.Activate();

                    //If the timer is finished, they can press the button, and if they do it goes to arena.
                    if (instructionsPreviewCooldownTimer.IsFinished() == true)
                    {
                        //If they press on the screen
                        if (mousePressed == true)
                        {
                            //Reset the timer 
                            instructionsPreviewCooldownTimer.ResetTimer(false);

                            //Play background music and take them to escape room
                            gameState = ROOM1;
                        }
                    }
                break;

                // Handle the updating when in the escape room state
                case ROOM1:

                    //If they press E on the door, it takes them to the passcode
                    if (kb.IsKeyDown(Keys.E) && !isEnteringPasscode)
                    {
                        //IF they're intersecting the door
                        if (playerRec.Intersects(doorRec))
                        {
                            //Takes them to the prompting case, resets the passcode input, and the boolean to check if they're entering the passcode
                            isEnteringPasscode = true;
                            passcodeInput = "";
                            buttonSnd.CreateInstance().Play();
                            gameState = PROMPTING;
                        }
                    }

                    //If all mirrors are aligned and the laser is drawn, laser puzzle is solved
                    if (mirrors[0].ContainsPoint(reflectedLaser.GetPt2()) && mirrors[1].ContainsPoint(newLaser.GetPt2()) && hanoiPuzzle.IsSolved())
                    {
                        laserSolved = true;
                    }

                    //If the player presses Q with the missing hand, they fix the clock and lose the hand
                    if (playerRec.Intersects(clockWallRec))
                    {
                        //If they press Q and have missing pole
                        if (kb.IsKeyDown(Keys.Q) && !prevKb.IsKeyDown(Keys.Q) && missingHandItem.GetItemState())
                        {
                            //Play the fix sound effect
                            fixSnd.CreateInstance().Play();

                            //Fixes the clock and loses the hand
                            clock.FixClock(); 
                            currentlyHeldItem = null;
                        }
                    }

                    //If the player presses Q on the hanoi with the missing pole, they fix it and lose the pole
                    if (playerRec.Intersects(hanoiRec))
                    {   
                        //If they press Q and have missing pole
                        if (kb.IsKeyDown(Keys.Q) && !prevKb.IsKeyDown(Keys.Q) && missingPoleItem.GetItemState())
                        {
                            //Play the fix sound effect
                            fixSnd.CreateInstance().Play();

                            //Fixes the clock and loses the pole
                            hanoiPuzzle.FixMiddlePole(); 
                            currentlyHeldItem = null;
                        }
                    }

                    
                    // Handle player movement and mirror interaction based on keyboard input
                    if (kb.IsKeyDown(Keys.W))
                    {
                        // Move player upward if not holding a mirror
                        if (heldMirror == null)
                        {
                            dirYPlayer = -1;
                        }
                    }

                    //If they press S
                    if (kb.IsKeyDown(Keys.S))
                    {
                        // Move player downward if not holding a mirror
                        if (heldMirror == null)
                        {
                            dirYPlayer = 1;
                        }
                    }

                    //If they press A move the player left
                    if (kb.IsKeyDown(Keys.A))
                    {
                        dirXPlayer = -1;

                        //If they're holding a mirror
                        if (heldMirror != null)
                        {
                            // Move the mirror left along with the player
                            newLocation = (heldMirror.GetBounds());
                            newLocation.X -= (int)(maxSpeedPlayer * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            heldMirror.SetPosition(new Vector2(playerRec.X + 40, newLocation.Y));
                            heldMirror.SetBounds(new Rectangle(playerRec.X + 40, (int)heldMirror.GetPosition().Y, heldMirror.GetBounds().Width, heldMirror.GetBounds().Height));
                        }
                    }

                    //If they press D move the player right
                    if (kb.IsKeyDown(Keys.D))
                    {
                        dirXPlayer = 1;

                        //If they're holding a mirror
                        if (heldMirror != null)
                        {
                            // Move the mirror right along with the player
                            newLocation = (heldMirror.GetBounds());
                            newLocation.X += (int)(maxSpeedPlayer * (float)gameTime.ElapsedGameTime.TotalSeconds);
                            heldMirror.SetPosition(new Vector2(playerRec.X + 40, newLocation.Y));
                            heldMirror.SetBounds(new Rectangle(playerRec.X + 40, (int)heldMirror.GetPosition().Y, heldMirror.GetBounds().Width, heldMirror.GetBounds().Height));
                        }
                    }

                    //If they aren't going up or down they're stopped in the Y axis
                    if (!kb.IsKeyDown(Keys.W) && (!(kb.IsKeyDown(Keys.S))))
                    {
                        dirYPlayer = 0;
                    }

                    //If they aren't going left or right they're stopped in the X axis
                    if (!kb.IsKeyDown(Keys.A) && (!(kb.IsKeyDown(Keys.D))))
                    {
                        dirXPlayer = 0;
                    }

                    // If the player presses E, check for interactions with nearby objects and update the game state accordingly
                    if ((kb.IsKeyDown(Keys.E) && prevKb.IsKeyUp(Keys.E)))
                    {
                        // If the player is near the electric panel, change the game state to LIGHTS
                        if (playerRec.Intersects(ePanelRec) && !lightsOutPuzzle.IsSolved())
                        {
                            buttonSnd.CreateInstance().Play();
                            gameState = LIGHTS;
                        }

                        // If the player is near the clock on the wall, change the game state to CLOCK
                        if (playerRec.Intersects(clockWallRec))
                        {
                            buttonSnd.CreateInstance().Play();
                            gameState = CLOCK;
                        }

                        //If the player is near the hanoi, change the gamestate to clock
                        if (playerRec.Intersects(hanoiRec))
                        {
                            buttonSnd.CreateInstance().Play();
                            gameState = HANOI;
                        }

                        //If they are near the lock, let them try to pick the lock
                        if (playerRec.Intersects(lockRec))
                        {
                           pickingLock = true;
                        }

                        //If they are not near the lock, they are not picking it
                        else
                        {
                            pickingLock = false;
                        }

                        // If the player is near the first pickupable item and the puzzle box is not already picked up, change the game state to SLIDING
                        if (playerRec.Intersects(pickupableItems[0].GetItemBounds()) && !puzzleBoxItem.GetItemState() && !slidingPuzzle.IsSolved())
                        {
                            buttonSnd.CreateInstance().Play();
                            gameState = SLIDING;
                        }

                        // Handle interactions with mirrors
                        if (heldMirror == null)
                        {
                            // If no mirror is currently held, check if the player is near any mirror
                            foreach (var mirror in mirrors)
                            {
                                //If the player is near a mirror and presses E, make that the held mirror 
                                if (playerRec.Intersects(mirror.GetBounds()))
                                {
                                    //Make this the held mirror (player can now move with it)
                                    heldMirror = mirror; 
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // If a mirror is already held, release it because they pressed E
                            heldMirror = null;
                        }
                    }

                    // Player is not holding anything, let them pick up their items
                    if (currentlyHeldItem == null) 
                    {
                        //If they press F
                        if ((kb.IsKeyDown(Keys.F) && prevKb.IsKeyUp(Keys.F)))
                        {
                            //If the lock has been solved, and clock hasn't been fixed, let them pick up the clock hand
                            if (playerRec.Intersects(missingHandItem.GetItemBounds()) && (lockSolved == true) && !clock.IsFixed())
                            {
                                //Set the currently held item to the hand and set its state to picked up
                                currentlyHeldItem = missingHandItem;
                                missingHandItem.SetItemState(true);

                                //Play pick up sound
                                pickUpSnd.CreateInstance().Play();
                            }

                            //If the sliding puzzle hasn't been solved, let them pick up the puzzle box
                            else if (playerRec.Intersects(puzzleBoxItem.GetItemBounds()) && !slidingPuzzle.IsSolved())
                            {
                                //Set the currently held item to the puzzle box and set its state to picked up
                                currentlyHeldItem = puzzleBoxItem;
                                puzzleBoxItem.SetItemState(true);

                                //Play pick up sound
                                pickUpSnd.CreateInstance().Play();
                            }

                            //If the clock has been solved and tower of hanoi hasn't been fixed, let them pick up the pole
                            else if (playerRec.Intersects(missingPoleItem.GetItemBounds()) && clock.IsSolved())
                            {
                                //Set the currently held item to the pole and set its state to picked up
                                currentlyHeldItem = missingPoleItem;
                                missingPoleItem.SetItemState(true);

                                //Play pick up sound
                                pickUpSnd.CreateInstance().Play();
                            }

                            //Let them pick up the pie at all times 
                            else if (playerRec.Intersects(pieItem.GetItemBounds()))
                            {
                                //Set the currently held item to the pie and set its state to picked up
                                currentlyHeldItem = pieItem;
                                pieItem.SetItemState(true);

                                //Play pick up sound
                                pickUpSnd.CreateInstance().Play();
                            }

                            //If they are near the dust polisher and laser was solved, let them pick it up
                            else if ((playerRec.Intersects(dustPolisherItem.GetItemBounds())) && (laserSolved == true) && (hanoiPuzzle.IsSolved() == true))
                            {
                                //Set the currently held item to the dust polisher and set its state to picked up
                                currentlyHeldItem = dustPolisherItem;
                                dustPolisherItem.SetItemState(true);

                                //Play pick up sound
                                pickUpSnd.CreateInstance().Play();
                            }
                        }
                            
                    }

                    // If player is holding an item, let them drop it
                    else 
                    {  
                        //If they press F
                        if (kb.IsKeyDown(Keys.F) && prevKb.IsKeyUp(Keys.F))
                            {
                            //Drop it at the player's position, update its rectangle, set it to not picked up, and set that there is no currently held item
                                currentlyHeldItem.SetItemPosition(playerPos);
                                currentlyHeldItem.UpdateBounds();          
                                currentlyHeldItem.SetItemState(false);      
                                currentlyHeldItem = null;

                            //Play drop sound effect
                            pickUpSnd.CreateInstance().Play();
                        }
                    }

                    //Clamp the player's movement to only on the floor
                    playerPos.X = MathHelper.Clamp(playerPos.X, 60, screenWidth - playerRec.Width - 60);
                    playerPos.Y = MathHelper.Clamp(playerPos.Y, -5, screenHeight - playerRec.Height+10);

                    //Update the player's speed given their direction
                    speedPlayer.X = dirXPlayer * (maxSpeedPlayer * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    speedPlayer.Y = dirYPlayer * (maxSpeedPlayer * (float)gameTime.ElapsedGameTime.TotalSeconds);

                    //Add the speeds to the every object's true position
                    playerPos.X = playerPos.X + speedPlayer.X;
                    playerPos.Y = playerPos.Y + speedPlayer.Y;

                    //Set the every object's bounding box position equal to its true position
                    playerRec.X = (int)(playerPos.X);
                    playerRec.Y = (int)(playerPos.Y);

                    //If the laser is solved, play a ding (boolean to avoid repition of the sound effect)
                    if (!dingSoundPlayed[5] && laserSolved)
                    {
                        dingSnd.CreateInstance().Play();
                        dingSoundPlayed[5] = true;
                    }

                    // If the user is currently picking the lock
                    if (pickingLock)
                    {
                        // If they press the Right arrow, move to the next lock slot
                        if ((kb.IsKeyDown(Keys.Right)) && (!prevKb.IsKeyDown(Keys.Right)))
                        {
                            // Increment the selected slot, wrapping around to slot 0 after slot 2
                            selectedLockSlot = (selectedLockSlot + 1) % 3;
                        }
                        // If they press the Left arrow, move to the previous lock slot
                        else if ((kb.IsKeyDown(Keys.Left)) && (!prevKb.IsKeyDown(Keys.Left)))
                        {
                            // Decrement the selected slot, wrapping around to slot 2 if moving past slot 0
                            selectedLockSlot = (selectedLockSlot - 1 + 3) % 3;
                        }

                        // If they press the Up arrow, increase the number in the selected slot
                        if ((kb.IsKeyDown(Keys.Up)) && (!prevKb.IsKeyDown(Keys.Up)))
                        {
                            // Increment the number, wrapping around to 0 after 9
                            selectedLockNum[selectedLockSlot] = (selectedLockNum[selectedLockSlot] + 1) % 10;
                        }
                        // If they press the Down arrow, decrease the number in the selected slot
                        else if ((kb.IsKeyDown(Keys.Down)) && (!prevKb.IsKeyDown(Keys.Down)))
                        {
                            // Decrement the number, wrapping around to 9 if moving past 0
                            selectedLockNum[selectedLockSlot] = (selectedLockNum[selectedLockSlot] - 1 + 10) % 10;
                        }

                        // Check if the lock combination is correct (3-1-4) and mark the lock as solved
                        if (selectedLockNum[0] == 3 && selectedLockNum[1] == 1 && selectedLockNum[2] == 4)
                        {
                            //Set lock to solved 
                            lockSolved = true;

                            //Play a ding (boolean to avoid repition of the sound effect)
                            if (!dingSoundPlayed[2])
                            {
                                dingSnd.CreateInstance().Play();
                                dingSoundPlayed[2] = true;
                            }
                        }
                    }

                    // If the player is not currently inspecting the scroll
                    if (!isInspectingScroll)
                    {
                        // Check if the player presses E while near the scroll to start inspecting it
                        if ((kb.IsKeyDown(Keys.E)) && (!prevKb.IsKeyDown(Keys.E)) && playerRec.Intersects(scroll.GetScrollRec()))
                        {
                            //Play button sound
                            buttonSnd.CreateInstance().Play();

                            // Enter inspection mode
                            isInspectingScroll = true; 
                        }

                        // If the player has the dust polisher and presses Q, reveal the symbols on the scroll
                        if (dustPolisherItem.GetItemState() && kb.IsKeyDown(Keys.Q))
                        {
                            //Play dust sound
                            dustSnd.CreateInstance().Play();

                            // Reveal hidden symbols
                            scroll.RevealSymbols();
                        }
                    }
                    else
                    {
                        // If the player presses E again while inspecting the scroll, exit inspection mode
                        if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
                        {
                            // Exit inspection mode
                            isInspectingScroll = false;
                        }
                    }

                break;

                // Handle the updating when in the CLOCK state
                case CLOCK:

                    // Update the clock logic, including any interactions or animations
                    clock.Update(gameTime, kb);

                    //If the clock is solved, play a ding (boolean to avoid repition of the sound effect)
                    if (!dingSoundPlayed[3] && clock.IsSolved())
                    {
                        dingSnd.CreateInstance().Play();
                        dingSoundPlayed[3] = true;
                    }

                    // Check if the player presses E to exit the clock view and return to ROOM1
                    if ((kb.IsKeyDown(Keys.Escape) && prevKb.IsKeyUp(Keys.Escape)))
                    {
                        gameState = ROOM1; 
                    }

                break;

                // Handle the updating when in the PROMPTING state
                case PROMPTING:

                    if (!inCampaign)
                    {
                        // Capture player input and check if Enter was pressed to see if we can move on
                        enterPressed = HandleTextInput(kb, prevKb, ref playerName);

                        // If Enter was pressed and the player's name is valid (not empty or whitespace)
                        if (enterPressed && !string.IsNullOrWhiteSpace(playerName))
                        {
                            // Save leaderboard entries based on the current puzzle name
                            if (currentPuzzleName == "Sliding")
                            {
                                // Save time entry if the player got a time highscore or highscores for both
                                if (promptType == "Time" || promptType == "Both")
                                {
                                    slidingLeaderboard.AddTimeEntry(playerName, Math.Round(slidingTimer.GetTimePassed() / 1000, 2));
                                }

                                // Save move entry if the player got a move highscore or highscores for both
                                if (promptType == "Moves" || promptType == "Both")
                                {
                                    slidingLeaderboard.AddMovesEntry(playerName, slidingPuzzle.GetMoveCount());
                                }

                                // Save and load the sliding puzzle leaderboard
                                slidingLeaderboard.SaveLeaderboard();
                                slidingLeaderboard.LoadLeaderboard();
                            }
                            else if (currentPuzzleName == "LightsOut")
                            {
                                // Save time entry if the player got a time highscore or highscores for both
                                if (promptType == "Time" || promptType == "Both")
                                {
                                    lightsLeaderboard.AddTimeEntry(playerName, Math.Round(lightsTimer.GetTimePassed() / 1000, 2));
                                }

                                // Save move entry if the player got a move highscore or highscores for both
                                if (promptType == "Moves" || promptType == "Both")
                                {
                                    lightsLeaderboard.AddMovesEntry(playerName, lightsOutPuzzle.GetMoveCount());
                                }

                                // Save and load the LightsOut puzzle leaderboard
                                lightsLeaderboard.SaveLeaderboard();
                                lightsLeaderboard.LoadLeaderboard();
                            }
                            else if (currentPuzzleName == "Hanoi")
                            {
                                // Save time entry if the player got a time highscore or highscores for both
                                if (promptType == "Time" || promptType == "Both")
                                {
                                    hanoiLeaderboard.AddTimeEntry(playerName, Math.Round(hanoiTimer.GetTimePassed() / 1000, 2));
                                }

                                // Save move entry if the player got a move highscore or highscores for both
                                if (promptType == "Moves" || promptType == "Both")
                                {
                                    hanoiLeaderboard.AddMovesEntry(playerName, hanoiPuzzle.GetMoveCount());
                                }

                                // Save the load the hanoi leaderboard
                                hanoiLeaderboard.SaveLeaderboard();
                                hanoiLeaderboard.LoadLeaderboard();
                            }

                            // Clear the player's name after saving to prepare for the next input
                            playerName = string.Empty;

                            // Reset the prompt type to a temporary value
                            promptType = "temp";

                            // Transition back to the puzzle menu game state
                            gameState = PUZZLE_MENU;
                        }

                    //If they're in the campaign
                    }
                    else
                    {
                        //If they press escape send them back to the room
                        if(kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
                        {
                            //Send them back to the room 
                            isEnteringPasscode = false;
                            gameState = ROOM1;
                        }

                        //Call the method to handle the text input and record if they presed enter
                        enterPressed = HandleTextInput(kb, prevKb, ref passcodeInput);

                        //If they pressed enter and it was the correct passcode, send them to victory
                        if (enterPressed)
                        {
                            if (passcodeInput == correctPasscode)
                            {
                                //Send them to victory, set campaign to played, and play the sound effects 
                                campaignPlayed = true;
                                MediaPlayer.Stop();
                                applauseSnd.CreateInstance().Play();
                                winSnd.CreateInstance().Play();
                                gameState = VICTORY;
                            }

                            //If it was wrong, play the buzzer sound and send them to the escape room
                            else
                            {
                                buzzerSnd.CreateInstance().Play();
                                isEnteringPasscode = false;
                                passcodeInput = "";
                                gameState = ROOM1;
                            }
                        }
                    }
                    
                break;

                // Handle the updating when in the HANOI state
                case HANOI:

                    // Update the state of the Hanoi puzzle
                    hanoiPuzzle.Update(gameTime, mouse, kb, prevKb);

                    // Check if the game is not in campaign mode
                    if (!inCampaign)
                    {
                        // Update and display the timer for the Hanoi puzzle
                        displayHanoiTimer = Convert.ToString(Math.Round(hanoiTimer.GetTimePassed() / 1000, 2));

                        // Check if the player clicked the "Return to Puzzle Menu" button and they didn't get a highscore (because then they should go to prompting)
                        if ((mouse.LeftButton == ButtonState.Pressed && buttonRecs[5].Contains(mouse.Position)) && !highscoreAcheived)
                        {
                            //Play the button sound
                            buttonSnd.CreateInstance().Play();
                            gameState = PUZZLE_MENU; // Set the game state to the puzzle menu
                        }

                        // Check if the Hanoi puzzle is solved
                        if (hanoiPuzzle.IsSolved())
                        {
                            currentPuzzleName = "Hanoi"; // Set the current puzzle name
                            hanoiTimer.Pause(); // Pause the puzzle timer
                            
                            // Check if the player's time qualifies as a top time
                            if (hanoiLeaderboard.IsTopTime(hanoiTimer.GetTimePassed()))
                            {
                                //State that the highscore's been acheived and start the timer to send them into prompting
                                highscoreAcheived = true;
                                afterPuzzleTimer.Activate(); // Activate the timer for post-puzzle actions

                                // Determine if the player's moves also qualify as a top score
                                if (hanoiLeaderboard.IsTopMove(hanoiPuzzle.GetMoveCount()))
                                {
                                    promptType = "Both"; // Prompt for both time and moves
                                }
                                else
                                {
                                    promptType = "Time"; // Prompt for time only
                                }

                                // If the post-puzzle timer has finished, prompt the player for their name
                                if (afterPuzzleTimer.IsFinished())
                                {
                                    playerName = ""; // Reset the player's name
                                    gameState = PROMPTING; // Set the game state to prompting
                                }
                            }
                            // Check if the player's moves qualify as a top score, but not their time
                            else if (hanoiLeaderboard.IsTopMove(hanoiPuzzle.GetMoveCount()) && !(hanoiLeaderboard.IsTopTime(hanoiTimer.GetTimePassed())))
                            {
                                //State that the highscore's been acheived and start the timer to send them into prompting
                                highscoreAcheived = true;
                                afterPuzzleTimer.Activate();

                                promptType = "Moves"; // Prompt for moves only

                                // If the post-puzzle timer has finished, prompt the player for their name
                                if (afterPuzzleTimer.IsFinished())
                                {
                                    playerName = ""; // Reset the player's name
                                    gameState = PROMPTING; // Set the game state to prompting
                                }
                            }
                        }
                    }
                    else
                    {
                        // If the 'Esc' key is pressed, return to Room 1
                        if (kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape))
                        {
                            gameState = ROOM1;
                        }

                        //If the hanoi puzzle is solved, play a ding and laser sound (boolean to avoid repition of the sound effect)
                        if (hanoiPuzzle.IsSolved() && (!dingSoundPlayed[4]) && !laserSoundPlayed)
                        {
                            dingSnd.CreateInstance().Play();
                            laserSnd.CreateInstance().Play();
                            laserSoundPlayed = true;
                            dingSoundPlayed[4] = true;
                        }
                    }

                break;

                case LIGHTS:

                    // Update the state of the Lights Out puzzle
                    lightsOutPuzzle.Update(gameTime, mouse, kb, prevKb);

                    // Display the Lights Out timer in seconds
                    displayLightsTimer = Convert.ToString(Math.Round(lightsTimer.GetTimePassed() / 1000, 2));

                    // Handle logic for campaign mode
                    if (inCampaign)
                    {
                        // If the puzzle is solved or they press escape, return to Room 1
                        if ((lightsOutPuzzle.IsSolved()) || (kb.IsKeyDown(Keys.Escape)))
                        {
                            gameState = ROOM1; // Set the game state to Room 1
                        }

                        //If the lights puzzle is solved, play a ding (boolean to avoid repition of the sound effect)
                        if (!dingSoundPlayed[0] && lightsOutPuzzle.IsSolved())
                        {
                            dingSnd.CreateInstance().Play();
                            dingSoundPlayed[0] = true;
                        }
                    }
                    else
                    {
                        // If the player clicks the "Return to Puzzle Menu" button and they haven't gotten a highscore (because then they should go to prompting)
                        if ((mouse.LeftButton == ButtonState.Pressed && buttonRecs[6].Contains(mouse.Position))  && !highscoreAcheived)
                        {
                            //Play button sound effect
                            buttonSnd.CreateInstance().Play();
                            gameState = PUZZLE_MENU; // Set the game state to the puzzle menu
                        }

                        // Check if the Lights Out puzzle is solved
                        if (lightsOutPuzzle.IsSolved())
                        {
                            currentPuzzleName = "LightsOut"; // Set the current puzzle name
                            lightsTimer.Pause(); // Pause the puzzle timer

                            // Check if the player's time qualifies as a top time
                            if (lightsLeaderboard.IsTopTime(lightsTimer.GetTimePassed()))
                            {
                                //State that the highscore's been acheived and start the timer to send them into prompting
                                highscoreAcheived = true;
                                afterPuzzleTimer.Activate(); // Activate the timer for post-puzzle actions
                                // Determine if the player's moves also qualify as a top score
                                if (lightsLeaderboard.IsTopMove(lightsOutPuzzle.GetMoveCount()))
                                {
                                    promptType = "Both"; // Prompt for both time and moves
                                }
                                else
                                {
                                    promptType = "Time"; // Prompt for time only
                                }

                                // If the post-puzzle timer has finished, transition to the prompting state
                                if (afterPuzzleTimer.IsFinished())
                                {
                                    gameState = PROMPTING; // Set the game state to prompting
                                }
                            }
                            // Check if the player's moves qualify as a top score, but not their time
                            else if (lightsLeaderboard.IsTopMove(lightsOutPuzzle.GetMoveCount()) && !(lightsLeaderboard.IsTopTime(lightsTimer.GetTimePassed())))
                            {
                                //State that the highscore's been acheived and start the timer to send them into prompting
                                highscoreAcheived = true;
                                afterPuzzleTimer.Activate(); // Activate the timer for post-puzzle actions

                                promptType = "Moves"; // Prompt for moves only

                                // If the post-puzzle timer has finished, transition to the prompting state
                                if (afterPuzzleTimer.IsFinished())
                                {
                                    gameState = PROMPTING; // Set the game state to prompting
                                }
                            }
                        }
                    }

                break;
                
                case SLIDING:

                    // Update the state of the Sliding puzzle
                    slidingPuzzle.Update(gameTime, Mouse.GetState(), Keyboard.GetState(), prevKb);

                    // Display the Sliding puzzle timer in seconds
                    displaySlidingTimer = Convert.ToString(Math.Round(slidingTimer.GetTimePassed() / 1000, 2));

                    //If we're in campaign, execute specific campaign logic
                    if (inCampaign)
                    {
                        // In campaign mode, pressing 'E' exits back to Room 1
                        if ((kb.IsKeyDown(Keys.Escape) && !prevKb.IsKeyDown(Keys.Escape)))
                        {
                            gameState = ROOM1; // Set the game state to Room 1
                        }

                        //If the sliding puzzle is solved, play a ding (boolean to avoid repition of the sound effect)
                        if (!dingSoundPlayed[1] && slidingPuzzle.IsSolved())
                        {
                            dingSnd.CreateInstance().Play();
                            dingSoundPlayed[1] = true;
                        }
                    }

                    //Execute specific puzzle logic
                    else
                    {
                        // If the player clicks the "Return to Puzzle Menu" button
                        if ((mouse.LeftButton == ButtonState.Pressed && buttonRecs[7].Contains(mouse.Position)) && !highscoreAcheived)
                        {
                            //Play button sound
                            buttonSnd.CreateInstance().Play();
                            gameState = PUZZLE_MENU; // Set the game state to the puzzle menu
                        }

                        // Check if the Sliding puzzle is solved
                        if (slidingPuzzle.IsSolved())
                        {
                            currentPuzzleName = "Sliding"; // Set the current puzzle name
                            slidingTimer.Pause(); // Pause the puzzle timer
                            

                            // Check if the player's time qualifies as a top time
                            if (slidingLeaderboard.IsTopTime(slidingTimer.GetTimePassed()))
                            {
                                //State that the highscore's been acheived and start the timer to send them into prompting
                                highscoreAcheived = true;
                                afterPuzzleTimer.Activate(); // Activate the timer for post-puzzle actions

                                // Determine if the player's moves also qualify as a top score
                                if (slidingLeaderboard.IsTopMove(slidingPuzzle.GetMoveCount()))
                                {
                                    promptType = "Both"; // Prompt for both time and moves
                                }
                                else
                                {
                                    promptType = "Time"; // Prompt for time only
                                }

                                // If the post-puzzle timer has finished, transition to the prompting state
                                if (afterPuzzleTimer.IsFinished())
                                {
                                    gameState = PROMPTING; // Set the game state to prompting
                                }
                            }
                            // Check if the player's moves qualify as a top score, but not their time
                            else if (slidingLeaderboard.IsTopMove(slidingPuzzle.GetMoveCount()) && !(slidingLeaderboard.IsTopTime(slidingTimer.GetTimePassed())))
                            {
                                //State that the highscore's been acheived and start the timer to send them into prompting
                                highscoreAcheived = true;
                                afterPuzzleTimer.Activate(); // Activate the timer for post-puzzle actions

                                promptType = "Moves"; // Prompt for moves only

                                // If the post-puzzle timer has finished, transition to the prompting state
                                if (afterPuzzleTimer.IsFinished())
                                {
                                    gameState = PROMPTING; // Set the game state to prompting
                                }
                            }
                            else
                            {
                                // If neither score qualifies and the player presses 'Space,' return to the puzzle menu
                                if (kb.IsKeyDown(Keys.Space))
                                {
                                    gameState = PUZZLE_MENU; // Set the game state to the puzzle menu
                                }
                            }
                        }
                    }

                break;

                case VICTORY:

                    //If they press the screen take them to the menu
                    if (mousePressed)
                    {
                        gameState = MENU;
                    }

                break;
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            switch (gameState)
            {
                //Handle drawing in menu state
                case MENU:

                    //Draw the buttons and text in the menu
                        spriteBatch.Draw(buttonImg, buttonRecs[0], Color.White);
                        spriteBatch.DrawString(ButtonFont, buttonStrings[0], textPos[0], Color.Red);
                        spriteBatch.Draw(buttonImg, buttonRecs[1], Color.White);
                        spriteBatch.DrawString(ButtonFont, buttonStrings[1], textPos[1], Color.Red);
                        spriteBatch.DrawString(TitleFont, titleStrings[0], titlePos[0], Color.Blue);
                break;

                //Handle drawing in puzzle menu
                case PUZZLE_MENU:

                    //Draw the title of the puzzle menu
                    spriteBatch.DrawString(TitleFont, titleStrings[1], titlePos[1], Color.Blue);

                    //Draw the buttons and text in the puzzle menu
                    for (int i = 2; i < 5; i++)
                    {
                        spriteBatch.Draw(buttonImg, buttonRecs[i], Color.White);
                        spriteBatch.DrawString(ButtonFont, buttonStrings[i], textPos[i], Color.Red);
                    }
                    spriteBatch.Draw(buttonImg, buttonRecs[8] , Color.White);
                    spriteBatch.DrawString(ButtonFont, buttonStrings[8], textPos[14], Color.Red);
                    spriteBatch.Draw(buttonImg, buttonRecs[9], Color.White);
                    spriteBatch.DrawString(ButtonFont, buttonStrings[9], textPos[17], Color.Red);


                break;

                //Handle drawing in leaderboard screen
                case LEADERBOARD:

                    // Draw the title of the leaderboard screen at the top
                    spriteBatch.DrawString(TitleFont, "Leaderboard", new Vector2(275, 30), Color.Blue);

                    //Define starting position
                    leaderboardStartingPosition = new Vector2(90, 105);

                    // Render the Tower of Hanoi leaderboard section
                    spriteBatch.DrawString(ButtonFont, "Tower of Hanoi", leaderboardStartingPosition, Color.DarkRed);
                    hanoiLeaderboard.DisplayLeaderboard("hanoi_leaderboard.txt", spriteBatch, TempFont, leaderboardStartingPosition + (new Vector2(0,40)));

                    // Render the Lights Out leaderboard section updating the starting position to move horizontally
                    leaderboardStartingPosition += new Vector2(200, 0); 
                    spriteBatch.DrawString(ButtonFont, "Lights Out", leaderboardStartingPosition + new Vector2(50, 0), Color.Goldenrod); 
                    lightsLeaderboard.DisplayLeaderboard("lights_leaderboard.txt", spriteBatch, TempFont, leaderboardStartingPosition + new Vector2(50, 0) + (new Vector2(0,40))); 

                    // Render the Sliding Puzzle leaderboard section updating the starting position to move horizontally
                    leaderboardStartingPosition += new Vector2(200, 0);
                    spriteBatch.DrawString(ButtonFont, "Sliding Puzzle", leaderboardStartingPosition + new Vector2(50, 0), Color.LightCoral);
                    slidingLeaderboard.DisplayLeaderboard("sliding_leaderboard.txt", spriteBatch, TempFont, leaderboardStartingPosition + new Vector2(50, 0) + (new Vector2(0, 40))); 

                    // Draw the back button to allow returning to the previous menu
                    spriteBatch.Draw(buttonImg, buttonRecs[10], Color.White);
                    spriteBatch.DrawString(ButtonFont, buttonStrings[10], textPos[18], Color.Red);

                break;

                    //State to handle instructions drawing
                case INSTRUCTIONS:

                    // Clears the screen with a black background
                    GraphicsDevice.Clear(Color.Black);

                    //Write all the instruction texts 
                    spriteBatch.DrawString(ButtonFont, "A rogue AI has turned robots into ruthless sentinels.", instructionsTextLocs[0], Color.Red);
                    spriteBatch.DrawString(ButtonFont, "You are trapped in a high-tech escape room built to test your logic", instructionsTextLocs[1], Color.Red);
                    spriteBatch.DrawString(ButtonFont, "Solve puzzles to outsmart the AI and escape.", instructionsTextLocs[2], Color.Red);
                    spriteBatch.DrawString(ButtonFont, "Time is running out, your wits are your only weapon", instructionsTextLocs[3], Color.Red);
                    spriteBatch.DrawString(ButtonFont, "------------------------------------------------------", instructionsTextLocs[4], Color.White);
                    spriteBatch.DrawString(ButtonFont, "Press E to interact with various items", instructionsTextLocs[5], Color.Cyan);
                    spriteBatch.DrawString(ButtonFont, "Press F to pick items up", instructionsTextLocs[6], Color.Cyan);
                    spriteBatch.DrawString(ButtonFont, "Press Esc to exit anything", instructionsTextLocs[7], Color.Cyan);
                    
                break;

                //Handle drawing in escape room state 
                case ROOM1:

                    //Draw the background
                    spriteBatch.Draw(whiteRoomBg, whiteRoomRec, Color.White);

                    //Draw some interactable objects making sure they get highlighted when the player is near
                    DrawWithIntersectionHighlight(spriteBatch, playerRec, ePanelImg, ePanelRec);
                    DrawWithIntersectionHighlight(spriteBatch, playerRec, clockWallImg, clockWallRec);
                    DrawWithIntersectionHighlight(spriteBatch, playerRec, hanoiImg, hanoiRec);
                    DrawWithIntersectionHighlight(spriteBatch, playerRec, doorImg, doorRec);

                    //Draw the bullseye 
                    spriteBatch.Draw(bullsImg, bullsRec, Color.White);
                    spriteBatch.Draw(eyeImg, eyeRec, Color.White);

                    //If the hanoi puzzle is solved, draw the first laser, and the others as well if the mirrors align
                    if (hanoiPuzzle.IsSolved())
                    {
                        //Draw laser 1 and if the bottom mirror touches the laser, draw the second one
                        newLaser.Draw(spriteBatch, Color.Red);
                        if (mirrors[1].ContainsPoint(newLaser.GetPt2()))
                        {
                            //Draw the second laser, and if the top mirror touches it, draw the third
                            reflectedLaser.Draw(spriteBatch, Color.Red);
                            if (mirrors[0].ContainsPoint(reflectedLaser.GetPt2()))
                            {
                                //Draw the third laser
                                reflectedLaser2.Draw(spriteBatch, Color.Red);
                            }
                        }
                    }

                    //If lights out puzzle gets solved, draw the letters on the wall and the riddle
                    if (lightsOutPuzzle.IsSolved())
                    {
                        //Cycle through the letters to draw them
                        for (int i = 0; i < 4; i++)
                        {
                            spriteBatch.Draw(lettersImgs[i], lettersRecs[i], Color.White);
                        }

                        //Cycle through the riddle lines to write them
                        for (int i = 0; i < 4; i++)
                        {
                            spriteBatch.DrawString(TempFont, riddle[i], riddleTextLocs[i], Color.Blue);
                        }
                    }

                    //Draw each mirror 
                    foreach (var mirror in mirrors)
                    {
                        mirror.Draw(spriteBatch, Color.White, 1);
                    }

                    //Draw each item if specific conditions are met (flow of the escape room)
                    foreach (var item in pickupableItems)
                    {
                        //If the item isn't picked up
                        if (!item.GetItemState())
                        {
                            //If its the puzzle box and the sliding puzzle hasn't been solved, draw it (make it disappear after its been solved)
                            if (item == puzzleBoxItem && !slidingPuzzle.IsSolved())
                            {
                                //Draw it and highlight it if the player intersects
                                DrawWithIntersectionHighlight(spriteBatch, playerRec, item.GetItemImg(), item.GetItemBounds());
                            }

                            //If its the pie item and the sliding puzzle has been solved, draw it (make it appear after sliding has been solved)
                            else if ((item == pieItem) && slidingPuzzle.IsSolved())
                            {
                                //Draw it and highlight it if the player intersects
                                DrawWithIntersectionHighlight(spriteBatch, playerRec, item.GetItemImg(), item.GetItemBounds());
                            }

                            //If its the missing pole and the clock has been solved, draw it (make it appear after clock has been solved)
                            else if ((item == missingPoleItem) && clock.IsSolved())
                            {
                                //Draw it and highlight it if the player intersects
                                DrawWithIntersectionHighlight(spriteBatch, playerRec, item.GetItemImg(), item.GetItemBounds());
                            }

                            //If its the missing clock hand and the lock has been solved, draw it (make it appear after lock has been solved)
                            else if ((item == missingHandItem) && (lockSolved == true))
                            {
                                //Draw it and highlight it if the player intersects
                                DrawWithIntersectionHighlight(spriteBatch, playerRec, item.GetItemImg(), item.GetItemBounds());
                                  
                            }

                            //If its the dust polisher item and the laser has been solved, draw it (make it appear after laser has been solved)
                            else if ((item == dustPolisherItem) && (laserSolved))
                            {
                                //Draw it and highlight it if the player intersects
                                DrawWithIntersectionHighlight(spriteBatch, playerRec, item.GetItemImg(), item.GetItemBounds());
                            }

                        }
                    }

                    // Draw a gray box to as the area the held item will be drawn
                    spriteBatch.Draw(blankSquareImg, currentlyHeldBox, Color.Gray);

                    // If an item is currently being held, draw it in the box and write its name there
                    if (currentlyHeldItem != null)
                    {
                        // Draw the image of the currently held item within the box
                        spriteBatch.Draw(currentlyHeldItem.GetItemImg(), currentlyHeldBox, Color.White);

                        // Calculate the position to draw the item's name above the box
                        Vector2 textPosition = new Vector2(
                            currentlyHeldBox.X + (currentlyHeldBox.Width / 2) - (ButtonFont.MeasureString(currentlyHeldItem.GetItemName()).X / 2), // Center horizontally
                            currentlyHeldBox.Y - 30 // Offset above the box
                        );

                        // Draw the name of the currently held item above the box in black text
                        spriteBatch.DrawString(ButtonFont, currentlyHeldItem.GetItemName(), textPosition, Color.Black);
                    }

                    //If there is no item being held, write empty above the box
                    else
                    {
                        // Display the text "Empty" above the box to indicate the absence of a held item
                        spriteBatch.DrawString(ButtonFont, "Empty", new Vector2(currentlyHeldBox.X + 13, currentlyHeldBox.Y - 30), Color.Black);
                    }

                    // Check if the player is near the clock with the hand and let them know they can fix it
                    if (playerRec.Intersects(clockWallRec) && missingHandItem.GetItemState() && !clock.IsFixed())
                    {
                        // Display instructions for fixing the clock
                        spriteBatch.DrawString(TempFont, "Press Q to ", new Vector2(685, 40), Color.Black);
                        spriteBatch.DrawString(TempFont, "Fix Clock ", new Vector2(690, 55), Color.Black);
                    }

                    // Check if the player is near the Tower of Hanoi puzzle with the missing pole and let them know they can fix it
                    if (playerRec.Intersects(hanoiRec) && missingPoleItem.GetItemState() && !hanoiPuzzle.IsFixed())
                    {
                        // Display instructions for fixing the rings
                        spriteBatch.DrawString(TempFont, "Press Q to ", new Vector2(490, 150), Color.Black);
                        spriteBatch.DrawString(TempFont, "Fix Rings ", new Vector2(500, 170), Color.Black);
                    }

                    // Check if the player is near the scroll with dust polisher and let them know they can fix it
                    if (playerRec.Intersects(scroll.GetScrollRec()) && dustPolisherItem.GetItemState() && isInspectingScroll == false && !scroll.IsFixed())
                    {
                        // Display instructions for polishing the scroll
                        spriteBatch.DrawString(TempFont, "Press Q to ", new Vector2(15, 210), Color.Black);
                        spriteBatch.DrawString(TempFont, "Polish Dust ", new Vector2(15, 225), Color.Black);
                    }

                    // Check if the player is not near the lock and the lock is not yet solved
                    if ((!playerRec.Intersects(lockRec)) && !(lockSolved))
                    {
                        // Draw the safe image in its default state
                        spriteBatch.Draw(safeImg, lockRec, Color.White);
                    }
                    else
                    {
                        // If the lock is not solved but the player is near it
                        if (!lockSolved)
                        {
                            // Highlight the safe image in light green to indicate interaction is possible
                            spriteBatch.Draw(safeImg, lockRec, Color.LightGreen);

                            // If the player is currently picking the lock
                            if (pickingLock)
                            {
                                // Draw the lock combination slots (background layers)
                                spriteBatch.Draw(whiteSquareImg, lockComboRecs[0], Color.White);
                                spriteBatch.Draw(whiteSquareImg, lockComboRecs[1], Color.White);
                                spriteBatch.Draw(whiteSquareImg, lockComboRecs[2], Color.White);

                                // Draw the overlay for the combination slots
                                spriteBatch.Draw(blankSquareImg, lockComboRecs[0], Color.White);
                                spriteBatch.Draw(blankSquareImg, lockComboRecs[1], Color.White);
                                spriteBatch.Draw(blankSquareImg, lockComboRecs[2], Color.White);

                                // Iterate through each combination slot
                                for (int i = 0; i < 3; i++)
                                {
                                    // Draw the number in the current slot and highlight it red or black depending on if its highlighted
                                    spriteBatch.DrawString(MediumFont,  Convert.ToString(selectedLockNum[i]),comboLockLoc[i], (i == selectedLockSlot) ? Color.Red : Color.Black);
                                }
                            }
                        }
                    }

                    //Draw the player and the scroll
                    spriteBatch.Draw(playerImg, playerRec, Color.White);
                    scroll.Draw(spriteBatch, isInspectingScroll);

                    //If the player intersects the scroll make it green to signal interactive item
                    if (playerRec.Intersects(scroll.GetScrollRec()) && !isInspectingScroll)
                    {
                        spriteBatch.Draw(lyingScrollImg, scroll.GetScrollRec(), Color.LightGreen);
                    }
                    
                break;

                //Handle drawing in clock state 
                case CLOCK:

                    //Draw the clock and the hands (will only draw the second hand if clock is fixed)
                    clock.Draw(spriteBatch);

                    spriteBatch.DrawString(MediumFont, "Arrow Keys to Control Clock", CenterTextHorizontally(MediumFont, "Arrow Keys to Control Clock", screenWidth, 400), Color.Blue);
                    //If the clock is fixed, write the message to let them know how to switch hands
                    if (clock.IsFixed())
                    {
                        spriteBatch.DrawString(MediumFont, "Press Space to Switch Hands", textPos[19], Color.Red);
                    }
                    
                break;

                //Handle drawing in prompting state 
                case PROMPTING:

                    if (!inCampaign)
                    {
                        // Display the name entry prompt and player's current input
                        spriteBatch.DrawString(TitleFont, "Enter your name:", new Vector2(100, 100), Color.Blue);
                        spriteBatch.DrawString(TitleFont, playerName + "_", new Vector2(100, 150), Color.LightGreen);
                    }
                    else
                    {
                        // Display the passcode entry prompt and player's current input
                        spriteBatch.DrawString(TitleFont, "Enter Passcode:", new Vector2(100, 100), Color.Blue);
                        spriteBatch.DrawString(TitleFont, passcodeInput + "_", new Vector2(100, 150), Color.LightGreen);
                    }

                break;

                // Handle drawing in Tower of Hanoi puzzle
                case HANOI:

                    // Draw the Hanoi puzzle
                    hanoiPuzzle.Draw(spriteBatch);

                    //If in puzzle mode, draw specific puzzle mode things
                    if (!inCampaign)
                    {
                        // Display "HighScore!" message if applicable
                        if ((promptType == "Both") || (promptType == "Time") || (promptType == "Moves"))
                        {
                            spriteBatch.DrawString(MediumFont, "HighScore!", (CenterTextHorizontally(MediumFont, "HighScore!", screenWidth, 50)), Color.Blue);
                        }

                        // Draw back button and puzzle-specific stats
                        spriteBatch.Draw(buttonImg, buttonRecs[5], Color.White);
                        spriteBatch.DrawString(ButtonFont, buttonStrings[5], textPos[5], Color.White);
                        spriteBatch.DrawString(ButtonFont, "Time: " + displayHanoiTimer + "s", textPos[8], Color.Red);
                        spriteBatch.DrawString(ButtonFont, "Move Count: " + Convert.ToString(hanoiPuzzle.GetMoveCount()), textPos[10], Color.Red);

                        // Draw the puzzle title
                        spriteBatch.DrawString(MediumFont, buttonStrings[2], titlePos[2], Color.Black);
                    }
                break;

                // Handle drawing in Lights Out puzzle
                case LIGHTS:

                    // Clear the screen and draw the Lights Out puzzle
                    GraphicsDevice.Clear(Color.Red);
                    lightsOutPuzzle.Draw(spriteBatch);

                    //Draw specific puzzle messages and stats
                    if (!inCampaign)
                    {
                        // Display "HighScore!" message if applicable
                        if ((promptType == "Both") || (promptType == "Time") || (promptType == "Moves"))
                        {
                            spriteBatch.DrawString(MediumFont, "HighScore!", (CenterTextHorizontally(MediumFont, "HighScore!", screenWidth, 440)), Color.Blue);
                        }

                        // Draw back button and puzzle-specific stats
                        spriteBatch.Draw(buttonImg, buttonRecs[6], Color.White);
                        spriteBatch.DrawString(ButtonFont, buttonStrings[5], textPos[6], Color.White);
                        spriteBatch.DrawString(ButtonFont, "Time: " + displayLightsTimer + "s", textPos[9], Color.Blue);
                        spriteBatch.DrawString(ButtonFont, "Move Count: " + Convert.ToString(lightsOutPuzzle.GetMoveCount()), textPos[11], Color.Blue);

                        // Draw the puzzle title
                        spriteBatch.DrawString(MediumFont, buttonStrings[3], titlePos[3], Color.Black);
                    }

                    //Draw specific campaign messages and stats
                    else
                    {
                        spriteBatch.DrawString(MediumFont, "Fix The Lights!", CenterTextHorizontally(MediumFont, "Fix The Lights!", screenWidth, 25), Color.White);
                    }

                break;

                // Handle drawing in Sliding Puzzle state
                case SLIDING:

                    //If the sliding puzzle is solved and we're in campaign, draw the pie and the message
                    if (slidingPuzzle.IsSolved())
                    {
                        if (inCampaign)
                        {
                            // Display victory image and messages
                            spriteBatch.Draw(pieImg, new Rectangle(280, 130, 210, 210), Color.White);
                            spriteBatch.DrawString(ButtonFont, pieMessage, textPos[15], Color.Blue);
                            spriteBatch.DrawString(ButtonFont, "                                             e ...", textPos[15], Color.Red);
                            spriteBatch.DrawString(ButtonFont, pressSpaceMessage, textPos[16], Color.Blue);
                        }
                    }

                    //If it hasn't been solved just draw it normally
                    else
                    {
                        // Draw the Sliding puzzle if not solved
                        slidingPuzzle.Draw(spriteBatch);
                    }

                    //If we're in puzzle mode draw specific puzzle mode things (like button and stats)
                    if (!inCampaign)
                    {
                        // Display "HighScore!" message if applicable
                        if ((promptType == "Both") || (promptType == "Time") || (promptType == "Moves"))
                        {
                            spriteBatch.DrawString(MediumFont, "HighScore!", (CenterTextHorizontally(MediumFont, "HighScore!", screenWidth, 30)), Color.Blue);
                        }

                        //If it hasn't been solved yet, just draw the puzzle normally
                        if (!slidingPuzzle.IsSolved())
                        {
                            // Draw the puzzle if not solved
                            slidingPuzzle.Draw(spriteBatch);
                        }

                        //If its been solved, draw the final image
                        else
                        {
                            // Draw the final solved image
                            spriteBatch.Draw(slidingPuzzle.GetSlidingImg(), new Rectangle(280, 130, 210, 210), Color.White);
                        }

                        // Draw back button and puzzle-specific stats
                        spriteBatch.Draw(buttonImg, buttonRecs[7], Color.White);
                        spriteBatch.DrawString(MediumFont, buttonStrings[4], titlePos[4], Color.Black);
                        spriteBatch.DrawString(ButtonFont, buttonStrings[7], textPos[7], Color.White);
                        spriteBatch.DrawString(ButtonFont, "Time: " + displaySlidingTimer + "s", textPos[9] - new Vector2(20,100), Color.Blue);
                        spriteBatch.DrawString(ButtonFont, "Move Count: " + Convert.ToString(slidingPuzzle.GetMoveCount()), textPos[11] - new Vector2(20, 100), Color.Blue);
                    }

                break;

                case VICTORY:

                    //Write out the victory messages
                    spriteBatch.DrawString(TitleFont, "You ESCAPED!", CenterTextHorizontally(TitleFont, "You ESCAPED!", screenWidth, 60), Color.Black);
                    spriteBatch.DrawString(MediumFont, "You have outsmarted the AI and proven", CenterTextHorizontally(MediumFont, "You have outsmarted the AI and proven", screenWidth, 150), Color.Blue);
                    spriteBatch.DrawString(MediumFont, "that human ingenuity triumphs over machine!!!", CenterTextHorizontally(MediumFont, "that human ingenuity triumphs over machine!!!", screenWidth, 240), Color.Blue);
                    spriteBatch.DrawString(MediumFont, "Click anywhere to proceed to Main Menu", CenterTextHorizontally(MediumFont, "Click anywhere to proceed to Main Menu", screenWidth, 330), Color.Red);

                break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }


        // Generates a randomized, solvable sliding puzzle configuration
        // Pre: gridSize is the size of the sliding puzzle grid (e.g., 3 for a 3x3 grid)
        // Post: Returns a 2D array of integers representing a shuffled, solvable sliding puzzle
        public int[,] GenerateRandomSlidingNums(int gridSize)
        {
            // List to hold all tile values (0 for the blank, and 1 to gridSize^2 - 1 for numbered tiles)
            List<int> tileValues = new List<int>();

            // Populate the list with numbers from 0 to gridSize^2 - 1
            for (int i = 0; i < gridSize * gridSize; i++)
            {
                tileValues.Add(i);
            }

            // Random instance for shuffling
            Random random = new Random();

            // Shuffle the list using the Fisher-Yates algorithm
            for (int i = tileValues.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1); // Generate a random index
                int temp = tileValues[i]; // Swap the current index with the random index
                tileValues[i] = tileValues[j];
                tileValues[j] = temp;
            }

            // Ensure the shuffled configuration is solvable
            while (!IsSolvable(tileValues, gridSize))
            {
                // Shuffle again until a solvable configuration is found
                for (int i = tileValues.Count - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1); // Generate a random index
                    int temp = tileValues[i]; // Swap the current index with the random index
                    tileValues[i] = tileValues[j];
                    tileValues[j] = temp;
                }
            }

            // Convert the shuffled list into a 2D array for the sliding puzzle grid
            int[,] slidingNums = new int[gridSize, gridSize];
            int index = 0; // Index to track the position in the shuffled list

            // Fill the 2D array row by row with the shuffled numbers
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    slidingNums[i, j] = tileValues[index++];
                }
            }

            // Return the randomized, solvable 2D array
            return slidingNums;
        }

        // Pre: text is a string, font is a valid SpriteFont, buttonRectangle is a Rectangle defining the button's bounds
        // Post: Returns a Vector2 containing the position to center the text within the button
        // Description: Calculates the position to draw text centered horizontally and vertically inside a button
        private Vector2 CenterTextOnButton(string text, SpriteFont font, Rectangle buttonRectangle)
        {
            // Measure the size of the text to determine its dimensions
            Vector2 textSize = font.MeasureString(text);

            // Calculate the X position to center the text horizontally in the button
            float centeredX = buttonRectangle.X + (buttonRectangle.Width - textSize.X) / 2;

            // Calculate the Y position to center the text vertically in the button
            float centeredY = buttonRectangle.Y + (buttonRectangle.Height - textSize.Y) / 2;

            // Return the position as a Vector2
            return new Vector2(centeredX, centeredY);
        }

        // Pre: font is a valid SpriteFont, text is a string, screenWidth is the screen's width, yPosition is the desired Y position
        // Post: Returns a Vector2 containing the position to center the text horizontally at the given Y position
        // Description: Calculates the position to draw text centered horizontally across the screen
        private Vector2 CenterTextHorizontally(SpriteFont font, string text, int screenWidth, int yPosition)
        {
            // Measure the width of the text
            float textWidth = font.MeasureString(text).X;

            // Calculate the X position to center the text horizontally
            float centeredX = (screenWidth - textWidth) / 2;

            // Return the position as a Vector2, keeping the Y position as specified
            return new Vector2(centeredX, yPosition);
        }

        // Pre: spriteBatch is a valid SpriteBatch, playerRec and targetRec are Rectangles, targetImg is a valid Texture2D
        // Post: Draws the target image with a highlight color if it intersects the player rectangle
        // Description: Checks for intersection between playerRec and targetRec, drawing the image in LightGreen if intersecting, otherwise White
        public void DrawWithIntersectionHighlight(SpriteBatch spriteBatch, Rectangle playerRec, Texture2D targetImg, Rectangle targetRec)
        {
            // Determine the color based on whether the player rectangle intersects the target rectangle
            Color drawColor = playerRec.Intersects(targetRec) ? Color.LightGreen : Color.White;

            // Draw the target image with the calculated color
            spriteBatch.Draw(targetImg, targetRec, drawColor);
        }

        // Pre: rectangleWidth is a positive integer representing the width of a rectangle
        // Post: Returns an integer representing the X coordinate to center the rectangle horizontally
        // Description: Calculates the X coordinate to position a rectangle centered on the screen
        public int GetCenteredX(int rectangleWidth)
        {
            return (screenWidth - rectangleWidth) / 2;
        }

        // Pre: rectangleHeight is a positive integer representing the height of a rectangle
        // Post: Returns an integer representing the Y coordinate to center the rectangle vertically
        // Description: Calculates the Y coordinate to position a rectangle centered on the screen
        public int GetCenteredY(int rectangleHeight)
        {
            return (screenHeight - rectangleHeight) / 2;
        }


        // Pre: currentKb represents the current keyboard state, previousKb represents the keyboard state from the last frame, 
        //      input is a reference to the string being updated with text input.
        // Post: Updates the input string based on current keyboard input, and returns true if the Enter key is pressed.
        // Description: Handles text input by processing key presses, allowing letters and digits, and supporting Backspace and Enter keys.
        private bool HandleTextInput(KeyboardState currentKb, KeyboardState previousKb, ref string input)
        {
            // Get the currently pressed keys
            Keys[] pressedKeys = currentKb.GetPressedKeys();
            bool enterPressed = false; // Tracks whether Enter has been pressed

            foreach (var key in pressedKeys)
            {
                // Ignore keys that were already pressed in the previous frame to prevent repeated input
                if (previousKb.IsKeyDown(key)) continue;

                // Handle Backspace to delete the last character if the input is not empty
                if (key == Keys.Back && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1); // Remove the last character
                }

                // Ensure input does not exceed the maximum length of 10 characters
                if (input.Length < 10)
                {
                    if (key >= Keys.A && key <= Keys.Z) // Check for letter keys
                    {
                        // Append the pressed key's string representation to the input
                        input += key.ToString();
                    }
                    else if (key >= Keys.D0 && key <= Keys.D9) // Check for digit keys
                    {
                        // Append the numeric value of the key by removing the "D" prefix
                        input += key.ToString().Replace("D", "");
                    }
                }

                // Handle the Enter key to signal the end of input
                if (key == Keys.Enter)
                {
                    enterPressed = true; // Mark Enter as pressed
                }
            }

            // Return true if Enter was pressed, otherwise false
            return enterPressed;
        }

        // Pre: tileValues is a list of integers representing the puzzle layout, gridSize is the size of the grid (e.g., 3 for a 3x3 grid).
        // Post: Returns true if the puzzle configuration is solvable, false otherwise.
        // Description: Determines the solvability of a sliding puzzle based on the number of inversions and the position of the blank tile.
        private bool IsSolvable(List<int> tileValues, int gridSize)
        {
            int inversions = 0; // Count the number of inversions in the tile array

            // Calculate the number of inversions in the flat tileValues list
            for (int i = 0; i < tileValues.Count; i++)
            {
                for (int j = i + 1; j < tileValues.Count; j++)
                {
                    // An inversion occurs if a larger number precedes a smaller number (ignoring the blank tile, represented as 0)
                    if (tileValues[i] > tileValues[j] && tileValues[i] != 0 && tileValues[j] != 0)
                    {
                        inversions++;
                    }
                }
            }

            // Case 1: If the grid size is odd, the puzzle is solvable if the number of inversions is even
            if (gridSize % 2 != 0)
            {
                return inversions % 2 == 0;
            }

            // Case 2: If the grid size is even, the position of the blank tile (0) affects solvability
            // Calculate the row of the blank tile from the bottom (1-based index)
            int blankRowFromBottom = gridSize - (tileValues.IndexOf(0) / gridSize);

            // If the blank tile is on an even row from the bottom
            if (blankRowFromBottom % 2 == 0)
            {
                // Puzzle is solvable if the number of inversions is odd
                return inversions % 2 != 0;
            }
            else // If the blank tile is on an odd row from the bottom
            {
                // Puzzle is solvable if the number of inversions is even
                return inversions % 2 == 0;
            }
        }
    }
}


