using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace testElevator
{
    class Passenger
    {
        private string name;
        private Floor destination; // end position
        private Floor departured; // start position
        private GameObject passengerGameObject;
        private float[] position = { -1, -1, -1 };

        public Passenger(string name, Floor departured)
        {
            this.name = name;
            this.departured = departured;
        }
       

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Floor Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public Floor Departured
        {
            get { return departured; }
            set { departured = value; }
        }

        public GameObject PassengerGameObject { get => passengerGameObject; set => passengerGameObject = value; }
        public float[] Position { get => position; set => position = value; }
    }
}

