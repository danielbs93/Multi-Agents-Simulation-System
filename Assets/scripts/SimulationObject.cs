using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationObject 
{
    //private fields
    private string name;
    private int id;

    public SimulationObject(string name, int id)
    {
        this.name = name;
        this.id = id;
    }

    public int getID()
    {
        return this.id;
    }

    public string getName()
    {
        return this.name;
    }

    public void setID(int id)
    {
        this.id = id;
    }

    public void setName(string name)
    {
        if (name != null)
            this.name = name;
    }

   
}
