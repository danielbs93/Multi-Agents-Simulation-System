using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SokobanManager : MonoBehaviour
{

    //Objects
    public Canvas canvas;
    public Canvas canvasmenu;
    public Canvas endingScreen;

    public GameObject simulationManager;

    public GameObject maphold;

    public GameObject agent;
    public GameObject wall;
    public GameObject box;
    public GameObject goal;
    public GameObject floor;

    public GameObject[] agents;
    public GameObject[] walls;
    public GameObject[] boxes;
    public GameObject[] goals;
    public GameObject[] floors;

    //Fields
    private sokobanParser sokoban_Parser;// = new sokobanParser("C:\\Users\\alona\\Desktop\\Studies\\p05");
    public SimulationObject[,,] map;

    private bool controlEnabled = false; // Face Camera option was clicked ('c' in keyboard)
    private bool keyboardPlayer = true; // Prototype option - Agent moves currently with the 

    private List<KeyValuePair<int, string>> actions; // List of agents actions from the plan file 

    private string initPath;
    private string preEffPath;
    private string planPath;

    int blockSize = 1;  // size of each block

    // Use this for initialization
    void Start()
    {

    }

    // Menu option for returning to the main page
    public void backmenu()
    {
        canvas.gameObject.SetActive(true);
        canvasmenu.gameObject.SetActive(false);
        //cong.gameObject.SetActive(false);
    }

    public void setSokobanPaths(string init, string preEff, string plan)
    {
        initPath = init;
        preEffPath = preEff;
        planPath = plan;
    }

    public void initParser()
    {
        sokoban_Parser = new sokobanParser(Path.GetDirectoryName(initPath));
    }


    // Initiate Game Objects at their positions
    public void setmap()
    {
        maphold.SetActive(true);

        //------GenerateMap - 3D by the parser with SimulationObject-------
        //map = sokoban_Parser.generateMap();

        //------generating map of char array by the parser---------
        SimulationObject[,,] char_map = sokoban_Parser.initializeMap();

        int countbox = Convert.ToInt32(sokoban_Parser.information["numOfStones"]); // # of boxes / goals
        int countwall = Convert.ToInt32(sokoban_Parser.information["numOfWalls"]); 
        int countgoal = Convert.ToInt32(sokoban_Parser.information["numOfGoals"]);
        int countAgents = Convert.ToInt32(sokoban_Parser.information["numberOfPlayers"]);
        int countFloor = char_map.GetLength(1) * char_map.GetLength(2);
        // -------Generate map by the MapGenerator class-------
        #region
        //MapGenerator mg = new MapGenerator();
        //char[,,] char_map = mg.GenerateMap(1);
        //int countbox = 0; // # of boxes / goals
        //int countwall = 0;
        //int countgoal = 0;
        //int countAgents = 0;
        //int countfloors = 0;

        //foreach (SimulationObject c in char_map){
        //    if (c.getName() == "b")
        //        countbox++;
        //    if (c.getName() == "w")
        //        countwall++;
        //    if (c.getName() == "g")
        //        countgoal++;
        //    if (c.getName() == "h")
        //        countAgents++;
        //    if (c.getName() == "f")
        //        countfloors++;
        //}
        #endregion

        //Initializing setting before drawing the objects in unity 
        if (boxes != null && goals != null)
        {
            wall.SetActive(true);
            box.SetActive(true);
            agent.SetActive(true);
            goal.SetActive(true);
            floor.SetActive(true);

            foreach (Transform child in maphold.transform)
            {
                Destroy(child.gameObject);
            }
        }
        boxes = new GameObject[countbox];
        goals = new GameObject[countgoal];
        walls = new GameObject[countwall];
        agents = new GameObject[countAgents];
        floors = new GameObject[countFloor];

        //-----Drawing map by SimulationObjects map recieved from the parser------
        DrawMap(char_map);

        //----Drawing map with char array recieved from the parser-----
        //DrawCharMap(char_map);
        
        Deactivate(box);
        Deactivate(wall);
        Deactivate(floor);
        //Deactivate(agent);
        Deactivate(goal);

        // actions = parser Plan file

    }

    // Agents moves
    void Update()
    {
        if (goals.Length != 0 && checkwin())
        {
            endingScreen.gameObject.SetActive(true);
            canvasmenu.gameObject.SetActive(true);
            Deactivate(agent);
            maphold.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.F))
            controlEnabled = !controlEnabled;

        if (keyboardPlayer)
        {
            if (!controlEnabled)
            {
                AgentMovements(false);
            }
        }
        else
        {
            AgentMovements(true);
            //AgentMovements(actions);
        }

    }


    private void DrawMap(SimulationObject[,,] map)
    {
        int flagBoxes = 0;
        int flagGoals = 0;
        int flagWall = 0;
        int flagAgents = 0;
        int flagFloors= 0;

        for (int y = 0; y < map.GetLength(0) - 1 ; y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int z = 0; z < map.GetLength(2); z++)
                {
                    GameObject g = null;
                    if (isWall(map[y, x, z]))
                    {
                        g = Instantiate(wall);
                        walls[flagWall] = g;                 
                        InitializeWall(walls[flagWall], x, y, z);
                        flagWall++;
                    }
                    if (isFloor(map[y, x, z]))
                    {
                        g = Instantiate(floor);
                        floors[flagFloors] = g;                 
                        InitializeFloor(floors[flagFloors], x, y, z);
                        flagFloors++;
                    }
                    if (isBox(map[y, x, z]))
                    {
                        g = Instantiate(box);
                        boxes[flagBoxes] = g;
                        InitializeCube(boxes[flagBoxes], x, y, z);
                        flagBoxes++;
                    }
                    if (isGoal(map[y, x, z]))
                    {
                        g = Instantiate(goal);
                        goals[flagGoals] = g;
                        InitializeCube(goals[flagGoals], x, y, z);
                        flagGoals++;
                    }
                    if (isAgent(map[y, x, z]))
                    {
                        g = Instantiate(agent);
                        agents[flagAgents] = g;
                        InitializeWorker(agents[flagAgents], x, y, z);
                        flagAgents++;
                    }
                    if (g != null)
                        g.transform.parent = maphold.transform;
                }
            }
        }
    }

    private void DrawCharMap(char[,,] map)
    {
        int flagBoxes = 0;
        int flagGoals = 0;
        int flagWall = 0;
        int flagAgents = 0;
        int flagFloors = 0;

        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                for (int z = 0; z < map.GetLength(2); z++)
                {
                    GameObject g = null;
                    if (map[y, x, z] == 'w')
                    {
                        g = Instantiate(wall);
                        walls[flagWall] = g;
                        InitializeWall(walls[flagWall], x, y, z);
                        flagWall++;
                    }
                    if (map[y, x, z] == 'f')
                    {
                        g = Instantiate(floor);
                        floors[flagFloors] = g;
                        InitializeFloor(floors[flagFloors], x, y, z);
                        flagFloors++;
                    }
                    if (map[y, x, z] == 'b')
                    {
                        g = Instantiate(box);
                        boxes[flagBoxes] = g;
                        InitializeCube(boxes[flagBoxes], x, y, z);
                        flagBoxes++;
                    }
                    if (map[y, x, z] == 'g')
                    {
                        g = Instantiate(goal);
                        goals[flagGoals] = g;
                        InitializeCube(goals[flagGoals], x, y, z);
                        flagGoals++;
                    }
                    if (map[y, x, z] == 'h')
                    {
                        g = Instantiate(agent);
                        agents[flagAgents] = g;
                        InitializeWorker(agents[flagAgents], x, y, z);
                        flagAgents++;
                    }
                    if (g != null)
                        g.transform.parent = maphold.transform;
                }
            }
        }
        Deactivate(box);
        Deactivate(wall);
        Deactivate(floor);
        //Deactivate(worker);
        Deactivate(goal);
    }


    /**
     * This function responsible for the ai agent moves.
     * 
     * Boolean 'isAgent' represent if the prototype currently is on - keyboard agent movement,
     * or the agent moves autonomously.
     * 
     */
    //private void AgentMovements(bool isAgent)
    //{
    //    foreach (KeyValuePair<int, string> action in actions)
    //    {
    //        string value = action.Value;
    //        int index = action.Key;

    //        if (value.Equals("U"))
    //        {
    //            CheckBoxMovment(agents[index].GetComponent<Transform>().position, new Vector3(-1, 0, 0));
    //            agents[index].GetComponent<Transform>().position += new Vector3(-1, 0, 0);
    //            //ValidifyMove(agents[index], new Vector3(-1, 0, 0));
    //        }
    //        if (value.Equals("R"))
    //        {
    //            CheckBoxMovment(agents[index].GetComponent<Transform>().position, new Vector3(0, 0, 1));
    //            agents[index].GetComponent<Transform>().position += new Vector3(0, 0, 1);
    //            //ValidifyMove(agents[index], new Vector3(0, 0, 1));
    //        }
    //        if (value.Equals("D"))
    //        {
    //            CheckBoxMovment(agents[index].GetComponent<Transform>().position, new Vector3(1, 0, 0));
    //            agents[index].GetComponent<Transform>().position += new Vector3(1, 0, 0);
    //            //ValidifyMove(agents[index], new Vector3(1, 0, 0));

    //        }
    //        if (value.Equals("L"))
    //        {
    //            CheckBoxMovment(agents[index].GetComponent<Transform>().position, new Vector3(0, 0, -1));
    //            agents[index].GetComponent<Transform>().position += new Vector3(0, 0, -1);
    //            //ValidifyMove(agents[index], new Vector3(0, 0, -1));
    //        }
    //    }
    //}

    /*** This function check if in the agent moves he pushes box
     *
     * 
     */
    private void CheckBoxMovment (Vector3 position, Vector3 move)
    {
        foreach (GameObject tbox in boxes)
        {
            if (position + move == tbox.GetComponent<Transform>().position)
            {
                //ValidifyMove(tbox, new Vector3(-1, 0, 0));
                tbox.GetComponent<Transform>().position += move;
            }
        }
    }

		private Animator anim;
		private CharacterController controller;
        		public float speed = 600.0f;
		public float turnSpeed =600.0f;
		private Vector3 moveDirection = Vector3.zero;
		public float gravity = 20.0f;

    // old function
    private void AgentMovements(bool isAgent)
    {

            int i = 0;
            controller = GetComponent <CharacterController>();
			anim = gameObject.GetComponentInChildren<Animator>();
        // anim.SetInteger ("AnimationPar", 0);

        if (Input.GetKeyDown(KeyCode.W))
        {
            anim.SetInteger ("AnimationPar", 1);
            foreach (GameObject tbox in boxes)
            {
                if (agents[i].GetComponent<Transform>().position + new Vector3(-1, 0, 0) == tbox.GetComponent<Transform>().position)
                {
                    ValidifyMove(tbox, new Vector3(-1, 0, 0));
                }
            }

            // Vector3 newPos = anim.bodyPosition +  new Vector3(-1, 0, 0);
            anim.transform.Rotate(anim.transform.right);
            // anim.transform.Rotate(0, 90 * turnSpeed * Time.deltaTime, 0);
            // anim.transform.Rotate(anim.bodyPosition);
            // controller.Move(moveDirection * Time.deltaTime);
            // moveDirection.y -= gravity * Time.deltaTime;
            ValidifyMove(agents[i], new Vector3(-1, 0, 0));

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            anim.SetInteger ("AnimationPar", 1);
            foreach (GameObject tbox in boxes)
            {
                if (agents[i].GetComponent<Transform>().position + new Vector3(1, 0, 0) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(1, 0, 0));
            }
            // anim.transform.Rotate(-anim.transform.forward);

            ValidifyMove(agents[i], new Vector3(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            anim.SetInteger ("AnimationPar", 1);
            foreach (GameObject tbox in boxes)
            {
                if (agents[i].GetComponent<Transform>().position + new Vector3(0, 0, -1) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, 0, -1));
            }
            // anim.transform.Rotate(-anim.transform.right);

            ValidifyMove(agents[i], new Vector3(0, 0, -1));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            anim.SetInteger ("AnimationPar", 1);
            foreach (GameObject tbox in boxes)
            {
                if (agents[i].GetComponent<Transform>().position + new Vector3(0, 0, 1) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, 0, 1));
            }
            // anim.transform.Rotate(anim.transform.right);

            ValidifyMove(agents[i], new Vector3(0, 0, 1));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (GameObject tbox in boxes)
            {
                if (agents[i].GetComponent<Transform>().position + new Vector3(0, 1, 0) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, 1, 0));
            }
            ValidifyMove(agents[i], new Vector3(0, 1, 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (GameObject tbox in boxes)
            {
                if (agents[i].GetComponent<Transform>().position + new Vector3(0, -1, 0) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, -1, 0));
            }
            ValidifyMove(agents[i], new Vector3(0, -1, 0));
        }
        // float turn = Input.GetAxis("Horizontal");
        // anim.transform.Rotate(0, 90 * turnSpeed * Time.deltaTime, 0);
        // controller.Move(moveDirection * Time.deltaTime);
        // moveDirection.y -= gravity * Time.deltaTime;

    }

    /*
     * This function checks if all boxes are in their goal positions
     */
    bool checkwin()
    {
        int goalflags = 0;
        float y_goal_box = 0.5f;
        foreach (GameObject tgoal in goals)
            foreach (GameObject tbox in boxes)
            {
                // Since the goal is ObjectGame at position (0,0.5,0) we check the addition of goal position to the box position
                if ((tgoal.GetComponent<Transform>().position + new Vector3(0, y_goal_box, 0)).Equals(tbox.GetComponent<Transform>().position))
                    goalflags++;
            }
        if (goals.Length == goalflags)
            return true;
        return false;
    }



    void InitializeWall(GameObject box, int x, int y, int z)
    {
        Vector3 size = box.GetComponent<Renderer>().bounds.size;
        box.GetComponent<Transform>().localScale = new Vector3(blockSize / size.x, blockSize / size.y, blockSize / size.z);
        box.GetComponent<Transform>().position = new Vector3(x * blockSize, y * blockSize, z * blockSize);
    }

    void InitializeFloor(GameObject box, int x, int y, int z)
    {
        Vector3 size = box.GetComponent<Renderer>().bounds.size;
        box.GetComponent<Transform>().localScale = new Vector3(blockSize / size.x, blockSize / size.y, blockSize / size.z);
        box.GetComponent<Transform>().position = new Vector3(x * blockSize, y * blockSize, z * blockSize);
    }

    void InitializeCube(GameObject box, int x, int y, int z)
    {
        // box.AddComponent<MeshRenderer>();
        // // box.AddComponent<LineRenderer>();
        // Vector3 size = box.GetComponent<Renderer>().bounds.size;
        // print( box.GetComponent<Renderer>());
        // box.GetComponent<Transform>().localScale = new Vector3(blockSize / size.x, blockSize / size.y, blockSize / size.z);
        box.GetComponent<Transform>().position += new Vector3(x * blockSize, y * blockSize, z * blockSize);
    }

    void InitializeWorker(GameObject box, int x, int y, int z)
    {
        // box.AddComponent<MeshRenderer>();
        // Vector3 size = box.GetComponent<Renderer>().bounds.size;
        // box.GetComponent<Transform>().localScale = new Vector3(blockSize / size.y, blockSize / size.y, blockSize / size.y);
        box.GetComponent<Transform>().position = new Vector3(x * blockSize, y * blockSize, z * blockSize);
    }

    void Deactivate(GameObject o)
    {
        o.SetActive(false);
    }

    void Deactivate(GameObject[] arr)
    {
        foreach (GameObject o in arr)
            o.SetActive(false);
    }

    public void deleteMap()
    {
        Deactivate(walls);
        Deactivate(wall);
        Deactivate(floors);
        Deactivate(floor);
        Deactivate(goals);
        Deactivate(goal);
        Deactivate(agents);
        Deactivate(agent);
        Deactivate(boxes);
        Deactivate(box);
    }

    /*
     * Validating Agent movements:
     *      illegal move: through walls, through boxes where there is walls after it.
     *      legal move: clear step ahead, box with clear step after it .
     */
    void ValidifyMove(GameObject o, Vector3 mov)
    {
        Vector3 pos = o.GetComponent<Transform>().position + mov;
        if (wallInPosition(pos))
            return;
        if (boxInPosition(pos))
        {
            Vector3 nextPos = pos + mov;
            if (wallInPosition(nextPos) || boxInPosition(nextPos))
                return;
        }
        o.GetComponent<Transform>().position += mov;
    }

    /*
     * Returns true if there is a box in the given position 
     * */
    private bool boxInPosition(Vector3 pos)
    {
        foreach (GameObject box in boxes)
        {
            if (box.GetComponent<Transform>().position == pos)
                return true;
        }
        return false;
    }

    /**
     * Returns true if there is a wall in the given position
     * */
    private bool wallInPosition(Vector3 pos)
    {
        foreach (GameObject wall in walls)
        {
            if (wall.GetComponent<Transform>().position == pos)
                return true;
        }
        return false;
    }

    // checks if the simulation objects in the map is a wall
    private bool isWall(SimulationObject obj)
    {
        if (obj.getName().Equals("w"))
        {
            return true;
        }
        return false;
    }
    
    // checks if the simulation objects in the map is a wall
    private bool isFloor(SimulationObject obj)
    {
        if (obj.getName().Equals("f"))
        {
            return true;
        }
        return false;
    }

    //checks if the simulation object in the map is an agent
    private bool isAgent(SimulationObject obj)
    {
        if (obj.getName().Equals("h"))
        {
            return true;
        }
        return false;
    }
    
    //checks if the simulation object in the map is a goal
    private bool isGoal(SimulationObject obj)
    {
        if (obj.getName().Equals("g"))
        {
            return true;
        }
        return false;
    }

    //checks if the simulation object in the map is a box
    private bool isBox(SimulationObject obj)
    {
        if (obj.getName().Equals("b"))
        {
            return true;
        }
        return false;
    }

}
