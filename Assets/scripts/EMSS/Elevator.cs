using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace testElevator
{
    enum Type
    {
        slow,
        fast,
    }
    class Elevator
    {
        private Type type;
        private int totalCapacity;
        List<Floor> reachableFloors;
        List<Floor> privateFloors;
        private int currentNumberOfPassengers;
        private int currentCost;
        private string name;
        private int initialPosition;
        private GameObject elevatorGameObject;


        public Elevator(Type type, int totalCapacity, List<Floor> reachableFloors, List<Floor> privateFloors,
            int currentCapacity, int currentCost, string name)
        {
            this.type = type;
            this.totalCapacity = totalCapacity;
            this.reachableFloors = reachableFloors;
            this.privateFloors = privateFloors;
            this.currentNumberOfPassengers = currentCapacity;
            this.currentCost = currentCost;
            this.name = name;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Elevator(Type type, string name)
        {
            this.type = type;
            this.name = name;
            this.privateFloors = new List<Floor>();
            this.reachableFloors = new List<Floor>();
        }

        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        public int InitialPosition
        {
            get { return initialPosition; }
            set { initialPosition = value; }
        }

        public int TotalCapacity
        {
            get { return totalCapacity; }
            set { totalCapacity = value; }
        }

        public List<Floor> ReachableFloors
        {
            get { return reachableFloors; }
            set { reachableFloors = value; }
        }

        public List<Floor> PrivateFloors
        {
            get { return privateFloors; }
            set { privateFloors = value; }
        }

        public int CurrentNumberOfPassengers
        {
            get { return currentNumberOfPassengers; }
            set { currentNumberOfPassengers = value; }
        }

        public int CurrentCost
        {
            get { return currentCost; }
            set { currentCost = value; }
        }

        public GameObject ElevatorGameObject { get => elevatorGameObject; set => elevatorGameObject = value; }
    }
}
