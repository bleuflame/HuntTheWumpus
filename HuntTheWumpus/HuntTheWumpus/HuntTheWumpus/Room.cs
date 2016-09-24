using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus
{
    public class Room
    {
        private String _room_id;
        private bool _contains_hazard;
        private Room[] neighbor = new Room[3];
        private IHazard _hazard, _wump;
        private Room[,] _map;

        //Getters and setters
        public bool contains_hazard
        {
            get
            {
                return this._contains_hazard;
            }
            set
            {
                this._contains_hazard = value;
            }

        }

        public String room_id
        {
            get
            {
                return this._room_id;
            }
            set
            {
                this._room_id = value;
            }

        }

        public IHazard hazard
        {
            get
            {
                return this._hazard;
            }
            set
            {
                this._hazard = value;
            }

        }

        public IHazard wump
        {
            get
            {
                return this._wump;
            }
            set
            {
                this._wump = value;
            }

        }


        public Room[,] map
        {
            get
            {
                return this._map;
            }
            set
            {
                this._map = value;
            }

        }

        //Constructor
        public Room() { }

        //sets up each room
        public void setRoom(Room[,] map, String id, Room room1, Room room2, Room room3)
        {
            this.map = map;
            room_id = id;
            neighbor[0] = room1;
            neighbor[1] = room2;
            neighbor[2] = room3;
            contains_hazard = false;
        }

        //Pick a room, X, Y, or Z? dialogue
        public String get_neighborlist()
        {
            return " " + (neighbor[0].room_id) + ", " + (neighbor[1].room_id) + ", and " + (neighbor[2].room_id);
        }

        //Is this a connected room? Yes, then go there. Else, go to a random one.
        public Room PlayerMove()
        {
            bool isNeighbor = false;


            do //While user doesn't select a valid neighbor id
            {
                int id;
                String neighbor_id;
                bool success;
                //Loop if the user did not enter a number
                do
                {
                    Console.WriteLine("WHERE TO?");
                    neighbor_id = Console.ReadLine();

                    success = int.TryParse(neighbor_id, out id);
                } while (!success);

                if (neighbor_id.Equals(neighbor[0].room_id))
                {
                    isNeighbor = true;
                    return neighbor[0];
                }
                else if (neighbor_id.Equals(neighbor[1].room_id))
                {
                    isNeighbor = true;
                    return neighbor[1];
                }
                else if (neighbor_id.Equals(neighbor[2].room_id))
                {
                    isNeighbor = true;
                    return neighbor[2];
                }
                Console.WriteLine("NOT POSSIBLE");
            } while (!isNeighbor);

            return neighbor[0]; //Never reached
        }





        //FOR ARROW.
        public Room ArrowMove(String id)
        {
            //If the id matches a neighbor, return that room.
            for (int x = 0; x < 3; x++)
            {
                if (id.Equals(neighbor[x].room_id))
                    return neighbor[x];
            }
            //Else return a random neighbor
            return RandMove();
        }


        //Move to a random room neighbor. FOR ARROW AND WUMPUS. NOT BATS
        public Room RandMove()
        {
            Random rnd = new Random();
            return neighbor[rnd.Next(3)];
        }

        //NOTE: ENSURE WUMPUS DOES NOT OVERRIDE OTHER HAZARDS
        public Room wumpusMove()
        {
            Room randRoom;

            randRoom = RandMove();

            randRoom.wump = this.wump;
            this.wump = null;

            return randRoom;
        }

        //Checks for hazards in neighboring rooms
        public void HazardCheck()
        {
            for (int i = 0; i < 3; i++)
            {
                if (neighbor[i].hazard != null)
                    neighbor[i].hazard.hazard_msg();
                if (neighbor[i].wump != null)
                    neighbor[i].wump.hazard_msg();
            }
        }

        //Returns a random room
        public Room RandomRoom()
        {
            Random rnd = new Random();
            int rnd_x = rnd.Next(4);
            int rnd_y = rnd.Next(5);

            return map[rnd_x, rnd_y];
        }



    }
}

