using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus
{
    public class Player
    {
        private Room _current_room;
        private int _arrows;
        private bool _alive;

        //Getters and setters
        public Room current_room
        {
            get
            {
                return this._current_room;
            }
            set
            {
                this._current_room = value;
            }

        }

        public int arrows
        {
            get
            {
                return this._arrows;
            }
            set
            {
                this._arrows = value;
            }

        }

        public bool alive
        {
            get
            {
                return this._alive;
            }
            set
            {
                this._alive = value;
            }

        }

        //Constructor
        public Player() { }

        //Constructor with current room and default settings.
        public Player(Room room)
        {
            current_room = room;
            arrows = 5;
            alive = true;

        }

        public void ShootArrow()
        {

            //If the user has arrows
            if (arrows > 0)
            {

                //HOW MANY ROOMS?
                bool success = false;  //True if user inputs an integer
                int amount; //Amount of rooms selected
                String value;

                int id;
                String room_id;

                Room arrowLocation = current_room;
                String[] path;

                do //While user inputs a non-integer or an out of range (1-5) integer. 
                {
                    Console.WriteLine("YOU HAVE " + arrows + " ARROWS. \nHOW MANY ROOMS SHOULD THE ARROW TRAVEL? (1-5)");
                    value = Console.ReadLine();

                    success = int.TryParse(value, out amount);
                } while ((!success) || (amount > 5) || (amount < 0));


                path = new String[amount + 1]; //+1 so we can add the current room
                path[0] = current_room.room_id; //Place current room in the path

                for (int i = 0; i < amount; i++) //For each room
                {
                    do //While the user inputs an invalid room (Not 1-20) or if they input a wrong selection (A-B-A)
                    {
                        Console.WriteLine("SELECT A ROOM ");
                        room_id = Console.ReadLine();

                        success = int.TryParse(value, out id);  //False if not an integer

                        if (i > 1) //If iterated once, check for A-B-A
                        {
                          //  Console.WriteLine(room_id + " AND " + path[i-2]);
                            if (room_id.Equals(path[i - 1])) //False if A-B-A 
                            {
                                Console.WriteLine("ARROWS AREN'T THAT CURVY! PICK ANOTHER ROOM ");
                                success = false;
                            }
                        }
                        if (!success)
                            Console.WriteLine("WRONG INPUT");
                    } while ((!success) || (id > 20) || (id < 0));



                    arrowLocation = arrowLocation.ArrowMove(room_id);
                    path[i + 1] = arrowLocation.room_id; //Place id in the path

                    if (arrowLocation.wump != null)
                    {
                        Console.WriteLine("YOU SHOT THE WUMPUS! TONIGHT WE FEAST!");
                        arrowLocation.wump.alive = false;
                    }
                    if (arrowLocation == current_room)
                    {
                        alive = false;
                        Console.WriteLine("YOU SHOT YOURSELF? RIP");
                    }
                }

                arrows--; //Subtract an arrow

            }
            else
                Console.WriteLine("I'm out of arrows!"); //Lose?

        }

    }
}
