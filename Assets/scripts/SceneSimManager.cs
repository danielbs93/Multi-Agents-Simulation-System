using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSimManager : MonoBehaviour
{
    public Canvas domainScreen;

    public static int domain;

    // Start is called before the first frame update
    void Start()
    {
        domainScreen.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadSokobanDomain()
    {
        domain = 0;
        domainScreen.gameObject.SetActive(false);
        SceneManager.LoadScene("Sokoban Domain");
    }

    public void loadEmssDomain()
    {
        domain = 1;
        domainScreen.gameObject.SetActive(false);
        SceneManager.LoadScene("EMSS Domain");
    }
}
