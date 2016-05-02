using UnityEngine;
using System.Collections;

public class position : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Vector3 pos = transform.position;
        Debug.Log("Inst x : " + pos.x + " and Inst y : " + pos.y + " and Inst z : " + pos.z);
        /*pos.x = 387;
        pos.y = 172;
        pos.z = 0;
        Debug.Log("Inst x : " + pos.x + " and Inst y : " + pos.y + " and Inst z : " + pos.z);
        */
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
