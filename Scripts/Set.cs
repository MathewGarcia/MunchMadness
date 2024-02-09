using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Set : MonoBehaviour
{

    public Material easyMaterial;
    public Material mediumMaterial;
    public Material hardMaterial;

    private Renderer couchRenderer;


    // Start is called before the first frame update
    void Start()
    {
        couchRenderer = GetComponent<Renderer>();
        GetComponent<Renderer>().material = easyMaterial;

        // Subscribe to the OnDifficultyChange event
        GameManagerScript.OnDifficultyChange += HandleDifficultyChange;


    }

    // Update is called once per frame
    void Update()
    {
    }


    private void HandleDifficultyChange(GameManagerScript.DifficultyState newState)
    {

       // Debug.Log(gameObject.name + " is active: " + gameObject.activeSelf);
        //Debug.Log("Couch script is active: " + enabled);

        switch (newState)
        {
            case GameManagerScript.DifficultyState.Easy:
                GetComponent<Renderer>().material = easyMaterial;
                //couchRenderer.material = easyMaterial;
                //Debug.Log("couch easy");
                break;
            case GameManagerScript.DifficultyState.Medium:
                //couchRenderer.material = mediumMaterial;
                GetComponent<Renderer>().material = mediumMaterial;
                //Debug.Log("couch med");
                break;
            case GameManagerScript.DifficultyState.Hard:
                //couchRenderer.material = hardMaterial;
                GetComponent<Renderer>().material = hardMaterial;
                //Debug.Log("couch is hard");
                break;
        }

       // Debug.Log("Current material: " + couchRenderer.material.name);

    }


}
