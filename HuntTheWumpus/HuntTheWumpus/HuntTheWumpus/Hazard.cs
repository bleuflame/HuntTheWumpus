using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuntTheWumpus
{

    public interface IHazard
    {
        void hazard_msg();
        void act(Player player);
        bool alive { get; set; } //Only for wumpus
        bool awake { get; set; } //Only for wumpus
    }



    class Pit : IHazard
    {

        private bool _awake;
        public bool awake
        {
            get
            {
                return false;
            }
            set
            {
                this._awake = value;
            }

        }

        private bool _alive;
        public bool alive
        {
            get
            {
                return false;
            }
            set
            {
                this._alive = value;
            }

        }

        //Constructor
        public Pit()
        {
        }


        public void hazard_msg()
        {
            Console.WriteLine("I feel a draft. Could it be a pit?");
        }


        //Kill player if they fall in a hole
        public void act(Player player)
        {
            Console.WriteLine("WHO DUG THIS HOOOOOOLLLLLLEEEEeeee~ *splat*");
            player.alive = false;

        }
    }


    class Bats : IHazard
    {


        private bool _awake;
        public bool awake
        {
            get
            {
                return false;
            }
            set
            {
                this._awake = value;
            }

        }

        private bool _alive;
        public bool alive
        {
            get
            {
                return false;
            }
            set
            {
                this._alive = value;
            }

        }



        //Constructor
        public Bats() { }

        public void hazard_msg()
        {
            Console.WriteLine("I hear bats nearby");
        }

        //Teleport the player to another room
        public void act(Player player)
        {
            Console.WriteLine("SUPERBATS ARE TAKING ME AWAY!! HOW IS THIS EVEN POSSIBLLLLEEEEEeeee~? *twinkle*");

            player.current_room = player.current_room.RandomRoom(); //Set player's current room to a random room.
        }
    }


    public class Wumpus : IHazard
    {
        private bool _awake;
        public bool awake
        {
            get
            {
                return this._awake;
            }
            set
            {
                this._awake = value;
            }

        }

        private bool _alive;
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
        public Wumpus()
        {
            alive = true;
            awake = false;
        }


        public void hazard_msg()
        {
            Console.WriteLine("I smell a Wumpus!");
        }

        //Set player to dead, if wumpus acts
        public void act(Player player)
        {
            Console.WriteLine("Oof! *dying noises*\nTell my programmer... \n*cough* \nHe should have set... p-player.alive... \nto readonly.");
            player.alive = false;

        }




    }

}



