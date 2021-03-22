using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testElevator {
    class MoveAction : Action {

        private int direction;

        public MoveAction(string executorName, int direction) : base(executorName) {
            this.direction = direction;
        }

        public int Direction {
            set { direction = value; }
            get { return this.direction; }
        }
    }
}
