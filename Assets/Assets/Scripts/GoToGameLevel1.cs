using UnityEngine;
using System.Collections;

public class GoToGameLevel1 : MonoBehaviour {

	public int alarm_counter = 0;
	public AlarmClass[] alarmarray=new AlarmClass[10];
	
	void Start () {
		for (int i=0; i<10; i++) {
			alarmarray[i]=new AlarmClass();
		}
	}

	public void Main_Screen () {
		Debug.Log("Going into Main_Screen");
		Application.LoadLevel("Main_Screen");	// Main Menu Screen
	}
	

	public void Main_Menu () {
		Debug.Log("Going into Main_Menu");
		Application.LoadLevel("Main_Menu");	// Main Menu Screen
	}
	
	public void Edit_Alarm (int alarm_id) {
		Debug.Log("Going into Edit_Alarm");
		PlayerPrefs.SetInt ("PlayerPrefs_current_alarm",alarm_id);//temporary
		//Debug.Log(alarmarray[alarm_id].time);
		Application.LoadLevel("Edit_Alarm");	// Alarm Set Screen
	}

	public void Set_Alarm () {
		int alarm_id = PlayerPrefs.GetInt ("PlayerPrefs_current_alarm");
		int tmpalarm=PlayerPrefs.GetInt ("PlayerPrefs_tmpalarm");//temporary

        PlayerPrefs.SetInt("PlayerPrefs_alarm_1", tmpalarm);



        alarmarray[alarm_id].time = tmpalarm;
		Debug.Log(alarmarray[alarm_id].time);
        Debug.Log("In main screen?");
        Application.LoadLevel("Main_Screen");	// Create the alarm and return to main screen
	}
}
