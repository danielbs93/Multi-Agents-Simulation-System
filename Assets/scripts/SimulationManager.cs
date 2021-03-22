using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SimulationManager : MonoBehaviour
{
    //Private Fields
    public Canvas loadScreen;
    public Canvas endingScreen;
    public Canvas menu;
    public Canvas menuOptions;
    private Canvas messageDialog;


    public GameObject domainManager;
    private SokobanManager sokobanScript;
    private EMSSManager emssScript;
    //TODO-EMSS fields

    private int domain; // 0 - means Sokoban, 1 - means EMSS

    private Button init_btn;
    // private Button preconditions_effects_btn;
    private Button plan_btn;
    private Button run_sim_btn;

    private InputField init_ph;
    // private InputField preconditions_effects_ph;
    private InputField  plan_ph;

    private string initPath;
    // private string preconditionEffectsPath;
    private string planPath;



    // Start is called before the first frame update
    void Start()
    {
        print("here");
        loadScreen.gameObject.SetActive(false);
        endingScreen.gameObject.SetActive(false);
        menu.gameObject.SetActive(false);

        messageDialog = GameObject.FindGameObjectWithTag("MessageDialog").GetComponent<Canvas>();
        messageDialog.gameObject.SetActive(false);

        getDomainChoice(SceneSimManager.domain);


        //Domains managers 
        domainManager.gameObject.SetActive(false);
        //TODO-EMSS manager false activation

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getDomainChoice(int i)
    {
        domain = i;
        loadScreen.gameObject.SetActive(true);
        activateLoadScreenComponents();
    }

    private void activateLoadScreenComponents()
    {
        // Loading Screen Properties
        init_btn = GameObject.FindGameObjectWithTag("initBrowse").GetComponent<Button>();
        plan_btn = GameObject.FindGameObjectWithTag("planBrowse").GetComponent<Button>();
        run_sim_btn = GameObject.FindGameObjectWithTag("runSim").GetComponent<Button>();

        init_ph = GameObject.FindGameObjectWithTag("initPH").GetComponent<InputField>();
        plan_ph = GameObject.FindGameObjectWithTag("planPH").GetComponent<InputField>();

    }

    public void openFileExplorer(string whichPath)
    {
        switch (whichPath)
        {
            case "init":
                initPath = EditorUtility.OpenFolderPanel("Initial file", "", "");
                init_ph.text = initPath;
                break;
            // case "pre":
            //     preconditionEffectsPath = EditorUtility.OpenFilePanel("Precondition & Effects file", "", "");
            //     preconditions_effects_ph.text = preconditionEffectsPath;
            //     break;
            case "plan":
                planPath = EditorUtility.OpenFilePanel("Plan file", "", "");
                plan_ph.text = planPath;
                break;
            default:
                break;
        }
    }

public void runSimulationByDomain()
    {
        //TEST ONLY ERASE AFTER THAT
        //initPath = "C:\\Users\\USER\\Desktop\\New folder\\Examples\\p04";
        //planPath = "C:\\Users\\USER\\Desktop\\New folder\\Examples\\p04\\problem-slow0-0.pddl";
        domain = 1;
        if (validateInputeFields())
        {
            menu.gameObject.SetActive(true);
            menuOptions.gameObject.SetActive(false);
            if (domain == 0)
            {
                runSokobanSimulation();
            }
            else
            {
                runEMSSsimulation();
            }
        }
    }

    /*
        
        If one of the paths is not initalized in the load screen then display error message dialog

         */
    private bool validateInputeFields()
    {
        if (initPath == null || planPath == null)
        {
            messageDialog.gameObject.SetActive(true);
            return false;
        }
        return true;
    }

    //Button for closing the error message dialog
    public void realeseMessageDialog()
    {
        messageDialog.gameObject.SetActive(false);
    }



    // Responsibility transformation by the assigned Sokoban domain
    private void runSokobanSimulation()
    {
        loadScreen.gameObject.SetActive(false);
        domainManager.gameObject.SetActive(true);
        sokobanScript = (SokobanManager)domainManager.GetComponent(typeof(SokobanManager));
        sokobanScript.setSokobanPaths(initPath, planPath);
        sokobanScript.initParser();
        sokobanScript.planParser();
        sokobanScript.initCamerasQueue();
        sokobanScript.setmap();
    }

    // Responsibility transformation by the assigned EMSS domain
    public void runEMSSsimulation()
    {
        loadScreen.gameObject.SetActive(false);
        domainManager.gameObject.SetActive(true);
        emssScript = (EMSSManager)domainManager.GetComponent(typeof(EMSSManager));
        emssScript.setEmssPaths(initPath, planPath);
        emssScript.initParser();
        emssScript.planParser(); // TODO
        emssScript.buildMap();
        emssScript.MapInitialized = true;
    }

    //Menu options buttons canvas appear
    public void menuPressed()
    {
        menuOptions.gameObject.SetActive(true);
        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                sokobanScript.stepByStepMode();
            }
        }
    }

    /**
     * Menu item was selected:
     *  0 - run the simulation again with the same plan and init files
     *  1 - choosing another plan and init file in the same domain
     *  2 - choosing another domain 
     *  3 - back to simulation
     */
    public void menuOptionChosed(int i)
    {
        if (i == 0)
        {
            menuRunSimulationAgain();
        }
        else if (i == 1)
        {
            menuLoadNewPlan();
        }
        else if (i ==2)
        {
            menuLoadNewDomain();
        }
        else
        {
            menuOptions.gameObject.SetActive(false);
            if (domain == 0)
            {
                if (sokobanScript != null)
                {
                    sokobanScript.playSimMode();
                }
            }
        }
        menuOptions.gameObject.SetActive(false);
    }

    /*
     When play_sim_btn is pressed the simulation tranfer its state to continuus running
     */
    public void playSimMode()
    {
        if (domain == 0)
        {
            sokobanScript.playSimMode();
        }
        else
        {
            
        }
    }

    /*
     When stp_by_stp is pressed the simulation transfer its state to step mode by demand
     */
    public void stepByStepMode()
    {
        if (domain == 0)
        {
            sokobanScript.stepByStepMode();
        }
        else
        {

        }
    }

    // Deleting objects from the current domain and restart the states for the simulation again
    private void DeactivateDomain(bool withGameManagerObjects)
    {
        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                sokobanScript.deleteMap();
            }
            if (withGameManagerObjects)
            {
                // init fields
                sokobanScript.initAgentsActions();
                sokobanScript.initActionCounter();
                sokobanScript.playSimMode();

                // Unity objects activeness
                domainManager.gameObject.SetActive(false);
                menu.gameObject.SetActive(false);
            }
        }
        else
        {
            //TODO-EMSS
        }
    }

    /**
     * Menu in game option - run simulation again 
     * Runing the simulation again with same init file and the same plan file
     */
    private void menuRunSimulationAgain()
    {
        DeactivateDomain(false);
        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                sokobanScript.initActionCounter();
                sokobanScript.playSimMode();
            }
        }
        runSimulationByDomain();
    }

    /**
     * Menu in game option- load new plan
     * Returning to Load Screen
     */
    private void menuLoadNewPlan()
    {
        DeactivateDomain(true);
        loadScreen.gameObject.SetActive(true);
    }

    /**
     * Menu in game option- load new domain
     * Returning to Domain Screen
     */
    private void menuLoadNewDomain()
    {
        DeactivateDomain(true);
        SceneManager.LoadScene("SceneSimManager");
        
    }

    public void speedUp()    {        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                this.sokobanScript.speedUp();
            }
        }    }    public void speedDown()    {        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                this.sokobanScript.speedDown();
            }
        }    }


}
