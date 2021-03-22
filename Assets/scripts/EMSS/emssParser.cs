using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testElevator
{
    class emssParser {

        private int slowElevators;
        private int fastElevators;
        private int numOfFloors;
        private int numOfPassengers;

        private Dictionary<string, int> costs = new Dictionary<string, int>();
        private Dictionary<string, Passenger> passengers = new Dictionary<string, Passenger>();
        private List<Elevator> elevators = new List<Elevator>();
        private List<Floor> floors = new List<Floor>();

        public List< Passenger> getPassengers { get => passengers.Values.ToList<Passenger>();}
        public int SlowElevators { get => slowElevators; set => slowElevators = value; }
        public int FastElevators { get => fastElevators; set => fastElevators = value; }
        public int NumOfFloors { get => numOfFloors; set => numOfFloors = value; }
        public int NumOfPassengers { get => numOfPassengers; set => numOfPassengers = value; }
        public Dictionary<string, int> Costs { get => costs; set => costs = value; }
        internal Dictionary<string, Passenger> Passengers { get => passengers; set => passengers = value; }
        internal List<Elevator> Elevators { get => elevators; set => elevators = value; }
        internal List<Floor> Floors { get => floors; set => floors = value; }

        public emssParser(string path) {
            List<string> paths = Directory.EnumerateFiles(path, "*.pddl").ToList();
            SlowElevators = paths.Count / 4;
            FastElevators = paths.Count / 4;
            Boolean first = true;
            foreach (string filePath in paths) {
                if (filePath.Contains("problem")){
                    Console.WriteLine(filePath);

                    List<string> list = new List<string>();
                    string[] lines = File.ReadAllLines(filePath);
                    list.AddRange(lines.ToList());
                    if (first) {
                        getInformationalData(list, first);
                        first = false;
                    }
                    else
                        getInformationalData(list, first);
                }
            }

            Console.WriteLine("hayush");


        }

        private void getInformationalData(List<string> list, Boolean first) {

            int counter = 2;
            Boolean hasPrivateFloor = false;

            if (first) {
                while (list[counter].Contains("passenger")) {
                    NumOfPassengers++;
                    counter++;
                }
                while (list[counter].Contains("count"))
                {
                    NumOfFloors++;
                    string[] line = list[counter].Split('-');
                    string floorNumber = line[0].Trim().Substring(1);
                    Floor floor = new Floor(Int32.Parse(floorNumber));
                    if (!Floors.Contains(floor))
                        this.Floors.Add(floor);
                    counter++;
                }
            }

            else
                counter = counter + NumOfPassengers + NumOfFloors;
           
            Elevator elevator = null;
            while (!list[counter].Contains(")")) {
                if (list[counter].Contains("elevator")) {
                    string [] type = list[counter].Split('-');
                    if (type[0].Trim().Substring(0, 4).Equals("slow")) {
                        elevator = new Elevator(Type.slow,type[0].Trim());
                       
                    }
                    else {
                        elevator = new Elevator(Type.fast, type[0].Trim());
                    }
                }
                if (list[counter].Contains("count")) {
                    string[] privateElevatorArray = list[counter].Split('-');
                    string floorNumber = privateElevatorArray[0].Trim().Substring(1);
                    Floor floor = new Floor(Int32.Parse(floorNumber));
                    elevator.PrivateFloors.Add(floor);
                    hasPrivateFloor = true;
                }
                counter++;
            }

            while (!list[counter].Contains("lift-at")){
                counter++;
            }

            string[] tempArray = list[counter].Split(' ');
            string position = tempArray[tempArray.Length - 1].Substring(1,tempArray[tempArray.Length-1].Length-2);
            elevator.InitialPosition = Int32.Parse(position);
            counter++;

            tempArray = list[counter].Split(' ');
            string numOfPass = tempArray[tempArray.Length - 1].Substring(1, tempArray[tempArray.Length - 1].Length - 2);
            elevator.CurrentNumberOfPassengers = Int32.Parse(numOfPass);
            counter++;

            int capacity = 0;
            while (list[counter].Contains("can-hold")) {
                capacity++;
                counter++;
            }

            elevator.TotalCapacity = capacity;

            while (list[counter].Contains("reachable-floor")) {
                string[] line = list[counter].Split(' ');
                string floorNumber = line[line.Length - 1].Substring(1, line[line.Length - 1].Length - 2);
                Floor floor = new Floor(Int32.Parse(floorNumber));
                elevator.ReachableFloors.Add(floor);
                counter++;
            }

            while (list[counter].Contains("passenger-at")) {
                string[] line = list[counter].Split(' ');
                string floorNumber = line[line.Length - 1].Substring(1, line[line.Length - 1].Length - 2);
                string passengerNumber = line[line.Length - 2].Substring(1);
                Floor floor = new Floor(Int32.Parse(floorNumber));
                Passenger passenger = new Passenger(passengerNumber, floor);
                if (!Passengers.ContainsKey(passengerNumber))
                    this.Passengers.Add(passengerNumber, passenger);
                counter++;
            }
            
            if (first || hasPrivateFloor)
                counter = insertCosts(list, counter);
            
            while(!list[counter].Contains("(passenger-at")) {
                counter++;
            }

            while (list[counter].Contains("(passenger-at")) {
                string[] splittedArray = list[counter].Split(' ');
                string passengerNumber = splittedArray[1].Substring(1);
                int destination = Int32.Parse(splittedArray[2].Substring(1, splittedArray[2].Length - 2));
                Floor destinationFloor = new Floor(destination);
                Passengers[passengerNumber].Destination = destinationFloor;
                counter++;
            }
           
            this.Elevators.Add(elevator);
            
        }

        private int insertCosts(List<string> list, int counter) {
            while (list[counter].Contains("(=") && !list[counter].Contains("(total-cost)")) {
               
                string[] splittedArray = list[counter].Split(' ');
                string fromTo = splittedArray[2].Substring(1) + splittedArray[3].Substring(1,splittedArray[3].Length-2);
                
                int cost = Int32.Parse(splittedArray[4].Substring(0,splittedArray[4].Length - 1));
                
                if (!Costs.ContainsKey(fromTo))
                    Costs.Add(fromTo, cost);
                counter++;
            }
            return counter;
        }

        public int getFloorCost(string floorNo)
        {
            if (costs.ContainsKey(floorNo)) {
                return costs[floorNo];
            }
            return -1;
        }

    }
}
