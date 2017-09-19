using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputDevice {
	Controller,
	Keyboard,
	KeyboardMouse
}

public class InputManager : MonoBehaviour {

	public InputDevice inputDevice;
	private Vector2 lastMousePosition;

	// Use this for initialization
	void Update () {
		GlobalManager.Instance.LevelManager.ProcessPause(
			Input.GetButtonDown("Pause"),
			Input.GetButtonDown("Unpause")
		);

		float horizontal, vertical;
		if(GlobalManager.Instance.LevelManager.MenuOpen) {
			horizontal = Input.GetAxisRaw("Horizontal");
			vertical = Input.GetAxisRaw("Vertical");

			GlobalManager.Instance.LevelManager.ProcessPauseInputs(
				horizontal: horizontal,
				vertical: vertical,
				menuconfirm: Input.GetButtonDown("MenuConfirm"),
				menuback: Input.GetButtonDown("MenuBack")
			);
		}
		else {
			horizontal = Input.GetAxis("Horizontal");
			vertical = Input.GetAxis("Vertical");

			//whichever input method is used will set the z component to 0
			Vector3 courseAdjust = Vector3.back;
			Vector3 fineAdjust = Vector3.back;
			Vector3 mouseAdjust = Vector3.back;

			float courseAdjustX = Input.GetAxisRaw("CourseAdjustX");
			float courseAdjustY = Input.GetAxisRaw("CourseAdjustY");
			if (courseAdjustX > 0.3f || courseAdjustY > 0.3f)
			{
				//if we are using the course adjust, fine adjust will be ignored
				courseAdjust = new Vector3(courseAdjustX, courseAdjustY, 0);
				courseAdjust.z = 0;
			}
			else
			{
				float fineAdjustX = Input.GetAxisRaw("FineAdjustX");
				float fineAdjustY = Input.GetAxisRaw("FineAdjustY");
				if (fineAdjustX > 0.3f || fineAdjustY > 0.3f)
				{
					//if we are not using course adjust and fine adjust is being used
					fineAdjust = new Vector3(fineAdjustX, fineAdjustY, 0);
					fineAdjust.z = 0;
				}
			}

			//only take mouse position if it has moved
			Rect screen = new Rect(0,0,Screen.width, Screen.height); //we should make this a variable so it doesn't have to create a rect every frame
			mouseAdjust = Input.mousePosition;
			if(!screen.Contains(mouseAdjust) || Vector2.Distance(lastMousePosition, mouseAdjust) < 1) {
				mouseAdjust.z = -1;
			}
			lastMousePosition = mouseAdjust;

			/*
			if (inputDevice == InputDevice.Controller || inputDevice == InputDevice.Keyboard) {
				
			}
			else {
				//use mouse position
				
			}
			*/
			
			float switchblock = 0;
			if(Input.GetButtonDown("SwitchBlock")) {
				switchblock = Input.GetAxisRaw("SwitchBlock");
			}
			else {
				switchblock = Input.GetAxisRaw("SwitchBlockScrollWheel");
			}

			GlobalManager.Instance.LevelManager.ProcessPlayingInputs(
				horizontal: horizontal,
				vertical: vertical,
				switchblock: switchblock,
				placeblock: Input.GetButton("PlaceBlock") || Mathf.Abs(Input.GetAxisRaw("PlaceBlock")) > 0.9f,
				jump: Input.GetButton("Jump"),
				courseAdjust: courseAdjust,
				fineAdjust: fineAdjust,
				mousePosition: mouseAdjust
			);
		}


	}
}
