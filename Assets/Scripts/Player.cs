using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(PlayerPhysics))]
public class Player : MonoBehaviour{

	public PlayerPhysics Physics { get; private set; }
	public PlayerController Controller { get; private set; }

	public bool Drowning = false;
	public bool PlayerInputAllowed = true;

	public Vector2 InputDirection { get; set; }
	private bool jumpDown = false;

	public float maxJumpHeight = 0.55f;
	public float minJumpHeight = 0.05f;
	public float timeToJumpApex = 0.4f;
	float accelerationTimeAirborne = 0.05f;
	float accelerationTimeGrounded = 0.025f;
	float moveSpeed = 8f;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float wallSlideSpeedMax = 3;
	public float wallStickTime = 0.25f;
	float timeToWallUnstick;

	public float Gravity { get; private set; }
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;

	public Vector2 Velocity;

	bool wallSliding;
	int wallDirX;

	public void Awake(){
		Physics = GetComponent<PlayerPhysics>();
		Physics.Player = this;
		Controller = GetComponent<PlayerController> ();
		Controller.Player = this;
	}

	public void Start() {
		(GlobalManager.Instance.LevelManager as LevelManagerInLevel).RegisterPlayer(this);

		Gravity = -(maxJumpHeight/4f) / Mathf.Pow(timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(Gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(Gravity) * minJumpHeight);

		moveSpeed *= Time.fixedDeltaTime;
	}

	public void FixedUpdate() {
		CalculateVelocity();
		HandleWallSliding();

		Physics.Move(ref Velocity, false);
		velocityXSmoothing = Velocity.x;

		if (Physics.collisions.above || Physics.collisions.below) {
			if (Physics.collisions.slidingDownMaxSlope) {
				Velocity.y += Physics.collisions.slopeNormal.y * -Gravity * Time.fixedDeltaTime;
			}
			else {
				Velocity.y = 0;
			}
		}
	}

	private void CalculateVelocity() {
		//determine how much the player will move this iteration if not obstructed
		float targetVelocityX;
		if (PlayerInputAllowed) {
			targetVelocityX = InputDirection.x * moveSpeed;
		}
		else {
			targetVelocityX = 0f;
		}

		Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref velocityXSmoothing, (Physics.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
		Velocity.y += Gravity * Time.fixedDeltaTime;		
	}

	private void HandleWallSliding() {
		wallDirX = (Physics.collisions.left) ? -1 : 1;
		wallSliding = false;
		if ((Physics.collisions.left || Physics.collisions.right) && !Physics.collisions.below && Velocity.y < 0) {
			//start sliding if there are collisions on the sides, gravity is pushing the player and there arent' collisions below
			wallSliding = true;

			if (Velocity.y < -wallSlideSpeedMax) {
				Velocity.y = -wallSlideSpeedMax;
			}

			//once the timeToWallUnstick is less than zero, break off the wall
			if (timeToWallUnstick > 0) {
				//if time to unstick from wall has already been set
				velocityXSmoothing = 0;
				Velocity.x = 0;

				if (InputDirection.x != wallDirX && InputDirection.x != 0) {
					//subtract the time between iterations if player is facing away from the wall
					timeToWallUnstick -= Time.fixedDeltaTime;
				}
				else {
					//if facing towards the wall, refresh the wall stick time
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				//just got on wall, set the wall stick time
				timeToWallUnstick = wallStickTime;
			}

		}
	}

	public void OnJumpInputDown() {
		if(PlayerInputAllowed) {
			if (wallSliding) {
				//use different jumps depending on wall orientation with respect to the current input direction
				if (wallDirX == InputDirection.x) {
					Velocity.x = -wallDirX * wallJumpClimb.x;
					Velocity.y = wallJumpClimb.y;
				}
				else if (InputDirection.x == 0) {
					Velocity.x = -wallDirX * wallJumpOff.x;
					Velocity.y = wallJumpOff.y;
				}
				else {
					Velocity.x = -wallDirX * wallLeap.x;
					Velocity.y = wallLeap.y;
				}
			}

			//can only jump if there are collisions below
			if (Physics.collisions.below) {
				if (Physics.collisions.slidingDownMaxSlope) {
					if (InputDirection.x != -Mathf.Sign(Physics.collisions.slopeNormal.x)) {
						// not jumping against max slope
						Velocity.y = maxJumpVelocity * Physics.collisions.slopeNormal.y;
						Velocity.x = maxJumpVelocity * Physics.collisions.slopeNormal.x;
					}
				}
				else {
					Velocity.y = maxJumpVelocity;
				}
			}
		}
	}

	public void OnJumpInputUp() {
		if (Velocity.y > minJumpVelocity) {
			Velocity.y = minJumpVelocity;
		}
	}

	public void ProcessJumpInput(bool jump) {
		if(!jumpDown && jump) {
			OnJumpInputDown();
		}
		else if(jumpDown && !jump) {
			OnJumpInputUp();
		}
		jumpDown = jump;
	}

	public void PlayerInWater() {
		Gravity = 0f;
		Velocity.y = 0f;
		Drowning = true;
		PlayerInputAllowed = false;
		Physics.box.enabled = false;
	}
}
