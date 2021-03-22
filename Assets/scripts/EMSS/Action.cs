using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testElevator{
    class Action {

        private string executorName;

        public Action(string executorName) {
            this.executorName = executorName;
        }

        public string ExecutorName {
            set { executorName = value;  }
            get { return this.executorName; }
        }
    }
}
