using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{

    //Objects
    public Canvas canvas;

    public Canvas canvasmune;

    public Canvas cong;

    public GameObject maphold;

    private MapGenerator mapGenerator = new MapGenerator();

    //Fields
    public char[,,] map;
    private bool controlEnabled = false; // Face Camera option was clicked ('c' in keyboard)
    private bool keyboardPlayer = true; // Prototype option - Agent moves currently with the 

    public GameObject worker;
    public GameObject wall;
    public GameObject box;
    public GameObject goal;

    public GameObject[] walls;
    public GameObject[] boxes;
    public GameObject[] goals;

    int blockSize = 1;  // size of each block

    // Use this for initialization
    void Start()
    {
        canvasmune.gameObject.SetActive(false);
        cong.gameObject.SetActive(false); // asd

    }

    // Update is called once per frame

    public void backmune()
    {
        canvas.gameObject.SetActive(true);
        canvasmune.gameObject.SetActive(false);
        //cong.gameObject.SetActive(false);
    }

    public void setmap(int i)
    {
        canvas.gameObject.SetActive(false);
        cong.gameObject.SetActive(false);
        maphold.SetActive(true);
        canvasmune.gameObject.SetActive(true);
        if (i == 1)
        {
            map = mapGenerator.GenerateMap(1);
        }
        if (i == 2)
        {
            map = mapGenerator.GenerateMap(2);
        }
        int countbox = 0; // # of boxes / goals
        int countwall = 0;
        int countgoal = 0;

        foreach (char c in map)
        {
            if (c == 'b')
                countbox++;
            if (c == 'w')
                countwall++;
            if (c == 'g')
                countgoal++;
        }
        if (boxes != null && goals != null)
        {
            wall.SetActive(true);
            box.SetActive(true);
            worker.SetActive(true);
            goal.SetActive(true);
            foreach (Transform child in maphold.transform)
            {
                Destroy(child.gameObject);
            }

        }
        boxes = new GameObject[countbox];
        goals = new GameObject[countgoal];
        walls = new GameObject[countwall];

        int flagBoxes = 0;
        int flagGoals = 0;
        int flagWall = 0;

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
                        //g = Instantiate(worker);
                        InitializeWorker(worker, x, y, z);
                    }
                    if (g != null)
                        g.transform.parent = maphold.transform;
                }
            }
        }
        Deactivate(box);
        Deactivate(wall);
        //Deactivate(worker);
        Deactivate(goal);





        // For each x
        // For each y
        // For each z

        // if box here, then {
        // Instantiate
        //GameObject m = GameObject.Instantiate(model);
        //Transform t = m.transform;
        //Vector3 p = t.position;

        //}
    }

    void Update()
    {

        if (goals.Length != 0 && checkwin())
        {
            cong.gameObject.SetActive(true);
            canvasmune.gameObject.SetActive(true);
            Deactivate(worker);
            maphold.SetActive(false);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
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
        }

    }

    /**
     * This function responsible for the ai agent moves.
     * 
     * Boolean 'isAgent' represent if the prototype currently is on - keyboard agent movement,
     * or the agent moves autonomously.
     * 
     */
    private void AgentMovements(bool isAgent)
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            foreach (GameObject tbox in boxes)
            {
                if (worker.GetComponent<Transform>().position + new Vector3(-1, 0, 0) == tbox.GetComponent<Transform>().position)
                {
                    ValidifyMove(tbox, new Vector3(-1, 0, 0));
                }
            }
            ValidifyMove(worker, new Vector3(-1, 0, 0));

        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            foreach (GameObject tbox in boxes)
            {
                if (worker.GetComponent<Transform>().position + new Vector3(1, 0, 0) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(1, 0, 0));
            }
            ValidifyMove(worker, new Vector3(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            foreach (GameObject tbox in boxes)
            {
                if (worker.GetComponent<Transform>().position + new Vector3(0, 0, -1) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, 0, -1));
            }
            ValidifyMove(worker, new Vector3(0, 0, -1));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            foreach (GameObject tbox in boxes)
            {
                if (worker.GetComponent<Transform>().position + new Vector3(0, 0, 1) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, 0, 1));
            }
            ValidifyMove(worker, new Vector3(0, 0, 1));
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            foreach (GameObject tbox in boxes)
            {
                if (worker.GetComponent<Transform>().position + new Vector3(0, 1, 0) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, 1, 0));
            }
            ValidifyMove(worker, new Vector3(0, 1, 0));
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            foreach (GameObject tbox in boxes)
            {
                if (worker.GetComponent<Transform>().position + new Vector3(0, -1, 0) == tbox.GetComponent<Transform>().position)
                    ValidifyMove(tbox, new Vector3(0, -1, 0));
            }
            ValidifyMove(worker, new Vector3(0, -1, 0));
        }
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
                if ((tgoal.GetComponent<Transform>().position + new Vector3(0, y_goal_box, 0)).Equals(tbox.GetComponent<Transform>().position))
                    goalflags++;
            }
        if (goals.Length == goalflags)
            return true;
        return false;
    }



    void InitializeWall(GameObject box, int x, int y, int z)
    {
        // box.AddComponent<Renderer>();
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
            if (wallInPosition(nextPos))
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
}
