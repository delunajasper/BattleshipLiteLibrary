using BattleshipLiteLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleshipLiteLibrary
{
    //static no need to store data, logic methods only for reusability
    public static class GameLogic
    {
        //Objects instantiated / class instances, dont pass copy just the address
        //passing in model
        //initialise grid using parameters
        public static void InitializeGrid(PlayerInfoModel model)
        {
            List<string> letters = new List<string>
            {
                "A",
                "B",
                "C",
                "D",
                "E"
            };

            List<int> numbers = new List<int>
            {
                1,
                2,
                3,
                4,
                5
            };

            foreach (string letter in letters)
            {
                foreach(int number in numbers)
                {
                    AddGridSpot(model, letter, number);
                }
            }
        }

        public static bool PlayerStillActive(PlayerInfoModel player)
        {
            //player still active
            bool isActive = false;

            foreach (var ship in player.ShipLocations)
            {
                //check for one that is not sunk, means player is still active
                if(ship.Status != GridSpotStatus.Sunk)
                {
                    isActive = true;
                }
            }

            return isActive;
        }

        private static void AddGridSpot(PlayerInfoModel model, string letter, int number)
        {
            //gona take model and add gridspot
            GridSpotModel spot = new GridSpotModel
            {
                SpotLetter = letter,
                SpotNumber = number,
               Status = GridSpotStatus.Empty
            };

            model.ShotGrid.Add(spot);
        }

        public static bool PlaceShip(PlayerInfoModel model, string location)
        {
            bool output = false;
            //split location into row and column
            (string row, int column) = SplitShotIntoRowAndColumn(location);

            //determine if on the grid
            //dtrmine if something is already there
            bool isValidLocation = ValidateGridLocation(model, row, column);
            bool isSpotOpen = ValidateShipLocation(model, row, column);
            //validate 

            //it will either place a ship or do nothing
          if(isValidLocation && isSpotOpen)
            {
                model.ShipLocations.Add(new GridSpotModel
                {
                    SpotLetter = row,
                    SpotNumber = column,
                    Status = GridSpotStatus.Ship
                });

                //if valid location and open space
                output = true;
            }

            return output;
        }

        //look at ships already been placed. 
        private static bool ValidateShipLocation(PlayerInfoModel model, string row, int column)
        {
            bool isValidaLocation = true;

            foreach (var ship in model.ShipLocations)
            {
                if(ship.SpotLetter == row.ToUpper() && ship.SpotNumber == column)
                {
                    isValidaLocation = false;
                }
            }
            return isValidaLocation;
        }

        //if we found a match this ship is on the grid. i.e match on e4
        private static bool ValidateGridLocation(PlayerInfoModel model, string row, int column)
        {
            bool isValidaLocation = false;

            foreach (var ship in model.ShotGrid)
            {
                if (ship.SpotLetter == row.ToUpper() && ship.SpotNumber == column)
                {
                    isValidaLocation = true;
                }
            }
            return isValidaLocation;
        }

        public static int GetShotCount(PlayerInfoModel player)
        {
            int shotCount = 0;
            //if taken a shot or empty
            foreach (var shot in player.ShotGrid)
            {
                if(shot.Status != GridSpotStatus.Empty)
                {
                    shotCount += 1;
                }
            }

            return shotCount;
        }

        //split into 2 different values
        public static (string row, int column) SplitShotIntoRowAndColumn(string shot)
        {
            string row = "";
            int column = 0;

            if(shot.Length != 2)
            {
                throw new ArgumentException("This was an invalid shot type");
            }

            char[] shotArray = shot.ToArray();

            //converts char to string, if empty string this blows up.
            //error checking
            row = shotArray[0].ToString();

            //get string value by parsing int
            column = int.Parse(shotArray[1].ToString());

            //return tuple
            return (row, column);
        }

        //check every spot in the shot grid until it match row and col
        public static bool ValidateShot(PlayerInfoModel player, string row, int column)
        {
            bool isValidShot = false;

            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if(gridSpot.Status == GridSpotStatus.Empty)
                    {
                        isValidShot = true;
                    }
                }
            }

            return isValidShot;
            
        }

        //check opponent if there's a ship - similar to validateshiplocation
        public static bool IdentifyShotResults(PlayerInfoModel opponent, string row, int column)
        {
            bool isAHit = false;

            foreach (var ship in opponent.ShipLocations)
            {
                if (ship.SpotLetter == row.ToUpper() && ship.SpotNumber == column)
                {
                    isAHit = true;
                    ship.Status = GridSpotStatus.Sunk;
                }
            }
            return isAHit;
        }

        /// <summary>
        /// looping through find spot on the grid that matches row and col
        /// mark as hit or miss
        /// </summary>
        /// <param name="player"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="isAHit"></param>
        public static void MarkShotResult(PlayerInfoModel player, string row, int column, bool isAHit)
        {

            foreach (var gridSpot in player.ShotGrid)
            {
                if (gridSpot.SpotLetter == row.ToUpper() && gridSpot.SpotNumber == column)
                {
                    if (isAHit)
                    {
                        gridSpot.Status = GridSpotStatus.Hit;
                    }
                    else
                    {
                        gridSpot.Status = GridSpotStatus.Miss;
                    }
                }
            }
        }
    }
}
