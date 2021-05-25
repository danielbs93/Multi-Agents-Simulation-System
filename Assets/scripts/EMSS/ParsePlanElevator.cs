using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testElevator {
    class ParsePlanElevator {

        private List<Action> actions;

        public ParsePlanElevator(string path) {
            string[] lines = File.ReadAllLines(path);
            this.Actions = new List<Action>();

            int linesCounter = 0;

            foreach(string line in lines) {
                if (!line.Equals("")) {
                    string[] splittedLine = line.Split(' ');
                    if (splittedLine[2].Contains("(move-")) { // elevator action
                        string executorName;
                        if (splittedLine[3].Contains("slow"))
                            executorName = splittedLine[3].Substring(0, splittedLine[3].Length - 2);
                        else // fast
                            executorName = splittedLine[3];

                        string endFloor = splittedLine[5].Substring(1, splittedLine[5].Length - 2);
                        string startFloor = splittedLine[4].Substring(1);

                        int targetFloor = Int32.Parse(endFloor);
                        int departureFloor = Int32.Parse(startFloor);

                        int direction = targetFloor - departureFloor;
                        MoveAction moveAction = new MoveAction(executorName, direction, departureFloor, targetFloor);
                        Actions.Insert(linesCounter, moveAction);
                        linesCounter++;
                    }

                    else { // Passenger action
                        Boolean isBoard = false;
                        if (splittedLine[2].Contains("(board")) 
                            isBoard = true;

                        string elevatorName;
                        if (splittedLine[3].Contains("slow"))
                            elevatorName = splittedLine[3].Substring(0, splittedLine[3].Length - 2);
                        else // fast
                            elevatorName = splittedLine[3];

                        string passengerName = splittedLine[4];
                        int floorNumber = Int32.Parse(splittedLine[5].Substring(1));
                        int finalCapacity = Int32.Parse(splittedLine[7].Substring(1, splittedLine[7].Length-2));

                        PassengerAction passengerAction = new PassengerAction(passengerName, isBoard,
                            elevatorName, floorNumber, finalCapacity);
                        Actions.Insert(linesCounter, passengerAction);

                        linesCounter++;
                        
                    }
                }
            }
        }

        internal List<Action> Actions { get => actions; set => actions = value; }
    }
}
