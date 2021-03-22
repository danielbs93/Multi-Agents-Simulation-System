using System;
using System.Collections;
using System.Collections.Generic;
using testElevator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EMSSManager: MonoBehaviour
{
    // Unity in-game objects
    public GameObject floor;
    public GameObject elevator;
    public GameObject ceiling;
    public GameObject passenger;
    
    // Simuation Objects
    public GameObject [][] floors;
    public GameObject [] elevators;
    public GameObject[] passengers;

    // ======= Cameras ========
    private Queue<Camera> cameraQueue;
    private Camera activeCameraObject;
    //Mouse scroll wheel adjustments
    private float minFov = 15f;
    private float maxFov = 90f;
    private float sensitivity = 10f;

    // Position controllers
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;

    // Speed controllers
    private float elevatorSpeed = 1;
    private float stepX = 7;
    private float stepY = 4;

    // Fields
    private bool mapInitialized =  false;
    private int numOfFloors = 4; 
    private int numOfElevators = 4; 
    private int numOfPassengers = 4; 

    private List<Elevator> elevatorsObjects;
    private List<Floor> floorObjects;
    private List<Passenger> passengersObjects;

    private string initPath;
    private string planPath;

    // Step by Step mode or continuus running
    private bool gameHold = false;
    private bool stepFlag = false;

    private string[] singleFloorsNumbers = { "01", "02", "03", "04", "05", "06", "07", "08", "09" };

    private emssParser emssParser;
    //private emssPlanParser emssPlanParser;

    void Start()
    {
        //floors = new GameObject[numOfElevators][];
        //elevators = new GameObject[numOfElevators];
        //for (int flo = 0; flo < numOfElevators; flo++)
        //{
        //    floors[flo] = new GameObject[numOfFloors];
        //}
        //BuildFloors();
        //initCameras();
    }

    // Update is called once per frame
    private int index = 0;

    public bool MapInitialized { get => mapInitialized; set => mapInitialized = value; }

    void Update()
    {
        if (MapInitialized)
        {
            //Switching cameras
            if (Input.GetMouseButtonDown(1))
            {
                switchCameras();
            }

            //Mouse Scroll Wheel zoom-in and zoom-out
            float fov = activeCameraObject.fieldOfView;
            fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
            fov = Mathf.Clamp(fov, minFov, maxFov);
            activeCameraObject.fieldOfView = fov;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                MoveElevator(elevatorsObjects[index].ElevatorGameObject, 1, 1);
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                MoveElevator(elevatorsObjects[index].ElevatorGameObject, -1, 1);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (index < 3)
                    index++;
                // print(index);
                else
                    index = 0;
            }
        }
    }

    /*
     Creating game objects by the prefabs assigned to the EmssManager object in the Unity platform
     */
    private void InitializeObject(GameObject g, float x, float y, float z)
    {
        g.GetComponent<Transform>().position = new Vector3(x,y,z);
    }

    public void buildMap()
    {
        floors = new GameObject[numOfElevators][];
        elevators = new GameObject[numOfElevators];
        for (int flo = 0; flo < numOfElevators; flo++)
        {
            floors[flo] = new GameObject[numOfFloors];
        }
        BuildFloors();
        initCameras();
    }

    /*
     Building domain with the info given from the init parser
     Init Floors, Elevators and Passengers.
     */
    private void BuildFloors()
    {

        for (int i = 0; i < numOfElevators; i++)
        {
            y = 0;
            GameObject e = Instantiate(elevator);
            elevatorsObjects[i] .ElevatorGameObject= e;
            float elevFloor = (float)(stepY * elevatorsObjects[i].InitialPosition);
            InitializeObject(e,x, elevFloor, z);
            initElevatorGameObjectParams(e, elevatorsObjects[i]);
            for (int j = 0; j < numOfFloors; j++)
            {
                GameObject f = Instantiate(floor);
                if (j >=0 && j < 10)
                {
                    f.transform.GetChild(5).GetComponent<TMP_Text>().text = singleFloorsNumbers[j];
                    f.transform.GetChild(6).GetComponent<TMP_Text>().text = singleFloorsNumbers[j];
                }
                else
                {
                    f.transform.GetChild(5).GetComponent<TMP_Text>().text = j.ToString();
                    f.transform.GetChild(6).GetComponent<TMP_Text>().text = j.ToString();
                }
                floors[i][j] = f;
                InitializeObject(floors[i][j],x,y,z);
                y = y + stepY;
            }

            x = x + stepX;
        }

        for (int i = 0; i < numOfPassengers; i++)
        {
            GameObject p = Instantiate(passenger);
            passengersObjects[i].PassengerGameObject = p;

            System.Random rand = new System.Random();
            float constX = 0f, constY = 0f, constZ = 0f;
            do
            {
                constZ = (float)(rand.NextDouble() * 2.0 + 1.0);
                //constX = (float)(rand.NextDouble()) * stepX * numOfElevators;
                constX = (float)(stepX * (numOfElevators-1) / (i+1));
                constY = (float)(stepY * passengersObjects[i].Departured.Number);
            } while (isPassengerAtPosition(constX, constY, constZ));

            passengersObjects[i].Position = new float[] { constX, constY, constZ };

            InitializeObject(p, constX, constY+0.05f, constZ);
            initPassengerInfo(p, passengersObjects[i]);
        }
    }

    private void initPassengerInfo(GameObject p, Passenger passenger)
    {
        p.transform.GetChild(11).GetComponent<TMP_Text>().text = "Name: P" + passenger.Name;
        p.transform.GetChild(12).GetComponent<TMP_Text>().text = "Destination: F" + passenger.Destination.Number;
    }

    private bool isPassengerAtPosition(float x, float y, float z)
    {
        foreach (Passenger p in passengersObjects)
        {
            if (p.Position.Equals(new float[] { x,y,z}))
            {
                return true;
            }
        }
        return false;
    }

    private void initElevatorGameObjectParams(GameObject e, Elevator elev)
    {
        e.transform.GetChild(3).GetComponent<TMP_Text>().text = "Name: " + elev.Name;
        if (elev.CurrentNumberOfPassengers > elev.TotalCapacity)
        {
            e.transform.GetChild(4).GetComponent<TMP_Text>().color = Color.red;
        }
        else
        {
            e.transform.GetChild(4).GetComponent<TMP_Text>().color = Color.green;
        }
        e.transform.GetChild(4).GetComponent<TMP_Text>().text = "Capacity: 0/" + elev.TotalCapacity;
        e.transform.GetChild(5).GetComponent<TMP_Text>().text = "Cost: " + elev.CurrentCost;
        //e.transform.GetChild(6).GetComponent<TMP_Text>().text = "Cost: " + elev.CurrentCost;
    }

    private void MoveElevator(GameObject e, int direction, int numFloors)
    {   
        Vector3 localPosition = e.GetComponent<Transform>().localPosition;
        e.GetComponent<Transform>().localPosition = Vector3.Lerp(localPosition, localPosition + new Vector3(0,direction*stepY*numFloors,0), Time.deltaTime*elevatorSpeed);
    }

    /*
     Initializing Cameras game objects
    Main Camera - parallel to the building
    Side Camera Right - right view of the building
    Side Camera Left - left view of the building
    Finding the cameras for each Elevator in our system and inserting them into the camera queue.
     */
    private void initCameras()
    {
        cameraQueue = new Queue<Camera>();
        activeCameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Camera sideCameraRight = GameObject.FindGameObjectWithTag("SideCameraRight").GetComponent<Camera>();
        sideCameraRight.gameObject.SetActive(false);
        Camera sideCameraLeft = GameObject.FindGameObjectWithTag("SideCameraLeft").GetComponent<Camera>();
        sideCameraLeft.gameObject.SetActive(false);
        cameraQueue.Enqueue(sideCameraRight);
        cameraQueue.Enqueue(sideCameraLeft);

        foreach (Elevator elev in elevatorsObjects)
        {
            GameObject go = elev.ElevatorGameObject;
            Camera current = go.GetComponentInChildren<Camera>();
            current.gameObject.SetActive(false);
            cameraQueue.Enqueue(current);
        }
    }

    /*
     Switching cameras by right click mouse command
     */
    public void switchCameras()
    {
        Camera tempCamera = activeCameraObject;
        activeCameraObject.gameObject.SetActive(false);
        activeCameraObject = cameraQueue.Dequeue();
        activeCameraObject.gameObject.SetActive(true);
        Canvas spaceCanvas = GameObject.FindGameObjectWithTag("SpaceCanvas").GetComponent<Canvas>() as Canvas;
        spaceCanvas.worldCamera = activeCameraObject;
        cameraQueue.Enqueue(tempCamera);
    }

    public void setEmssPaths(string init, string plan)
    {
        initPath = init;
        planPath = plan;
    }

    public void initParser()
    {
        emssParser = new emssParser(initPath);
        numOfElevators = emssParser.FastElevators + emssParser.SlowElevators;
        numOfFloors = emssParser.NumOfFloors;
        numOfPassengers = emssParser.NumOfPassengers;
        elevatorsObjects = emssParser.Elevators;
        passengersObjects = emssParser.getPassengers;
        floorObjects = emssParser.Floors;
    }

    public void planParser()
    {
        //emssPlanParser = new emssPlanParser(planPath);
    }

    /*
    Simulation tranformation to continuus running
    */
    public void playSimMode()
    {
        gameHold = false;
        stepFlag = false;
    }

    /*
     Simulation transformation to step-by-step mode by demand
     */
    public void stepByStepMode()
    {
        gameHold = true;
        stepFlag = true;
    }

}
