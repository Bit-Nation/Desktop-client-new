using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

    // Controls time elapsed since last cycle
	float time = 0;
	
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// add the ammount of time in seconds since last frame
		time += Time.deltaTime;
					
		// If on the current frame the time elapsed > then this value
		// then it rotates the loader image by 30 degrees
		if(time > 0.075){
			
			this.gameObject.transform.Rotate(0, 0, -30);
			
			time = 0;
					
		}       

    }
	
}
