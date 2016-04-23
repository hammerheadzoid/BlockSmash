using UnityEngine;
using System.Collections;

public class SingleAlarmPrefab : MonoBehaviour {

    // Use this for initialization. Create the playerprefs here. 
    void Start () {

        PlayerPrefs.SetInt("unixTime", 123456);                         // Set time variable
        PlayerPrefs.SetString("alarmName", "alarmName");                // Set the alarm name
        PlayerPrefs.SetInt("RepeatWeekly", 1);                          // Repeat Weekly Yes/No
        PlayerPrefs.SetInt("sun", 0);                                   // Set sun as checked
        PlayerPrefs.SetInt("mon", 0);                                   // Set mon as checked
        PlayerPrefs.SetInt("tue", 0);                                   // Set tue as checked
        PlayerPrefs.SetInt("wed", 0);                                   // Set wed as checked
        PlayerPrefs.SetInt("thr", 0);                                   // Set thr as checked
        PlayerPrefs.SetInt("fri", 0);                                   // Set fri as checked
        PlayerPrefs.SetInt("sat", 0);                                   // Set sat as checked
        PlayerPrefs.SetInt("volume", 50);                               // Set volume (0 - 99)
        PlayerPrefs.SetString("alarmTone", "alarmTone");                // Set the alarm name
        PlayerPrefs.SetString("bgImageLocation", "bgImageLocation");    // Set the alarm name


    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EditAlarmButton() {

        Debug.Log("A button was clicked...");

    }
}
