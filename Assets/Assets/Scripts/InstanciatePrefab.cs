using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InstanciatePrefab : MonoBehaviour {


    public GameObject alarmInstance;
    public GameObject panel;
    public GameObject button;
    public Transform markerPoint;
    //public GameObject canvas;
    
    private int counter = 0;

    // Use this for initialization
    void Start () {
        

    }

    // Update is called once per frame
    void Update () {

        if (Input.GetKeyUp("l"))
        {
            if (counter < 10)
            {
                // Instantiate the gameobject that contains the script HelloWorld
                Instantiate(alarmInstance);
                Debug.Log("You clicked L");
                //counter++;

                // Instantiate canvas and button
                GameObject newCanvas = Instantiate(panel, markerPoint.position, markerPoint.rotation) as GameObject;
                //myCanvas = FindGameObjectsWithTag("parentCanvas");
                //myCanvas.transform.SetParent(Canvas);
                GameObject newButton = Instantiate(button) as GameObject;
                newButton.transform.SetParent(newCanvas.transform, false);
                counter++;
                Debug.Log("Counter is " + counter);
                
            }
            else
            {
                Debug.Log("You have reached the max number of instances");
            }
        }
    }
}
