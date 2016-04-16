using UnityEngine;
using System.Collections;

public class SingleAlarm : MonoBehaviour {

    /*
    This class will do and contain the following:

        1)  Contain a list of all the variables needed to save all the details of the alarm. These are:
            *   hour 1
            *   hour 2
            *   minute 1
            *   minute 2
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
        3)  There has to be a way to figure out which instance will be on top. Is it meerly the next time. 
            What if one time is 1500 hours today. That is the only instance I have. So at 3pm the alarm is going to go off. 
            However, if I create a second alarm at 1400 hours, that alarm will go off at 2pm today. Now this alarm instance is designated number 1 and 3pm is number 2.
            But what if the alarm is 1300 hours for 2 days time. So now, there are two things to consider. And there always was two things to consider. The day and 
            the time of day. So a solution for this is if the alarm is set for today then it will be assigned a '0'. 
            This way we have the following:
            today       '0'
            day two     '1'
            day three   '2'
            day four    '3'
            day five    '4'
            day six     '5'
            day seven   '6'
            There is a whole week. So my 3pm is 0:1500. My 2pm is 0:1400. And my 1pm is 2:1300. Simple. 
            I do not know the best way exactly to do this, but the above concept is pretty clear cut. Does the "day of the week" variable need a Playpref? 
            When you think about it. Every alarm could have a certain time set for all seven days of the week. That is seven settings. But there are 10 
            alarms possible. We coould theoritically have 70 individual alarm settings. 

     */

    // Use this for initialization. Create the playerprefs here. 
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
