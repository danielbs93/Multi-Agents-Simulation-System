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
        
        List<Floor> reachableFloors;
        List<Floor> privateFloors;
        
        private int totalCapacity;
        private int currentNumberOfPassengers;
        private int currentCost;
        private string name;
        private int initialPosition;

        private LinkedList<Vector3> freeSpaceVectors;
        private LinkedList<Vector3> occupiedSpaceVectors;
        
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
            InitSpaceVectors();
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
            InitSpaceVectors();
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

        /*
          Changing the postion of the game object in unity towrds the target position
         */
        public void MoveElevator(int direction, float stepY, float simulationSpeed, float elevatorSpeed)
        {
            Vector3 localPosition = elevatorGameObject.GetComponent<Transform>().position;
            elevatorGameObject.GetComponent<Transform>().position = Vector3.MoveTowards(localPosition, localPosition + new Vector3(0, direction * stepY, 0), (simulationSpeed / elevatorSpeed));
        }

        public Transform GetGameObjectTransform()
        {
            return elevatorGameObject.transform;
        }
        /*
         This structures responsible for managing the entering and exsiting passengers into the elevator.
            Assumption: no more than 9 passengers at a single elevator
         */
        private void InitSpaceVectors()
        {
            occupiedSpaceVectors = new LinkedList<Vector3>();
            freeSpaceVectors = new LinkedList<Vector3>();

            // Divinding the elevators into 9 equals spaces
            float x = -0.7f;
            float z = -0.7f;
            for (int i = 0; i < 3; i++)
            {
                x = -0.7f;
                for (int j = 0; j < 3; j++)
                {
                    freeSpaceVectors.AddLast(new Vector3(x, 0, z));
                    x += 0.7f;
                }
                z += 0.7f;
            }
        }

        public Vector3 getNextFreeSpace()
        {
            System.Random rand = new System.Random();
            int randIdx = rand.Next(0, (freeSpaceVectors.Count - 1));
            Vector3 output = freeSpaceVectors.ElementAt(randIdx);
            occupiedSpaceVectors.AddLast(output);
            freeSpaceVectors.Remove(output);
            return output;
        }

        public void freePassengerSpace(Vector3 incitement)
        {
            occupiedSpaceVectors.Remove(incitement);
            freeSpaceVectors.AddLast(incitement);
        }

    }
}
