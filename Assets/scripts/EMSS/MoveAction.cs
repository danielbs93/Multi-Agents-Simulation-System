using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testElevator {
    class MoveAction : Action {

        private int direction;
        private int targetFloor;
        private int deparuredFloor;

        public MoveAction(string executorName, int direction, int startFloor, int endFloor) : base(executorName) {
            this.direction = direction;
            TargetFloor = endFloor;
            deparuredFloor = startFloor;
        }

        public MoveAction(MoveAction ma) : base(ma.ExecutorName)
        {
            this.direction = ma.Direction;
            TargetFloor = ma.TargetFloor;
            deparuredFloor = ma.DeparuredFloor;
        }

        public int Direction {
            set { direction = value; }
            get { return this.direction; }
        }

        public int TargetFloor { get => targetFloor; set => targetFloor = value; }
        public int DeparuredFloor { get => deparuredFloor; set => deparuredFloor = value; }
    }
}
