using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

	public bool MenuOpen { get; set; }

	protected virtual void Awake () {
		GlobalManager.Instance.LevelManager = this;
	}
	protected virtual void Update () {
		
	}

	public virtual void ResetLevel()
	{

	}

	public virtual void AdvanceLevel()
	{

	}

	public virtual void QuitLevel()
	{

	}

	public virtual void ProcessPause(bool pausePressed, bool unpausePressed = false) {
		//unpause isn't mapped for keyboards because the escape button is used for unpause and back (ESCAPE)
		if(MenuOpen) {
			if(unpausePressed /* || pausePressed && Menu.AtRoot */) {
				Debug.Log("Unpaused");
				MenuOpen = false;
			}
		}
		else {
			if(pausePressed) {
				Debug.Log("Paused");
				MenuOpen = true;
			}
		}
	}

	public virtual void ProcessPlayingInputs(float horizontal, float vertical, float switchblock, bool placeblock, bool jump, Vector3 courseAdjust, Vector3 fineAdjust, Vector3 mousePosition)
	{
		/* TESTING INPUTS
		Debug.Log("Horizontal: " + horizontal + ", Vertical: " + vertical);
		if(switchblock!=0 || placeblock || jump)
			Debug.Log(switchblock + " " + placeblock + " " + jump);
		*/


	}

	public virtual void ProcessPauseInputs(float horizontal, float vertical, bool menuconfirm, bool menuback)
	{
		/* TESTING INPUTS
		Debug.Log("Horizontal: " + horizontal + ", Vertical: " + vertical);
		if(menuconfirm || menuback)
			Debug.Log(menuconfirm + " " + menuback);
		*/


	}
}
