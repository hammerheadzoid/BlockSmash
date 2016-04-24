using UnityEngine;
using System.Collections;

public class Edit_Alarm : MonoBehaviour {

	// March 2nd Test Donal
//hello donal
	
	private int time;
	private int tmpalarm;
	private int digit1,digit2,digit3,digit4;
	public GameObject wheel1,wheel2,wheel3,wheel4;
	
	void Start () {
		//lock screen rotation
		Screen.orientation = ScreenOrientation.Portrait;

		//scale maincamera
		Camera.main.orthographicSize = Screen.height/2;

		//lock screen rotation
		Screen.orientation = ScreenOrientation.Portrait;

        System.DateTime timenow = System.DateTime.Now;
        time = timenow.Hour * 100 + timenow.Minute;

        setinitialtime (time);
		//tmpalarm = PlayerPrefs.GetInt ("PlayerPrefs_tmpalarm");//temporary
	}

    void Update () {
	}

	void setinitialtime(int time){

        digit1 = time / 1000;
		digit2 = time % 1000 / 100;
		digit3 = time % 100 / 10;
		digit4 = time % 10;

        Debug.Log("digit1: " + digit1);
        Debug.Log("digit2: " + digit2);
        Debug.Log("digit3: " + digit3);
        Debug.Log("digit4: " + digit4);


        time = digit1 * 1000 + digit2 * 100 + digit3 * 10 + digit4;
        Debug.Log("initial time set to " + time);

        int w1 = digit1 * 120;
        int w2 = digit2 * 36;
        int w3 = digit3 * 60;
        int w4 = digit4 * 36;

        Debug.Log("rotate1: " + w1);
        Debug.Log("rotate2: " + w2);
        Debug.Log("rotate3: " + w3);
        Debug.Log("rotate4: " + w4);

        wheel1.transform.Rotate (w1/2, 0, 0);
        wheel2.transform.Rotate (w2/2, 0, 0);
        wheel3.transform.Rotate (w3/2, 0, 0);
        wheel4.transform.Rotate (w4/2, 0, 0);

    }

    public void updatealarm() {
        digit1 %= 3;
        digit2 %= 10;
        digit3 %= 6;
        digit4 %= 10;

        time = digit1 * 1000 + digit2 * 100 + digit3 * 10 + digit4;
        Debug.Log(time);
        tmpalarm = time;//temporary
        Debug.Log(tmpalarm);
        PlayerPrefs.SetInt("PlayerPrefs_tmpalarm", tmpalarm);//temporary
    }

    public void wheeloneup()
    {
        wheel1.transform.Rotate(120, 0, 0);//360/3
        digit1++;
        updatealarm();
    }

    public void wheelonedown()
    {
        wheel1.transform.Rotate(-120, 0, 0);//360/3
        digit1--;
        if (digit1 <= -1)
        {
            digit1 = 2;
        }
        updatealarm();
    }

    public void wheeltwoup()
    {
        wheel2.transform.Rotate(36, 0, 0);//360/10
        digit2++;
        /*
        if (digit2 >= 10)
        {
            wheel1.transform.Rotate(120, 0, 0);
            digit1++;
        }
        */
        updatealarm();
    }

    public void wheeltwodown()
    {
        wheel2.transform.Rotate(-36, 0, 0);//360/10
        digit2--;
        if (digit2 <= -1)
        {
            digit2 = 9;
        }
        updatealarm();
    }

    public void wheelthreeup()
    {
        wheel3.transform.Rotate(60, 0, 0);//360/6
        digit3++;
        /*
        if (digit3 >= 6)
        {
            wheel2.transform.Rotate(36, 0, 0);
            digit2++;
            if (digit2 >= 10)
            {
                wheel1.transform.Rotate(120, 0, 0);
                digit1++;
            }
        }
        */
        updatealarm();
    }

    public void wheelthreedown()
    {
        wheel3.transform.Rotate(-60, 0, 0);//360/6
        digit3--;
        if (digit3 <= -1)
        {
            digit3 = 5;
        }
        updatealarm();
    }

    public void wheelfourup()
    {
        wheel4.transform.Rotate(36, 0, 0);//360/10
        digit4++;
        /*
        if (digit4 >= 10)
        {
            wheel3.transform.Rotate(60, 0, 0);
            digit3++;
            if (digit3 >= 6)
            {
                wheel2.transform.Rotate(36, 0, 0);
                digit2++;
                if (digit2 >= 10)
                {
                    wheel1.transform.Rotate(120, 0, 0);
                    digit1++;
                }
            }
        }
        */
        updatealarm();
    }

    public void wheelfourdown()
    {
        wheel4.transform.Rotate(-36, 0, 0);//360/10
        digit4--;
        if (digit4 <= -1)
        {
            digit4 = 9;
        }
        updatealarm();
    }
}
