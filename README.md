# Multi-Agent-Simulation-System

## Project Goal

The project's goal is to make it easier for programmers/observers/analysts drawing conclusions and decision-making when they want to run machine learning algorithms
that use intelligent agents.
Prof .Guy Shani has learning algorithms that operate intelligent agents and his desire to develop a simulation that will help him understand the running processes of the
agents in maximum comfort.
The simulation is a visual tool to which the series of action performed by the agents are loaded, through which the programmer/observer/analyst will be able to make
decisions more easily that is not possible from reading the output alone.
The simulation we built in this project was done using Unity platform ,which provides an interface and toolbox that enables to create simulations that can be used to simulate multi-agent design algorithms.
The simulation we built contains two different environments :
1. Sokoban: video game where the player's goal is to push objects to different
destinations using a minimum number of movements .The objects are boxes,
and the agents are the characters who push them towards the target .
2. Multi Agent Elevator Movement Simulation System :here, the agents are the
elevators, and the objects are people scattered on the different floors and want
to reach the different destinations.
The simulation is not a necessary tool for understanding the processes that occur during the running of the algorithm ,but it is an essential tool that optimizes the times of reasoning ,the quality of the conclusions, and the decision-making processes that the programmer/observer/ analyst must perform.

## Introduction

Classical planning problems are an industry in the field of AI that involves the implementation of strategies or sequences of actions. 
Execution is usually done using smart agents, autonomous robots, or unmanned vehicles. Solutions to planning problems will often be complex, and in order to discover and analyze them, it is necessary to use multidimensional space.
While planning is commonly referred to as a single-agent mission, multi-agent planning (MAP) incorporates this concept by employing multiple non-self-interested intelligent agents working collaboratively to develop a distributed problem-solving action, rather than the classic single-agent planning paradigm.

There is difficulty in understanding the initial problem definitions, tracking agent’s movements, detection of algorithm running problems and the most important, there is a major difficulty in concluding if the agent’s goals and the targets of the planning algorithm have been achieved or if the algorithm produced an optimal solution. 
As part of the algorithm solution, choices, actions, and calculations are made that are not always understandable and easy to track for the programmer.
For each planning domain problem there are at least 2 files: the first, describes the initial state of the problem and the second is the pre-conditions and effects of an agent, and each agent has these 2 files of his own.

Our solution – Multi-AI agents simulation system, we will create a graphical interface system that will illustrate how AI agents work in a multi-agent planning problem and the system will demonstrate the algorithm stages and will be able to produce analysis in a simple and clear way.
Since every planning domain has its own problem definitions, actions, effects, and problems it is impossible to generate a generic simulation for all possible domains so our client decided we will focus on 2 main domains and produce an adaptive extension able modular simulation.
The first domain is Sokoban - This is a video game where the player's goal is to push all the objects in the maze to different targets. 

The second domain is Elevator simulation system, in this environment the agents are the elevators and the objects are people scattered on a different floors and their desire is to reach different destinations. 
The Elevators domain has multiple view angles for the viewer convenience in order to understand the program running. 
Each floor has a gathering space - the grass area which each person reaches to it if it’s his destination floor. 
Above each elevator there are some information like its name, capacity, and the cost of each movement it has performed.
For each person there is information describing him like his name and his destination floor.

Planning algorithm developers often use visual tools to illustrate how the solution runs. Our simulation is an effective visual tool that contributes to the understanding of the experts during the process of drawing conclusions and making decisions from the familiar textual output that is obtained in multi-agent design algorithms.
There is time saving when decoding the output, there is the ability to identify whether the algorithm has indeed achieved its goal and verify it, there is the ability to verify and compare different runs of the algorithm - which of the runs achieved better results, there is the ability to identify whether agents cooperated to achieve the goal or Sub-goals on the way to achieving the goal.

## Implementation
We have used Unity platform along with C# scripts to build our system. 
