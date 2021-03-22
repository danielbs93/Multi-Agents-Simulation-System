using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testElevator
{
    class PassengerAction : Action {

        private Boolean actionType; // 1-Board, 0- Leave
        private string elevatorName;
        private int floorNumber;
        private int finalCapacity;

        public PassengerAction(string executorName, bool actionType, string elevatorName,
            int floorNumber, int finalCapacity) : base(executorName) {

            this.actionType = actionType;
            this.elevatorName = elevatorName;
            this.floorNumber = floorNumber;
            this.finalCapacity = finalCapacity;
        }

        public Boolean ActionType {
            set { actionType = value; }
            get { return this.actionType; }
        }

        public string ElevatorName{
            set { elevatorName = value; }
            get { return this.elevatorName; }
        }

        public int FloorNumber{
            set { floorNumber = value; }
            get { return this.floorNumber; }
        }

        public int FinalCapacity {
            set { finalCapacity = value; }
            get { return this.finalCapacity; }
        }
    }
}
