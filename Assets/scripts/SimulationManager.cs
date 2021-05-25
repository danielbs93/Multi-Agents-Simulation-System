using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SFB;



public class SimulationManager : MonoBehaviour
{
    //Private Fields
    public Canvas loadScreen;
    public Canvas endingScreen;
    public Canvas menu;
    public Canvas menuOptions;
    private Canvas messageDialog;
    // public SmartFileExplorer sfe = new SmartFileExplorer();

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
                string[] vsFolder = StandaloneFileBrowser.OpenFolderPanel("Open File", "", false);
                initPath = vsFolder[0];
                // initPath = EditorUtility.OpenFolderPanel("Initial file", "", "");
                init_ph.text = initPath;
                break;

            case "plan":
                string[] vsFile = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
                planPath = vsFile[0];
                // planPath = EditorUtility.OpenFilePanel("Plan file", "", "");
                plan_ph.text = planPath;
                break;
            default:
                break;
        }
    }

public void runSimulationByDomain()
    {
        //TEST ONLY ERASE AFTER THAT
        // initPath = "C:\\Users\\erant\\Desktop\\p09";
        // planPath = "C:\\Users\\erant\\Desktop\\p09converted.pddl";
        //domain = 1;
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
        emssScript.planParser(); 
        emssScript.buildMap();
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
        else if (domain == 1)
        {
            if (emssScript != null)
            {
                emssScript.stepByStepMode();
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
            emssScript.playSimMode();
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
            emssScript.stepByStepMode();
        }
    }

    /*
     When step backward is pressed the simulation transfer its state to backward step mode by demand
     */
    public void stepBackMode()
    {
        if (domain == 0)
        {
            sokobanScript.stepBackMode();
        }
        else
        {
            emssScript.stepBackMode();
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
        else if (domain == 1)
        {
            if (emssScript != null)
            {
                emssScript.DeleteMap();
            }
            if (withGameManagerObjects)
            {
                // init fields
                emssScript.InitActionCounter();
                emssScript.playSimMode();

                // Unity objects activeness
                domainManager.gameObject.SetActive(false);
                menu.gameObject.SetActive(false);
            }
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
        else if (domain == 1)
        {
            if (emssScript != null)
            {
                emssScript.RunSamePlanAgain();
                emssScript.playSimMode();
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

    public void speedUp()
    {
        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                this.sokobanScript.speedUp();
            }
        }
        else
        {
            if (emssScript != null)
            {
                emssScript.speedUp();
            }
        }
    }

    public void speedDown()
    {
        if (domain == 0)
        {
            if (sokobanScript != null)
            {
                this.sokobanScript.speedDown();
            }
        }
        else
        {
            if (emssScript != null)
            {
                emssScript.speedDown();
            }
        }
    }


}
