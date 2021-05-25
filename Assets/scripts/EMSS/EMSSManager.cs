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
    public GameObject destRoom;
    public Text actionNumber;


    // Simulation run time Objects
    public GameObject [][] floors;
    public GameObject [] elevators;
    public GameObject[] passengers;
    public GameObject[] ceilings;
    public GameObject[] destRooms;


    // ======= Cameras ========
    private Queue<Camera[]> cameraQueue;
    private Queue<Camera> activeCameras;
    private Camera activeCameraObject;
    private Camera[] mainCameras;
    private Camera[] view1Cameras;
    private Camera[] view2Cameras;
    private Camera[] view3Cameras;
    //Mouse scroll wheel adjustments
    private float minFov = 15f;
    private float maxFov = 180f;
    private float sensitivity = 10f;

    // Position controllers
    private float x = 0f;
    private float y = 0f;
    private float z = 0f;

    // Speed controllers
    private float stepX = 7;
    private float stepY = 4;

    //==== Fields ====
    private int numOfFloors = 4; 
    private int numOfElevators = 4; 
    private int numOfPassengers = 4;
    private float _std = 0.067f;

    // Init parser objects
    private List<Elevator> elevatorsObjects;
    private List<Floor> floorObjects;
    private List<Passenger> passengersObjects;
    private Material[] passengersClothingColors;

    private string initPath;
    private string planPath;

    // Step by Step mode or continuus running
    private bool gameHold = false;
    private bool stepFlag = false;
    private bool stepBackwardFlag = false;
    private bool firstTimeBackwardPressed = true;

    // Actions controlers 
    private int frames = 0;
    private float simulationSpeed = 30;
    private float elevatorSpeed = 300;
    
    // Plan fields needed to run the simulation automatically
    private List<testElevator.Action> actions;
    private int actionCounter = 0;

    // Caurrent objects state of the simulation
    private testElevator.Action currentAction;
    private Elevator currentElevatorObject;
    private Passenger currentPassengerObject;

    private float currentDirectionX;
    private int currentDirectionY;
    private float currentDirectionZ;

    // controller flags for smooth moving of the unity game objects
    private bool actionIsOnGoing = false;
    private bool elevatorActionFlag = false;
    private bool passengerActionFlag = false;

    // replacing floors single digits to double digits
    private string[] singleFloorsNumbers = { "00","01", "02", "03", "04", "05", "06", "07", "08", "09" };

    // Parsers objects
    private emssParser emssParser;
    private ParsePlanElevator emssPlanParser;

    void Start()
    {
        actionNumber = GameObject.Find("ActionNumber").GetComponent<Text>();
    }

    void Update()
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

        // Elevators agents movements - plan execution
        frames++;
        if (actionCounter <= actions.Count && (gameHold == stepFlag || gameHold == stepBackwardFlag))
        {
            if (!actionIsOnGoing && frames % simulationSpeed == 0)
            {
                    // Iterating over the agents actions
                    IterateActionsList();
                    frames = 0;
            }
            else if (gameHold == stepFlag)
            {
                // elevator movement flag
                if (elevatorActionFlag) { ElevatorIsMoving(); }

                // passenger movement flag
                if (passengerActionFlag) { PassengerIsMoving(); }
            }
        }
    }



    /*
     Extracting Action objects each time the next conditions are fulfilled:
        + There are more actions in the list
        + if we in step by step mode each click in the button will take only one Action from the list
     */
    private void IterateActionsList()
    {
        actionNumber.text = actionCounter.ToString(); 
        if (actionCounter < actions.Count)
        {
            currentAction = actions[actionCounter];
            if (currentAction != null && gameHold == stepFlag)
            {       
                //if (stepBackwardFlag && actionCounter >= 0)
                //{
                //    testElevator.Action reversedAction = ReverseActionProperties();
                //    SimulationMovements(reversedAction);
                //}
                //else
                //{
                    SimulationMovements(currentAction);
                    actionCounter++;
                //}

                if (gameHold && stepFlag)
                {
                    stepFlag = !stepFlag;
                }
            }
            else if (currentAction != null && gameHold == stepBackwardFlag)
            {
                if (actionCounter >= 0)
                {
                    testElevator.Action reversedAction = ReverseActionProperties();
                    ReturnToFormerPositions(reversedAction);// switch to new function of just placing the objects in their former state
                }

                if (gameHold && stepBackwardFlag)
                {
                    stepBackwardFlag = !stepBackwardFlag;
                }
            }
        }
    }

    /*
     For each Action given in the List<Action> we classify them as 3:
        + MoveAction: Elevator is moving from one floor to another
        + PassengerAction:
            + Type1 - Board: a passenger is getting aboard to an elevator
            + Type2 - Leave: a passenger is leaving the current elevator
    In each PassengerAction we updating the current state of an elevator: current capacity/ total capacity and the current cost by 
        the cost dictionary provided in the init files that have been readen by the emssParser
     */
    private void SimulationMovements(testElevator.Action action)
    {
        if (action is MoveAction)
        {
            passengerActionFlag = false;
            elevatorActionFlag = true;
            string elevName = action.ExecutorName;

            CalculateAndSetElevatorCost(elevName, ((MoveAction)action));

            currentElevatorObject = getElevatorByName(elevName);
            currentDirectionY = ((MoveAction)action).Direction;
            actionIsOnGoing = true;

            Vector3 view1UpperCameraPosition = view1Cameras[0].gameObject.transform.position;
            float desiredY = ((MoveAction)action).DeparuredFloor * stepY+ 2.5f;
            view1Cameras[0].gameObject.transform.position = new Vector3(view1UpperCameraPosition.x, desiredY, view1UpperCameraPosition.z);
        }

        if (action is PassengerAction)
        {
            // Placing the view1UpperCamera in the floor that the passenger is moving on
            if (actionCounter == 0)
            {
                int thisFloorNumber = ((PassengerAction)action).FloorNumber;
                PlaceView1UpperCameraInInitFloor(thisFloorNumber);
            }

            elevatorActionFlag = false;
            passengerActionFlag = true;

            string elevName = ((PassengerAction)action).ElevatorName;
            currentElevatorObject = getElevatorByName(elevName);


            string passName = ((PassengerAction)action).ExecutorName;
            currentPassengerObject = getPassengerByName(passName);

            // start walking
            currentPassengerObject.WalkingOrStandingMovement(true);

            if (((PassengerAction)action).IsBoard)
            {
                Vector3 incitement = currentElevatorObject.getNextFreeSpace();
                currentDirectionX = currentElevatorObject.GetGameObjectTransform().position.x + incitement.x;
                currentDirectionZ = currentElevatorObject.GetGameObjectTransform().position.z + incitement.z;

                currentPassengerObject.BoardAction = true;
                currentPassengerObject.InsideElevatorIncitement = incitement;
                currentPassengerObject.rotatePassengerTowardsX(currentDirectionX);
            }
            else
            {
                // Detaching the parent elevtor game object from the passenger game object
                currentPassengerObject.GetGameObjectTransform().SetParent(null);
                currentPassengerObject.GetGameObjectTransform().position = currentElevatorObject.GetGameObjectTransform().position + currentPassengerObject.InsideElevatorIncitement;
                currentElevatorObject.freePassengerSpace(currentPassengerObject.InsideElevatorIncitement);

                // Sending the Passenger to the destination floor in case it is his target or find him random place in the current floor
                System.Random rand = new System.Random();
                if (currentPassengerObject.Destination.Number == ((PassengerAction)action).FloorNumber+1 && !stepBackwardFlag)
                {
                    currentDirectionZ = (float)(rand.NextDouble() * 1.5 + 1.1);
                    float maxD = (float)((numOfElevators + 1) * stepX) - 3.7f;
                    float minD = (float)(numOfElevators * stepX) + 0.5f;
                    currentDirectionX = (float)(rand.NextDouble() * (maxD - minD)) + minD;
                }
                // Finding random space in the floor for the passenger
                else
                {
                    currentDirectionZ = (float)(rand.NextDouble() * 1.9 + 1.1);
                    currentDirectionX = (float)(rand.NextDouble()* ((numOfElevators*stepX)-2)) + 1;
                }

                currentPassengerObject.RotatePassengerInfo(false);

                currentPassengerObject.LeaveAction = true;

            }
            UpdateElevatorCapacity(elevName, ((PassengerAction)action).FinalCapacity);
            actionIsOnGoing = true;
        }
    }

    /*
     If the reversed action is MoveAction then the elevator will return to its former postion 
        i.e the floor the elevator was before the next action occurred
    If the reversed action is PassengerAction then the passenger will return to its former position immidietly 
    without any animation (so we can distinguish between step forward and step backward)
     */
    private void ReturnToFormerPositions(testElevator.Action reversedAction)
    {
        if (reversedAction is MoveAction)
        {
            string elevName = reversedAction.ExecutorName;
            CalculateAndSetElevatorCost(elevName, ((MoveAction)reversedAction));
            currentElevatorObject = getElevatorByName(elevName);
            Vector3 elevPos = currentElevatorObject.GetGameObjectTransform().position;
            currentElevatorObject.GetGameObjectTransform().position = new Vector3(elevPos.x, ((MoveAction)reversedAction).TargetFloor * stepY, elevPos.z);
        }
        else
        {
            string elevName = ((PassengerAction)reversedAction).ElevatorName;
            currentElevatorObject = getElevatorByName(elevName);

            string passName = ((PassengerAction)reversedAction).ExecutorName;
            currentPassengerObject = getPassengerByName(passName);

            Vector3 elevPos = currentElevatorObject.GetGameObjectTransform().position;
            currentElevatorObject.GetGameObjectTransform().position = new Vector3(elevPos.x, ((PassengerAction)reversedAction).FloorNumber * stepY, elevPos.z);

            // standing
            currentPassengerObject.WalkingOrStandingMovement(false);

            if (((PassengerAction)reversedAction).IsBoard)
            {
                Vector3 incitement = currentElevatorObject.getNextFreeSpace();
                float elevDirectionX = currentElevatorObject.GetGameObjectTransform().position.x + incitement.x;
                float elevDirectionZ = currentElevatorObject.GetGameObjectTransform().position.z + incitement.z;
                currentPassengerObject.InsideElevatorIncitement = incitement;

                currentPassengerObject.GetGameObjectTransform().SetParent(currentElevatorObject.GetGameObjectTransform());
                currentPassengerObject.GetGameObjectTransform().position = new Vector3(elevDirectionX, ((PassengerAction)reversedAction).FloorNumber * stepY, elevDirectionZ);
                currentPassengerObject.RotatePassengerInfo(true);

                currentPassengerObject.LeaveAction = false;
            }
            else
            {
                // Detaching the parent elevtor game object from the passenger game object
                currentPassengerObject.GetGameObjectTransform().SetParent(null);
                currentElevatorObject.freePassengerSpace(currentPassengerObject.InsideElevatorIncitement);

                System.Random rand = new System.Random();
                float randDirectionZ = (float)(rand.NextDouble() * 1.9 + 1.1);
                float randDirectionX = (float)(rand.NextDouble() * ((numOfElevators * stepX) - 4)) + 1;

                currentPassengerObject.GetGameObjectTransform().position = new Vector3(randDirectionX, ((PassengerAction)reversedAction).FloorNumber * stepY, randDirectionZ);
                currentPassengerObject.RotatePassengerInfo(false);

                currentPassengerObject.BoardAction = false;
            }
            currentPassengerObject.rotatePassengerTowardsZ();
            currentPassengerObject.rotatePassengerTowardsZ();
            UpdateElevatorCapacity(elevName, ((PassengerAction)reversedAction).FinalCapacity);
        }
    }

    /*
     Calculating the cost of an elevator movements by the emssParser of costs property 
        and update the result into the elevator cost property
     */
    private void CalculateAndSetElevatorCost(string elevName, MoveAction action)
    {
        int lowerF = Math.Min(action.DeparuredFloor, action.TargetFloor);
        int higherF = Math.Max(action.DeparuredFloor, action.TargetFloor);
        string costKey = lowerF.ToString() + "-" + higherF.ToString();
        int currentCost = emssParser.getFloorCost(costKey);
        if (stepBackwardFlag)
        {
            updateElevatorCost(elevName, -currentCost);
        }
        else
        {
            updateElevatorCost(elevName, currentCost);
        }
    }

    /*
     If Step backward option was selected then we have to reverse the properties of the former action that hase been performed
        Dividing into 2 options of actions - MoveAction and PassengerAction
        MoveAction: if the elevator went up then we need the reversed action to be get down with flipping the 
                            the target floor and the departured floor. Same with the other way around (went down -> goes up)
        PassengerAction: if the passenger has entered into the elevetor then we need it to leave it and update the capacity again.
                                    same if the passenger left the elevator -> boarding.
     */
    private testElevator.Action ReverseActionProperties()
    {
        if (currentAction is MoveAction)
        {
            MoveAction reversed = new MoveAction((MoveAction)currentAction);
            reversed.Direction *= -1;
            int temp = reversed.TargetFloor;
            reversed.TargetFloor = reversed.DeparuredFloor;
            reversed.DeparuredFloor = temp;
            return reversed;
        }
        else
        {
            PassengerAction reversed = new PassengerAction((PassengerAction)currentAction);
            if (reversed.IsBoard)
            {
                reversed.FinalCapacity -= 1;
            }
            else
            {
                reversed.FinalCapacity += 1;
            }
            reversed.IsBoard = !reversed.IsBoard;
            return reversed;
        }
    }

    /*
     Every frame the current elevator is moving towrds the current direction.
        The stop case is when it reaches the target floor according to the given field of the MoveAction.
        +- std for reaching to the destination
     */
    private void ElevatorIsMoving()
    {
        if (currentElevatorObject.GetGameObjectTransform().position.y <= ((MoveAction)currentAction).TargetFloor * stepY + (_std)
                    && currentElevatorObject.GetGameObjectTransform().position.y >= ((MoveAction)currentAction).TargetFloor * stepY - (_std))
        {
            actionIsOnGoing = false;
        }
        else
        {
            currentElevatorObject.MoveElevator(currentDirectionY, stepY, simulationSpeed, elevatorSpeed);
            MoveView1UpperCamera();
        }
    }

    // Passenger movement seperator - boarding or leaving
    private void PassengerIsMoving()
    {
        if (((PassengerAction)currentAction).IsBoard)
        {
            PassengerIsBoarding();
        }
        else 
        {
            PassengerIsLeaving();
        }
    }

    /*
     This function responsible for the passenger boarding operation:
     - while the passenger has not reached to its position on X axis he will still be walking
     - otherwise we will switch to walking on Z axis - EntertToElevator function
        +- std for reaching to the destination
     */
    private void PassengerIsBoarding()
    {
        // case the passenger arrive to position parallel to the elevator entrance
        if (currentPassengerObject.GetGameObjectTransform().position.x <= currentDirectionX + (_std)
                   && currentPassengerObject.GetGameObjectTransform().position.x >= currentDirectionX - (_std))
        {
            EnterToElevator();
        }
        // case the passenger will move on x axis to reach the parallel position of the desired elevator i.e right or left
        else
        {
            currentPassengerObject.MoveOnXaxis(currentDirectionX, simulationSpeed, elevatorSpeed);
        }
    }

    /*
     While the passenger has not reached to its position inside the elevator, walk on Z axis.
    Otherwise, the passenger has reached its position and then we will stop his actions and 
    connect together the elevator game object to the passenger game object.
        +- std for reaching to the destination
     
     */
    private void EnterToElevator()
    {
        // case the passenger reach the destination of elevator
        if (currentPassengerObject.GetGameObjectTransform().position.z <= currentDirectionZ + (_std)
                    && currentPassengerObject.GetGameObjectTransform().position.z >= currentDirectionZ - (_std))
        {
            // Action is done
            actionIsOnGoing = false;

            // Stop walking
            currentPassengerObject.WalkingOrStandingMovement(false);

            // Connect the passenger gameobject to the elevator gameobject
            if (((PassengerAction)currentAction).IsBoard)
            {
                //gameObjectMoving.transform.SetParent(gameObjectTarget.transform);
                currentPassengerObject.GetGameObjectTransform().SetParent(currentElevatorObject.GetGameObjectTransform());
            }
        }
        // case the passenger wlaks on z axis i.e forward
        else
        {
            currentPassengerObject.MoveOnZaxis(currentDirectionZ, simulationSpeed, elevatorSpeed);
        }
    }

    private void PassengerIsLeaving()
    {
        // case the passenger is got out from the elevator and in its parallel postion
        if (currentPassengerObject.GetGameObjectTransform().position.z <= currentDirectionZ + (_std)
                    && currentPassengerObject.GetGameObjectTransform().position.z >= currentDirectionZ - (_std))
        {
            ReachPassengerPlaceOnTheFloor();
        }
        // case the passenger walking on z axis i.e forward
        else
        {
            currentPassengerObject.MoveOnZaxis(currentDirectionZ, simulationSpeed, elevatorSpeed);
        }
    }

    // Passenger is walking to a random place in the floor after he gets out from the elevator
    private void ReachPassengerPlaceOnTheFloor()
    {
        // case the passenger reach a random place on the floor, rotate its face towrds the elevators
        if (currentPassengerObject.GetGameObjectTransform().position.x <= currentDirectionX + (_std)
                  && currentPassengerObject.GetGameObjectTransform().position.x >= currentDirectionX - (_std))
        {
            // Action is done
            actionIsOnGoing = false;

            // Stop walking
            currentPassengerObject.WalkingOrStandingMovement(false);

            currentPassengerObject.rotatePassengerTowardsZ();
        }
        // case the passenger walking on x axis to reach its random position on the floor
        else
        {
            currentPassengerObject.MoveOnXaxis(currentDirectionX, simulationSpeed, elevatorSpeed);
        }
    }

    /*
     Return Game Object of the elevator by its name
     */
    private Elevator getElevatorByName(string name)
    {
        foreach (Elevator e in elevatorsObjects)
        {
            if (e.Name.Equals(name))
            {
                //return e.ElevatorGameObject;
                return e;
            }
        }
        return null;
    }

    /*
     Return Game Object of the passenger by its name
     */
    private Passenger getPassengerByName(string name)
    {
        foreach (Passenger p in passengersObjects)
        {
            if (p.Name.Equals(name))
            {
                return p;
            }
        }
        return null;
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
        ceilings = new GameObject[numOfElevators];
        destRooms = new GameObject[numOfFloors];
        passengersClothingColors = new Material[numOfPassengers];
        for (int flo = 0; flo < numOfElevators; flo++)
        {
            floors[flo] = new GameObject[numOfFloors];
        }
        BuildTower();
        PlacePassengers();
        initCameras();
    }

    /*
     Building domain with the info given from the init parser
     Init Floors, Elevators and Passengers.
     */
    private void BuildTower()
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
                if (j >=0 && j < 9)
                {
                    f.transform.GetChild(5).GetComponent<TMP_Text>().text = singleFloorsNumbers[j+1];
                    f.transform.GetChild(6).GetComponent<TMP_Text>().text = singleFloorsNumbers[j+1];
                }
                else
                {
                    f.transform.GetChild(5).GetComponent<TMP_Text>().text = (j+1).ToString();
                    f.transform.GetChild(6).GetComponent<TMP_Text>().text = (j+1).ToString();
                }
                Floor reachableFloor = GetReachableFloorFromElevator(elevatorsObjects[i], j);
                if (reachableFloor == null)
                {
                    ColorFloorNumberInRed(f);
                }
                floors[i][j] = f;
                InitializeObject(floors[i][j],x,y,z);
                y = y + stepY;
            }

            GameObject c = Instantiate(ceiling);
            ceilings[i] = c;
            InitializeObject(ceilings[i], x, y, z);

            x = x + stepX;
        }

        // initiate desitnation floors parallel to each floor
        float yD = 0f;
        for (int i = 0; i < numOfFloors; i++)
        {
            GameObject d = Instantiate(destRoom);
            if (i >= 0 && i < 9)
            {
                d.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = singleFloorsNumbers[i+1];
            }
            else
            {
                d.transform.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = (i+1).ToString();
            }
            destRooms[i] = d;
            InitializeObject(destRooms[i], x, yD, z);
            yD = yD + stepY;
        }
    }

    /*
     Placing the passengers at their initial positions with the initial properties tied to them
     */
    private void PlacePassengers()
    {
        //generating random colors for passenger's cloths
        GenerateRandomMaterialColors();
        // initiate passengers
        for (int i = 0; i < numOfPassengers; i++)
        {
            GameObject p = Instantiate(passenger);
            passengersObjects[i].PassengerGameObject = p;
            ColorPassengerClothes(p, i);

            System.Random rand = new System.Random();
            float constX = 0f, constY = 0f, constZ = 0f;
            do
            {
                constZ = (float)(rand.NextDouble() * 2.0 + 1.0);
                constX = (float)(stepX * (numOfElevators - 1) / (i + 1));
                constY = (float)(stepY * passengersObjects[i].Departured.Number);
            } while (isPassengerAtPosition(constX, constY, constZ));

            passengersObjects[i].Position = new float[] { constX, constY, constZ };

            InitializeObject(p, constX, constY + 0.05f, constZ);
            initPassengerInfo(p, i);
        }
    }

    /*
     Setting new color to the passenger clothes
     */
    private void ColorPassengerClothes(GameObject p, int i)
    {
        SkinnedMeshRenderer top_renderer = p.gameObject.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer bottom_renderer = p.gameObject.transform.GetChild(2).GetComponent<SkinnedMeshRenderer>();
        ColorTheCloth(top_renderer, i);
        ColorTheCloth(bottom_renderer, i);
        
    }

    /*
     Rendering new material for part of the clothes of the passenger game object
     */
    private void ColorTheCloth(SkinnedMeshRenderer renderer, int i)
    {
        Material[] mats = renderer.materials;
        mats[0] = passengersClothingColors[i];
        renderer.materials = mats;
    }

    /*
      Creating new Materials with random different colors for the passengers.
        This way the viewer can distinguish beween passengers.
     */
    private void GenerateRandomMaterialColors()
    {
        for (int i = 0; i < numOfPassengers; i++)
        {
            Color newColor = UnityEngine.Random.ColorHSV();
            Material newMaterial = new Material(Shader.Find("Standard"));
            newMaterial.SetColor("_Color", newColor);
            passengersClothingColors[i] = newMaterial;
        }
    }


    /*
        Returning the reachble floor by number of a specific elevator object
     */
    private Floor GetReachableFloorFromElevator(Elevator e, int numOfFloor)
    {
        foreach (Floor f in e.ReachableFloors)
        {
            if (f.Number == numOfFloor)
            {
                return f;
            }
        }
        return null;
    }

    /*
        Coloring in red the label of an elevator 
     */
    private void ColorFloorNumberInRed(GameObject floor)
    {
        floor.transform.GetChild(5).GetComponent<TMP_Text>().color = Color.red;
        floor.transform.GetChild(6).GetComponent<TMP_Text>().color = Color.red;
    }

    private void initPassengerInfo(GameObject p, int i)
    {
        Passenger passenger = passengersObjects[i];
        p.transform.GetChild(11).GetComponent<TMP_Text>().text = "Name: " + passenger.Name;
        p.transform.GetChild(11).GetComponent<TMP_Text>().color = passengersClothingColors[i].color;

        p.transform.GetChild(12).GetComponent<TMP_Text>().text = "Dest: F" + passenger.Destination.Number;
        p.transform.GetChild(12).GetComponent<TMP_Text>().color = passengersClothingColors[i].color;
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

    /*
     After each PassengerAction (Board/Leave) updating the capacity of the elevator the passenger has interact with
     */
    private void UpdateElevatorCapacity(string eName, int currentCapacity)
    {
        foreach (Elevator e in elevatorsObjects)
        {
            if (e.Name.Equals(eName))
            {
                e.CurrentNumberOfPassengers = currentCapacity;

                if (e.CurrentNumberOfPassengers > e.TotalCapacity)
                {
                    e.ElevatorGameObject.transform.GetChild(4).GetComponent<TMP_Text>().color = Color.red;
                }
                else
                {
                    e.ElevatorGameObject.transform.GetChild(4).GetComponent<TMP_Text>().color = Color.green;
                }
                e.ElevatorGameObject.transform.GetChild(4).GetComponent<TMP_Text>().text = "Capacity: " + e.CurrentNumberOfPassengers + "/" + e.TotalCapacity;
            }
        }
    }

    /*
    After each MoveAction of an elevator updating the cost 
    */
    private void updateElevatorCost(string eName, int cost)
    {
        foreach (Elevator e in elevatorsObjects)
        {
            if (e.Name.Equals(eName))
            {
                e.CurrentCost += cost;
                e.ElevatorGameObject.transform.GetChild(5).GetComponent<TMP_Text>().text = "Cost: " + e.CurrentCost;
            }
        }
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
        bool isGreaterThan10Floors = false;
        cameraQueue = new Queue<Camera[]>();
        activeCameras = new Queue<Camera>();
        mainCameras = new Camera[1];
        if (emssParser.NumOfFloors > 10)
        {
            view1Cameras = new Camera[4];
            isGreaterThan10Floors = true;
        }
        else
        {
            view1Cameras = new Camera[3];
        }
        view2Cameras = new Camera[2];
        view3Cameras = new Camera[3];

        activeCameraObject = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        Canvas spaceCanvas = GameObject.FindGameObjectWithTag("SpaceCanvas").GetComponent<Canvas>();
        spaceCanvas.worldCamera = activeCameraObject;
        mainCameras[0] = activeCameraObject;
        activeCameras.Enqueue(activeCameraObject);
        activeCameraObject.gameObject.SetActive(true);

        //Camera rightMainCameraObj = GameObject.FindGameObjectWithTag("MainRightCamera").GetComponent<Camera>();
        //spaceCanvas = GameObject.FindGameObjectWithTag("RightSpaceCanvas").GetComponent<Canvas>() as Canvas;
        //spaceCanvas.worldCamera = rightMainCameraObj;
        //rightMainCameraObj.rect = new Rect(0.5f, 0, 0.5f, 1);

        InitView1Cameras(isGreaterThan10Floors);
        InitView2Cameras();
        InitView3Cameras(isGreaterThan10Floors);

        cameraQueue.Enqueue(view1Cameras);
        cameraQueue.Enqueue(view3Cameras);
        cameraQueue.Enqueue(view2Cameras);
        cameraQueue.Enqueue(mainCameras);

        //Camera sideCameraRight = GameObject.FindGameObjectWithTag("SideCameraRight").GetComponent<Camera>();
        //sideCameraRight.gameObject.SetActive(false);
        //Camera sideCameraLeft = GameObject.FindGameObjectWithTag("SideCameraLeft").GetComponent<Camera>();
        //sideCameraLeft.gameObject.SetActive(false);

        //cameraQueue.Enqueue(sideCameraRight);
        //cameraQueue.Enqueue(sideCameraLeft);

        foreach (Elevator elev in elevatorsObjects)
        {
            GameObject go = elev.ElevatorGameObject;
            Camera current = go.GetComponentInChildren<Camera>();
            current.gameObject.SetActive(false);
        }

        for (int i = 0; i < 4; i++)
        {
            switchCameras();
        }
    }

    /*
    Initialize View3 cameras
    */
    private void InitView3Cameras(bool isGreaterThan10Floors)
    {
        Camera zoomCamera = GameObject.FindGameObjectWithTag("View3ZoomCamera").GetComponent<Camera>();
        zoomCamera.rect = new Rect(0, 0, 0.4f, 0.5f);
        zoomCamera.gameObject.SetActive(false);

        Camera worldCamera = GameObject.FindGameObjectWithTag("View3WorldCamera").GetComponent<Camera>();
        worldCamera.rect = new Rect(0, 0, 1, 1);
        if (isGreaterThan10Floors)
        {
            worldCamera.transform.localPosition = new Vector3(25f, 42f, 56f);
        }
        else
        {
            worldCamera.transform.localPosition = new Vector3(25f, 22f, 36f);
        }
        worldCamera.gameObject.SetActive(false);
        
        Camera nullCamera = GameObject.FindGameObjectWithTag("View3NullCamera").GetComponent<Camera>();
        nullCamera.rect = new Rect(0, 0.5f, 0.4f, 1);
        nullCamera.gameObject.SetActive(false);

        view3Cameras[0] = worldCamera;
        view3Cameras[1] = zoomCamera;
        view3Cameras[2] = nullCamera;
    }

    /*
     Initialize View2 cameras
     */
    private void InitView2Cameras()
    {
        Camera leftCamera = GameObject.FindGameObjectWithTag("View2LeftCamera").GetComponent<Camera>();
        leftCamera.rect = new Rect(0, 0, 0.5f, 1);
        leftCamera.gameObject.SetActive(false);

        Camera rightCamera = GameObject.FindGameObjectWithTag("View2RightCamera").GetComponent<Camera>();
        rightCamera.rect = new Rect(0.5f, 0, 0.5f, 1);
        rightCamera.gameObject.SetActive(false);

        view2Cameras[0] = leftCamera;
        view2Cameras[1] = rightCamera;
    }

    /*
     Initialize View1 cameras
     */
    private void InitView1Cameras(bool isGreaterThan10Floors)
    {

        Camera upperCamera = GameObject.FindGameObjectWithTag("View1UpperCamera").GetComponent<Camera>();
        upperCamera.rect = new Rect(0, 0.6f, 1, 1);
        upperCamera.gameObject.SetActive(false);

        Camera leftCamera = GameObject.FindGameObjectWithTag("View1LeftCamera").GetComponent<Camera>();
        Camera rightCamera = GameObject.FindGameObjectWithTag("View1RightCamera").GetComponent<Camera>();

        if (isGreaterThan10Floors)
        {
            Camera middleCamera = GameObject.FindGameObjectWithTag("View1MiddleCamera").GetComponent<Camera>();
            middleCamera.rect = new Rect(0.333f, 0, 0.333f, 0.6f);
            view1Cameras[2] = middleCamera;
            middleCamera.transform.localPosition= new Vector3(5.8f, 35f, 18f);
            middleCamera.gameObject.SetActive(false);

            leftCamera.rect = new Rect(0, 0, 0.333f, 0.6f);
            leftCamera.transform.localPosition = new Vector3(5.8f, 11f, 18f);
            leftCamera.gameObject.SetActive(false);

            rightCamera.rect = new Rect(0.666f, 0, 0.333f, 0.6f);
            rightCamera.transform.localPosition = new Vector3(5.8f, 55f, 18f);
            rightCamera.gameObject.SetActive(false);

            view1Cameras[0] = upperCamera;
            view1Cameras[1] = leftCamera;
            view1Cameras[2] = middleCamera;
            view1Cameras[3] = rightCamera;
        }
        else
        {
            leftCamera.rect = new Rect(0, 0, 0.5f, 0.6f);
            leftCamera.transform.localPosition = new Vector3(5.8f, 7f, 12f);
            leftCamera.gameObject.SetActive(false);

            rightCamera.rect = new Rect(0.5f, 0, 0.5f, 0.6f);
            rightCamera.transform.localPosition = new Vector3(5.8f, 27f, 12f);
            rightCamera.gameObject.SetActive(false);

            view1Cameras[0] = upperCamera;
            view1Cameras[1] = leftCamera;
            view1Cameras[2] = rightCamera;
        }
    }

    /*
     Switching cameras by right click mouse command
     */
    public void switchCameras()
    {
        while (activeCameras.Count != 0)
        {
            Camera toRemove = activeCameras.Dequeue();
            toRemove.gameObject.SetActive(false);
        }
        Camera[] camerasToBeActivated = cameraQueue.Dequeue();
        bool firstToBeOut = true;
        foreach(Camera c in camerasToBeActivated)
        {
            c.gameObject.SetActive(true);
            activeCameras.Enqueue(c);
            if (firstToBeOut)
            {
                activeCameraObject = c;
                firstToBeOut = false;
            }
        }
        cameraQueue.Enqueue(camerasToBeActivated);
    }

    /*
     This function call every time there is a movement of passengers or elevetors.
    The View1 Upper camera is moving through floors in a parallel position.
     */
    private void MoveView1UpperCamera()
    {
        Vector3 localPosition = view1Cameras[0].GetComponent<Transform>().position;
        view1Cameras[0].GetComponent<Transform>().position = Vector3.MoveTowards(localPosition, localPosition + new Vector3(0, currentDirectionY * stepY, 0), (simulationSpeed / elevatorSpeed));
    }

    /*
     PLacing the View1 Upper camera in initial position in case the first action is of a passenger and not an elevator
     */
    private void PlaceView1UpperCameraInInitFloor(int thisFloorNumber)
    {
        view1Cameras[0].transform.localPosition = new Vector3(6f, thisFloorNumber * stepY, 2.5f);
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
        emssPlanParser = new ParsePlanElevator(planPath);
        actions = emssPlanParser.Actions;
    }

    /*
    Simulation tranformation to continuus running
    */
    public void playSimMode()
    {
        gameHold = false;
        stepFlag = false;
        stepBackwardFlag = false;
        firstTimeBackwardPressed = true;
    }

    /*
     Simulation transformation to forward step-by-step mode by demand
     */
    public void stepByStepMode()
    {
        gameHold = true;
        stepFlag = true;
        stepBackwardFlag = false;
        firstTimeBackwardPressed = true;
    }

    /*
     Simulation transformation to backward step-by-step mode by demand
     */
    public void stepBackMode()
    {
        if (firstTimeBackwardPressed)
        {
            actionIsOnGoing = false;
            if (actionCounter > 0) { actionCounter -= 1; }
            firstTimeBackwardPressed = false;
        }

        gameHold = true;
        stepFlag = false;
        stepBackwardFlag = true;
        
        if (actionCounter > 0)
        {
            actionCounter -= 1;
        }
    }

    public void speedUp()
    {
        if (simulationSpeed - 4 > 0)
        {
            simulationSpeed = simulationSpeed - 4;
            _std -= 0.00864285f;
        }
    }

    public void speedDown()
    {
        if (simulationSpeed + 4 < 100)
        {
            simulationSpeed = simulationSpeed + 4;
            _std += 0.00864285f;
        }
    }

    // Run simulation again after restart all needed game objects
    public void RunSamePlanAgain()
    {
        InitActionCounter();
        foreach(Camera[] carr in cameraQueue)
        {
            foreach(Camera c in carr)
            {
                c.gameObject.SetActive(true);
            }
        }

    }

    public void InitActionCounter()
    {
        actionCounter = 0;

        stepX = 7;
        stepY = 4;

        x = 0f;
        y = 0f;
        z = 0f;

        actionIsOnGoing = false;
        elevatorActionFlag = false;
        passengerActionFlag = false;
    }

    public void DeleteMap()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            DestroyObjects(floors[i]);
        }
        DestroyObjects(elevators);
        DestroyObjects(passengers);
        DestroyObjects(ceilings);
        DestroyObjects(destRooms);

        currentAction = null;
        Destroy(currentElevatorObject.ElevatorGameObject);
        Destroy(currentPassengerObject.PassengerGameObject);
        currentElevatorObject = null;
        currentPassengerObject = null;

        foreach (Elevator e in elevatorsObjects)
        {
            Destroy(e.ElevatorGameObject);
        }
        foreach (Passenger p in passengersObjects)
        {
            Destroy(p.PassengerGameObject);
        }
        elevatorsObjects.Clear();
        floorObjects.Clear();
        passengersObjects.Clear();
    }

    private void DestroyObjects(GameObject[] arr)
    {
        foreach (GameObject o in arr)
            Destroy(o);
    }

}
