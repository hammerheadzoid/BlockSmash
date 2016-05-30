using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetAlarm : MonoBehaviour {

    public GameObject alarmPanel02;
    public Button sunButton;

    private int panel1Alarm = 0;
    private int alarmtime = 0;
    private string alarmtimeString = null;
    public Text timeAlarm02 = null;
    
    
	// Use this for initialization
	void Start () {
        panel1Alarm = PlayerPrefs.GetInt("unixTime"); // What is this and why is it here!
        alarmtime =  PlayerPrefs.GetInt("PlayerPrefs_tmpalarm"); // The time from edit alarm
        alarmtimeString = alarmtime.ToString();     // Chris' alarmtime

        //sunButton.onClick.AddListener(checkIfWeekDayIsSelected);//adds a listener for when you click the button

        // if a time is 09:30 for example... the 0 will be left out, so i am adding the zero in here
        if (alarmtimeString.Length < 4)
        {
            Debug.Log("Less than four, it is " + alarmtimeString.Length);
            alarmtimeString = string.Concat("0", alarmtimeString);
        }
        else
        {
            Debug.Log("Four or more, it is " + alarmtimeString.Length);
        }
        timeAlarm02.text = alarmtimeString;
    }
	
	// Update is called once per frame
	void Update () {

	   
	}

    
}
