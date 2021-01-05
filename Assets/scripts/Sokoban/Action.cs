using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action{

    private string agentId;
    private string description;
    private string direction;

    public Action(string id, string desc, string dir)
    {
        this.agentId = id;
        this.description = desc;
        this.direction = dir;
    }

    public string getId()
    {
        return this.agentId;
    }
    public string getDescription()
    {
        return this.description;
    }

    public string getDirection()
    {
        return this.direction;
    }

    public void setId(string id)
    {
        this.agentId = id;
    }

    public void setDescription(string desc)
    {
        this.description = desc;
    }

    public void setDirection(string dir)
    {
        this.direction = dir;
    }
}
