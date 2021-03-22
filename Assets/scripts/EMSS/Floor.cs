using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace testElevator
{
    class Floor
    {
        private int number;

        public Floor(int number)
        {
            this.number = number;
        }

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        public override bool Equals(System.Object obj) {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType())){
                return false;
            }
            else {
                Floor floor = (Floor)obj;
                return this.number == floor.number;
            }
        }

    }
}

