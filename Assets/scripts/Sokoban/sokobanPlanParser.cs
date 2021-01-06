using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class sokobanPlanParser {

    private List<Action> actions;
    
    public sokobanPlanParser(string path) {
        string[] lines = File.ReadAllLines(path);
        List<string> listOfActions = new List<string>();
        listOfActions.AddRange(lines.ToList());
        this.actions = new List<Action>();

        int linesCounter = 0;
        foreach (string line in lines){
            if (line != "") {
                string description;
                if (linesCounter < 10)
                    description = line.Substring(5, line.Length - 6);
                else
                    description = line.Substring(6, line.Length - 7);
                

                string[] splittedLine = description.Split(' ');
                string agentId="";
                string direction="";
                foreach (string word in splittedLine) {
                    if (word.StartsWith("player")) 
                        agentId = word.Substring(7);
                    if (word.StartsWith("dir"))
                        direction = word.Substring(4);
                }
                Action action = new Action(agentId, description, direction);
                actions.Insert(linesCounter,action);
                // actions.Add(action);
                linesCounter++;
            }
        }
    }

    public List<Action> getActions(){
        return new List<Action>(actions);
    }
}
