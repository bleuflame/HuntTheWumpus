using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text.RegularExpressions;

namespace HuntTheWumpus
{
    /// 
    /// This is the main type for your game
    ///
    public class Game1 : Microsoft.Xna.Framework.Game
    {


        Room[,] map;
        Player player;

        IHazard wumpus;
        IHazard pits;
        IHazard bats;

        //Default settings
        Room wumpusDefRoom, playerDefRoom, wumpusRoom;
        Room[] pitDefRoom = new Room[2];
        Room[] batsDefRoom = new Room[2];


        //GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;

        public Game1()
        {
            //graphics = new GraphicsDeviceManager(this);
            //Content.RootDirectory = "Content";

            SetRooms();  //Sets up each Room object connected to each other

            PutInRooms(); //Places each player and hazard object on the map

            instructions();

            do //While true (Forever until the game is closed)
            {
                //MAIN GAME LOOP
                do //While the player and wumpus are still alive
                {
                    WumpusTurn();  //Wumpus takes his turn
                    player.current_room.HazardCheck(); //Check the current room for nearby hazards for messages.
                    PlayerTurn(); //Allow the player to shoot or move. 
                    Console.WriteLine(); //Cosmetic use only
                    HazardEffects(); //If the player lands in a hazard, the effect goes off
                }
                while ((player.alive == true) && (wumpus.alive == true));

                //End game message
                if (player.alive == false)
                    Console.WriteLine("YOU LOSE");
                else if (wumpus.alive == false)
                    Console.WriteLine("YOU'VE WON!");

                String same_settings;  //Same settings? Yes or No?

                do //While the string is no equal to Y and N. 
                {
                    Console.WriteLine("PLAY AGAIN WITH SAME SETTINGS? YES OR NO?");
                    same_settings = Console.ReadLine();
                    same_settings = same_settings[0].ToString(); //Take first character, in case the player says "yes" or "no" rather than Y or N. 
                }
                while (!String.Equals(same_settings, "Y", StringComparison.OrdinalIgnoreCase) && (!String.Equals(same_settings, "N", StringComparison.OrdinalIgnoreCase)));

                GameReset(same_settings); //Reset game and send same setting preferences.
                Console.WriteLine();
            }
            while (true);
        }


        //
        public void instructions()
        {
            Console.WriteLine("There are 20 rooms, each connected to three other rooms.");
            Console.WriteLine("The game contains three hazards.\n");
            Console.WriteLine("The Wumpus: Asleep until you run into him or shoot an arrow. He is deadly.\nTwo Superbats: Which will teleport you to a random room.\nTwo Bottomless Pits: Which instantly turn you into pudding.\n\nThe goal of the game is to shoot the wumpus without dying.\nAre you ready?");     
 
            String data = Console.ReadLine();
        }


        //Initialize
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        //protected override void Initialize()
        //{
        //    // TODO: Add your initialization logic here

        //    base.Initialize();
        //}


        //Instructions

        //SETUP
        //Creates the map. Only needed once, unless new game
        public void SetRooms()
        {

            map = new Room[4, 5];

            //Initiate rooms so rooms can contain Room objects as neighbors
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    map[x, y] = new Room();
                }
            }

