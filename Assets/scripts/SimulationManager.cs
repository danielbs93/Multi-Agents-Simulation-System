using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SimulationManager : MonoBehaviour
{
    //Private Fields
    public Canvas loadScreen;
    public Canvas domainScreen;
    public Canvas endingScreen;
    public Canvas menu;
    public Canvas menuOptions;

    public GameObject sokobanManager;
    private SokobanManager sokobanScript;
    //TODO-EMSS fields

    private int domain; // 0 - means Sokoban, 1 - means EMSS

    private Button init_btn;
    private Button preconditions_effects_btn;
    private Button plan_btn;
    private Button run_sim_btn;

    private InputField init_ph;
    private InputField preconditions_effects_ph;
    private InputField  plan_ph;

    private string initPath;
    private string preconditionEffectsPath;
    private string planPath;



    // Start is called before the first frame update
    void Start()
    {
        loadScreen.gameObject.SetActive(false);
        endingScreen.gameObject.SetActive(false);
        domainScreen.gameObject.SetActive(true);
        menu.gameObject.SetActive(false);

        //Domains managers 
        sokobanManager.gameObject.SetActive(false);
        //TODO-EMSS manager false activation

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void getDomainChoice(int i)
    {
        domain = i;
        domainScreen.gameObject.SetActive(false);
        loadScreen.gameObject.SetActive(true);
        activateLoadScreenComponents();
    }

    private void activateLoadScreenComponents()
    {
        init_btn = GameObject.FindGameObjectWithTag("initBrowse").GetComponent<Button>();
        preconditions_effects_btn = GameObject.FindGameObjectWithTag("precEffBrowse").GetComponent<Button>();
        plan_btn = GameObject.FindGameObjectWithTag("planBrowse").GetComponent<Button>();
        run_sim_btn = GameObject.FindGameObjectWithTag("runSim").GetComponent<Button>();

        init_ph = GameObject.FindGameObjectWithTag("initPH").GetComponent<InputField>();
        preconditions_effects_ph = GameObject.FindGameObjectWithTag("preEffPH").GetComponent<InputField>();
        plan_ph = GameObject.FindGameObjectWithTag("planPH").GetComponent<InputField>();
    }

    public void openFileExplorer(string whichPath)
    {
        switch (whichPath)
        {
            case "init":
                initPath = EditorUtility.OpenFilePanel("Initial file", "", "");
                init_ph.text = initPath;
                break;
            case "pre":
                preconditionEffectsPath = EditorUtility.OpenFilePanel("Precondition & Effects file", "", "");
                preconditions_effects_ph.text = preconditionEffectsPath;
                break;
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

    // Responsibility transformation by the assigned Sokoban domain
    private void runSokobanSimulation()
    {
        loadScreen.gameObject.SetActive(false);
        sokobanManager.gameObject.SetActive(true);
        sokobanScript = (SokobanManager)sokobanManager.GetComponent(typeof(SokobanManager));
        sokobanScript.setSokobanPaths(initPath, preconditionEffectsPath, planPath);
        sokobanScript.initParser();
        sokobanScript.planParser();
        sokobanScript.initCamerasQueue();
        sokobanScript.setmap();
    }

    // Responsibility transformation by the assigned EMSS domain
    public void runEMSSsimulation()
    {
        //TODO
    }

    //Menu options buttons canvas appear
    public void menuPressed()
    {
        menuOptions.gameObject.SetActive(true);
    }

    /**
     * Menu item was selected:
     *  0 - run the simulation again with the same plan and init files
     *  1 - choosing another plan and init file in the same domain
     *  2 - choosing another domain 
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
        }
        menuOptions.gameObject.SetActive(false);
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
                sokobanManager.gameObject.SetActive(false);
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
        domainScreen.gameObject.SetActive(true);
    }

}
