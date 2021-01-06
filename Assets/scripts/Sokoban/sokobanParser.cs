using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class sokobanParser {

    public Dictionary<string, object> information; // dimensions, movements, #goals,#agents,..
    public string[,] domain;
    private int playerCounter = 1;
    private int stonesCounter = 1;

    public sokobanParser(string path) {
            information = new Dictionary<string, object>();
            List<string> paths = Directory.EnumerateFiles(path, "*.pddl").ToList();
            // insert #players into the informative dictionary
            information.Add("numberOfPlayers", paths.Count / 2);
            

            Boolean firstFile = true;
            foreach (string filePath in paths) {
                if (filePath.Contains("problem-player")) {
                    List<string> list = new List<string>();
                    string[] lines = File.ReadAllLines(filePath);
                    list.AddRange(lines.ToList());
                    if (firstFile)
                    {
                        getInformationalData(list);
                        firstFile = false;
                    }
                    else
                        insertPlayers(list);
                } 
            }
            addWalls();
            clearEmptySpaces();

        }

        private void getInformationalData(List<string> problemPlayer) {
            int counter = 0;
            Boolean stonesFounded = false;

            // insert dimensions into the informative dictionary
            foreach (string s in problemPlayer){
                if (!s.Contains("stone")) {
                    counter++;
                }
                else {
                    string temp = problemPlayer[counter - 1].Split(' ')[0];
                    string[] dimensions = temp.Substring(5).Split('-');
                    information.Add("rows", dimensions[0]);
                    information.Add("columns", dimensions[1]);

                    //initialize domain dimensions
                    domain = new string[Int32.Parse(dimensions[0]),Int32.Parse(dimensions[1])];
                    stonesFounded = true;
                }
                if (stonesFounded)
                    break;
            }

            // insert #stones into the informative dictionary
            int numOfStones = 0;
            while (!(problemPlayer[counter].Equals(""))){
                numOfStones++;
                counter++;
            }
            information.Add("numOfStones", numOfStones);

            while (!(problemPlayer[counter].Contains("IS-GOAL"))) { 
                counter++;
            }
            int numOfGoals = 0;
            information.Add("isGoalLine", counter);
            

            while (!(problemPlayer[counter].Contains("at player"))) {
                counter++;
            }
            string temp2 = problemPlayer[counter].Substring(19, problemPlayer[counter].Length - 20);
            string[] playerPositions = temp2.Split('-');

        // insert player position into the domain 2D array
            string heroNumber = problemPlayer[counter].Substring(12, 2);
            domain[Int32.Parse(playerPositions[0]) - 1, Int32.Parse(playerPositions[1]) - 1] = "h"+ heroNumber;
            counter++;

            while (problemPlayer[counter].Contains("at stone")) {
                string temp3 = problemPlayer[counter].Substring(18, problemPlayer[counter].Length - 19);
                //insert stone position into the domain 2D array
                string[] stonePositions = temp3.Split('-');
                if (stonesCounter < 10){
                    domain[Int32.Parse(stonePositions[0]) - 1, Int32.Parse(stonePositions[1]) - 1] = "b0" + stonesCounter;
                }
                else
                    domain[Int32.Parse(stonePositions[0]) - 1, Int32.Parse(stonePositions[1]) - 1] = "b" + stonesCounter;
                counter++;
                stonesCounter++;
            }

            while (!problemPlayer[counter].Contains("clear pos"))
                counter++;

            while (problemPlayer[counter].Contains("clear pos"))
            {
                string temp4 = problemPlayer[counter].Substring(12, problemPlayer[counter].Length - 13);
                //insert clear position into the domain 2D array
                string[] clearPositions = temp4.Split('-');
                if (domain[Int32.Parse(clearPositions[0]) - 1, Int32.Parse(clearPositions[1]) - 1] == null)
                {
                    domain[Int32.Parse(clearPositions[0]) - 1, Int32.Parse(clearPositions[1]) - 1] = "c";
                }

                counter++;
            }

            int currCounter = Convert.ToInt32(information["isGoalLine"]);
            while (problemPlayer[currCounter].Contains("IS-GOAL"))
            {
                // insert goal positions into the domain 2D array
                string temp1 = problemPlayer[currCounter].Substring(14, problemPlayer[currCounter].Length - 15);
                string[] goalPositions = temp1.Split('-');
                if (domain[Int32.Parse(goalPositions[0]) - 1, Int32.Parse(goalPositions[1]) - 1] == null 
                || !domain[Int32.Parse(goalPositions[0]) - 1, Int32.Parse(goalPositions[1]) - 1].StartsWith("b"))
                {
                    domain[Int32.Parse(goalPositions[0]) - 1, Int32.Parse(goalPositions[1]) - 1] = "g";
                }
                numOfGoals++;
                currCounter++;
            }
            // insert #goals into the informative dictionary
            information.Add("numOfGoals", numOfGoals);
        }

        private void insertPlayers(List<string> list) {
            foreach (string line in list){
                if (line.Contains("at player")) {
                    string temp = line.Substring(19, line.Length - 20);
                    string[] playerPositions = temp.Split('-');

                // insert player position into the domain 2D array
                string heroNumber = line.Substring(12, 2);
                domain[Int32.Parse(playerPositions[0]) - 1, Int32.Parse(playerPositions[1]) - 1] = "h" + heroNumber;

                }
            }
        }

        private void addWalls() {
        int wallsCounter = 0;
            for ( int i = 0; i < domain.GetLength(0); i++ ){
                for ( int j = 0; j < domain.GetLength(1); j++ ){
                    if (domain[i,j] == null) {
                        domain[i, j] = "w";
                        wallsCounter++;
                    }
                }
            }
        information.Add("numOfWalls", wallsCounter + (4+2*domain.GetLength(0)+2* domain.GetLength(1)));
        }

        private void clearEmptySpaces() {
        for (int i = 0; i < domain.GetLength(0); i++){
            for (int j = 0; j < domain.GetLength(1); j++) {
                if (domain[i, j] == "c") {
                    domain[i, j] = " ";
                }
            }
        }

    }
        public string[,] getDomain()
        {
            return this.domain;
        }

        public Dictionary<string, object> getInformation()
        {
            return this.information;
        }

        public int getRows(){
            return Convert.ToInt32(information["rows"]);
        }
    
        public int getColumns()
        {
            return Convert.ToInt32(information["columns"]);
        }

        public string getCharAt(int row, int col) {
            return domain[row, col];
        }

    public SimulationObject[,,] initializeMap()
    {
        SimulationObject[,,] domain = new SimulationObject[3, this.getRows() + 2, this.getColumns() + 2];
        string[,] realDomain = this.getDomain();

        //creating floor 
        for (int i = 0; i < domain.GetLength(1); i++)
        {
            for (int j = 0; j < domain.GetLength(2); j++)
            {
                // Simulation Object Initiate - inserting wall as name

                domain[0, i, j] = new SimulationObject("f",0); // assign the simulation objects
            }
        }

        //creating domain
        for (int i = 0; i < this.getRows(); i++)
        {
            for (int j = 0; j < this.getColumns(); j++) {
                if (this.getCharAt(i, j).StartsWith("h") || this.getCharAt(i, j).StartsWith("b")) {
                    domain[1, i + 1, j + 1] = new SimulationObject(this.getCharAt(i, j).Substring(0,1), Convert.ToInt32(this.getCharAt(i, j).Substring(1, 1)));
                }
                else
                {
                    domain[1, i + 1, j + 1] = new SimulationObject(this.getCharAt(i, j), 0);
                }
            }
        }

        //creating frame
        for (int i = 0; i < domain.GetLength(2); i++)
        {
            domain[1, 0, i] = new SimulationObject("w", 0);
            domain[1, this.getRows() + 1, i] = new SimulationObject("w", 0);
        }

        for (int i = 0; i < domain.GetLength(1); i++)
        {
            domain[1, i, 0] = new SimulationObject("w", 0);
            domain[1, i, this.getColumns() + 1] = new SimulationObject("w", 0);
        }
        return domain;
    }

    public SimulationObject[,,] generateMap()
    {
        return new SimulationObject[3,20,20];
    }
}