using UnityEngine;
using System.Collections;

public class Create_Wheel : MonoBehaviour {

	private GameObject wheel1,wheel2,wheel3,wheel4;

	void Start () {
		wheel1 = new GameObject();
		wheel2 = new GameObject();
		wheel3 = new GameObject();
		wheel4 = new GameObject();

		wheel1.name="Wheel_1";
		wheel2.name="Wheel_2";
		wheel3.name="Wheel_3";
		wheel4.name="Wheel_4";

		createwheel (wheel1,3);
		createwheel (wheel2, 10);
		createwheel (wheel3, 6);
		createwheel (wheel4, 10);

		wheel1.transform.position = new Vector3(-3,0,0);
		wheel2.transform.position = new Vector3(-1,0,0);
		wheel3.transform.position = new Vector3(1,0,0);
		wheel4.transform.position = new Vector3(3,0,0);
	
		wheel1.transform.Rotate (60,0,0);
	}

	void createwheel(GameObject parentwheel,int divisions){
		int rotationangle = 360 / divisions;
		for(int i=0;i<divisions;i++){
			GameObject cube=GameObject.CreatePrimitive(PrimitiveType.Cube);
			cube.name=i.ToString();
			cube.transform.position = new Vector3(0,0,5);
			cube.transform.parent = parentwheel.transform;
			parentwheel.transform.Rotate (rotationangle,0,0);
		}
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("1")){
			wheel1.transform.Rotate(120,0,0);//360/3
		}
		if(Input.GetKeyDown("2")){
			wheel2.transform.Rotate(36,0,0);//360/10
		}
		if(Input.GetKeyDown("3")){
			wheel3.transform.Rotate(60,0,0);//360/6
		}
		if(Input.GetKeyDown("4")){
			wheel4.transform.Rotate(36,0,0);//360/10
		}
	}
}