            //Set id and neighbors for each room.
            int id = 1;
            for (int x = 0; x < 4; x++) //X grid. Up and down. 
            {
                for (int y = 0; y < 5; y++) //Y grid. Left and right.
                {
                    try
                    {
                        switch (x) //The algorithm to link rooms depends on the X row
                        {
                            case 0:
                                {
                                    map[x, y].setRoom(map, id.ToString(), map[x, (((y - 1) % 5) + 5) % 5], map[x, (y + 1) % 5], map[x + 1, y]);
                                    break;
                                }
                            case 1:
                                {
                                    map[x, y].setRoom(map, id.ToString(), map[x + 1, y], map[x - 1, y], map[x + 1, (((y - 1) % 5) + 5) % 5]);
                                    break;
                                }
                            case 2:
                                {
                                    map[x, y].setRoom(map, id.ToString(), map[x + 1, y], map[x - 1, y], map[x - 1, (y + 1) % 5]);

                                    break;
                                }
                            case 3:
                                {
                                    map[x, y].setRoom(map, id.ToString(), map[x, (((y - 1) % 5) + 5) % 5], map[x, (y + 1) % 5], map[x - 1, y]);
                                    break;
                                }
                        }
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        Console.WriteLine("Out of range exception" + e);
                    }
                    id++; //Next room ID
                }
            }
        }



        //SETUP
        //Places hazards, Wumpus, and player onto the map. TODO: Set up for Play again? Same Settings or no?
        public void PutInRooms()
        {

            Random rnd = new Random(); //Randomly generate a new room
            int rnd_x = rnd.Next(4);
            int rnd_y = rnd.Next(5);


            //Set up Wumpus
            wumpusDefRoom = map[rnd_x, rnd_y];
            wumpus = new Wumpus();

            wumpusRoom = wumpusDefRoom;
            wumpusRoom.wump = wumpus;


            //Set up two pits
            pits = new Pit();
            int i;
            for (i = 0; i < 2; i++)
            {
                //Set pits 
                do
                {
                    rnd_x = rnd.Next(4);
                    rnd_y = rnd.Next(5);

                    pitDefRoom[i] = map[rnd_x, rnd_y];



                } while (pitDefRoom[i].contains_hazard == true);
                //Loop again if the Pit 1's room contains a hazard, Pit 2's room contains a hazard, or if Pit 1 and 2 are the same room. 

                pitDefRoom[i].hazard = pits;
                pitDefRoom[i].contains_hazard = true; //Set room hazard to true

            }


            //Place bat nests in two rooms. Bats return to their nest after teleporting the player
            bats = new Bats();
            for (i = 0; i < 2; i++)
            {
                //Put bats in room
                do //While the selected room already contains a hazard
                {
                    rnd_x = rnd.Next(4);
                    rnd_y = rnd.Next(5);

                    batsDefRoom[i] = map[rnd_x, rnd_y];

                } while (batsDefRoom[i].contains_hazard == true);
                //Loop again if the bat's room already contains a hazard


                batsDefRoom[i].hazard = bats;
                batsDefRoom[i].contains_hazard = true; //Set room hazard to true

            }


            //Put player in room
            do
            {
                rnd_x = rnd.Next(4);
                rnd_y = rnd.Next(5);

                playerDefRoom = map[rnd_x, rnd_y];
            } while (playerDefRoom.contains_hazard == true);
            //Loop again if the player's room already contains a hazard

            player = new Player(playerDefRoom);
        }



        //Wumpus' turn. Move if awake. Do nothing if asleep. 
        private void WumpusTurn()
        {
            if (wumpus.awake) //If the wumpus is awake
            {
                Random rnd = new Random();
                if (rnd.Next(4) != 3) //move if random generator selects 0, 1, or 2. Do not move if 3 is selected
                {
                    wumpusRoom = wumpusRoom.wumpusMove();
                }
            }

        }


        private void PlayerTurn()
        {
            String choice = "";

            //If the user doesn't input S or M, the console asks again.
            do
            {
                //Display room information
                Console.WriteLine("Looks like I'm in Room " + player.current_room.room_id + "\nTunnels lead to rooms " + player.current_room.get_neighborlist() + "\n[S]HOOT OR [M]OVE?");
                try
                {
                    choice = Console.ReadLine(); //Read choice
                }
                catch (System.IO.IOException e)
                {
                    Console.WriteLine(e);
                }

                choice = choice[0].ToString();
                if (!String.Equals(choice, "S", StringComparison.OrdinalIgnoreCase) && (!String.Equals(choice, "M", StringComparison.OrdinalIgnoreCase)))
                    Console.WriteLine("ERROR! Try again! [S]HOOT OR [M]OVE");
            }
            while (!String.Equals(choice, "S", StringComparison.OrdinalIgnoreCase) && (!String.Equals(choice, "M", StringComparison.OrdinalIgnoreCase)));

            //If S, shoot. If M, move.
            if (String.Equals(choice, "S", StringComparison.OrdinalIgnoreCase))
            {
                player.ShootArrow();
                if (wumpus.awake == false)
                    Console.WriteLine("GREAT JOB! YOU MISSED! NOW THE WUMPUS IS AWAKE! ");
                wumpus.awake = true;
            }
            else
            {
                player.current_room = player.current_room.PlayerMove(); //MOVE TO PLAYER OBJECT

            }

        }


        //HAZARD EFFECTS IF ANY
        public void HazardEffects()
        {
            IHazard evnt = null;

            do
            {
                //If Wumpus is in the room. If awake, else if asleep. 
                if ((player.current_room.wump != null) && (wumpus.awake == true) && (wumpus.alive == true)) //If player walks into the room with the wumpus and it's awake, death.
                    wumpus.act(player);
                else if ((player.current_room.wump != null) && (wumpus.awake == false)) //If wumpus is in the same room and asleep, awaken and move. 
                {
                    Console.WriteLine("It's the wumpus! Now he's awake!\nWhere did he go?");
                    wumpus.awake = true;
                }
                //If there is a hazard in the room
                if (player.current_room.hazard != null)
                {
                    evnt = player.current_room.hazard;
                    player.current_room.hazard.act(player);

                }
            }
            while ((evnt is Bats) && (player.current_room.hazard != null)); //Loop if the room still has a hazard or wumpus AFTER a bat event.
        }


        //Game Reset Pits and Bats stay still. Wumpus room must be cleared. Player room cleared. Alive both. Arrows = 5. 
        private void GameReset(String same_settings)
        {

            if (same_settings.Equals("Y") || same_settings.Equals("y"))
            {
                player.arrows = 5;
                player.alive = true;
                wumpus.alive = true;
                wumpus.awake = false;
                player.current_room = null;
                player.current_room = playerDefRoom;
                wumpusRoom.wump = null;
                wumpusRoom = wumpusDefRoom;
                wumpusRoom.wump = wumpus;
            }
            else
            {
                pits = null;
                bats = null;
                player = null;
                wumpus = null;
                map = null;
                SetRooms();
                PutInRooms();
            }

        }



        //        //LoadContent
        //        /// <summary>
        //        /// LoadContent will be called once per game and is the place to load
        //        /// all of your content.
        //        /// </summary>
        //        protected override void LoadContent()
        //        {

        //            // Create a new SpriteBatch, which can be used to draw textures.
        //            spriteBatch = new SpriteBatch(GraphicsDevice);

        //            // TODO: use this.Content to load your game content here
        //        }



        ////UnloadContent
        //        /// <summary>
        //        /// UnloadContent will be called once per game and is the place to unload
        //        /// all content.
        //        /// </summary>
        //        protected override void UnloadContent()
        //        {
        //            // TODO: Unload any non ContentManager content here
        //        }

        // //Update
        //        /// <summary>
        //        /// Allows the game to run logic such as updating the world,
        //        /// checking for collisions, gathering input, and playing audio.
        //        /// </summary>
        //        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //        protected override void Update(GameTime gameTime)
        //        {
        //            // Allows the game to exit
        //            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        //                this.Exit();

        //            // TODO: Add your update logic here

        //            base.Update(gameTime);
        //        }

        // //Draw
        //        /// <summary>
        //        /// This is called when the game should draw itself.
        //        /// </summary>
        //        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        //        protected override void Draw(GameTime gameTime)
        //        {
        //            GraphicsDevice.Clear(Color.CornflowerBlue);

        //            // TODO: Add your drawing code here

        //            base.Draw(gameTime);
        //        }
        //    }
    }

}
