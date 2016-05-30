using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SingleAlarmPrefab : MonoBehaviour {

    public int sunCheck02 = 1;
    public Button sunButton;
  

    /*
    This class will do and contain the following:

        1)  Contain a list of all the variables needed to save all the details of the alarm. These are:
            *   Unixtime
            *   Alarm name
            *   Repeat weekly (yes/no)
            *   sun (on/off)
            *   mon (on/off)
            *   tue (on/off)
            *   wed (on/off)
            *   thr (on/off)
            *   fri (on/off)
            *   sat (on/off)
            *   volume
            *   alarm tone (location of tone)
            *   background image (image background)
        2)  Here is the time line of how things will happen.
            *   First from the Main Menu scene the "Create New" button is clicked. 
            *   We are then taken to the Edit Alarm scene.
            *   At the Edit Alarm scene the details contained in point 1) above will be recoreded simply as playerpref settings. 
            *   When we are happy with the details that we have chosen then we click on the "Set Alarm" button. 
            *   Clicking on the set alarm button takes us to the clock screen or the "Main Screen" scene (this is just a choice as we could have gone to Main Menu again)
            *   Clicking on the set alarm button also creates the prefab for the single alarm just created.
            *   The prefab just created or "instantiated" will now show up in the "Main Menu" scene. 
        3)  There has to be a way to figure out which instance will be on top. This will be done using a unix timestamp
     */

    // Use this for initialization. Create the playerprefs here. 
    void Start () {

        PlayerPrefs.SetInt("unixTime", 123456);                         // Set time variable
        PlayerPrefs.SetString("alarmName", "alarmName");                // Set the alarm name
        PlayerPrefs.SetInt("RepeatWeekly", 1);                          // Repeat Weekly Yes = 1 / No = 2
        PlayerPrefs.SetInt("sun", 0);                                   // Set sun as unchecked
        PlayerPrefs.SetInt("mon", 0);                                   // Set mon as unchecked
        PlayerPrefs.SetInt("tue", 0);                                   // Set tue as unchecked
        PlayerPrefs.SetInt("wed", 0);                                   // Set wed as unchecked
        PlayerPrefs.SetInt("thr", 0);                                   // Set thr as unchecked
        PlayerPrefs.SetInt("fri", 0);                                   // Set fri as unchecked
        PlayerPrefs.SetInt("sat", 0);                                   // Set sat as unchecked
        PlayerPrefs.SetInt("volume", 50);                               // Set volume (0 - 99)
        PlayerPrefs.SetString("alarmTone", "alarmTone");                // Set the alarm name
        PlayerPrefs.SetString("bgImageLocation", "bgImageLocation");    // Set the alarm name
    }
	
	// Update is called once per frame
	void Update () {

 
    }

    public void checkWeekDay()
    {
        if (sunCheck02 != 1 && sunCheck02 != 0)
        {
            sunCheck02 = 1;
        }

        if (sunCheck02 == 0)
        {
            sunCheck02 = 1;
            ColorBlock colorblock = ColorBlock.defaultColorBlock;
            colorblock.normalColor = new Color(1, 0, 0);
            sunButton.GetComponent<Button>().colors = colorblock;
            //sunButton.colors.normalColor = Color.blue;
            Debug.Log("sunCheck02 is now 1");
        }
        else if (sunCheck02 == 1)
        {
            sunCheck02 = 0;
            Debug.Log("sunCheck02 is now 0");
        }
    }
}
