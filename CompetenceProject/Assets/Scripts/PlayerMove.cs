using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
Add this script to player object. Give camera "MainCamera" tag.
 */

[RequireComponent (typeof(CharacterController))]

public class PlayerMove : MonoBehaviour {

	public float speed;
	public float sprintSpeed;
	public float rotationSpeed;
	//public float gravity;
    public float turnSpeed;
    public float currentSpeed;

	//public VirtualJoystick virtualJoystick;

	private CharacterController cc;
	private bool sprint = false;

    public PlayFootsteps playFootsteps;

    public FogHack fog;

    //private AudioController ac;
    //public string playerFootsteps;

	private void Awake() {
		cc = GetComponent<CharacterController>();
        playFootsteps = GetComponent<PlayFootsteps>();
        currentSpeed = speed;
	}
#if UNITY_EDITOR || UNITY_STANDALONE
    private void FixedUpdate() {

        if(fog != null) fog.UpdateRealtivePositionToPlayer();

        if (GameManager.instance.charactersCanMove) {
            Vector3 targetMove = Vector3.zero;

            int rotationDir = 0;

            if (Input.GetMouseButton(0)) { rotationDir -= 1; }
            if (Input.GetMouseButton(1)) { rotationDir += 1; }
            transform.Rotate(new Vector3(0, rotationDir * turnSpeed, 0));



            if (Input.GetMouseButton(0) && Input.GetMouseButton(1)) {
                sprint = false;
                currentSpeed = speed;
                playFootsteps.currentCounterIndex = 0;
            }
            else {
                sprint = true;
                currentSpeed = sprintSpeed;
                playFootsteps.currentCounterIndex = 1;
            }

            Vector3 newPos = transform.forward * currentSpeed * Time.deltaTime;
            cc.Move(newPos);
            playFootsteps.Play();
        }
	}

#elif UNITY_EDITOR || UNITY_STANDALONE
    private bool turnLeft = false;
    private bool turnRight = false;

    private void FixedUpdate() {

        if (GameManager.instance.charactersCanMove) {
            Vector3 targetMove = Vector3.zero;

            int rotationDir = 0;

            if (turnLeft) { rotationDir -= 1; }
            if (turnRight) { rotationDir += 1; }
            transform.Rotate(new Vector3(0, rotationDir * turnSpeed, 0));



            if (turnLeft && turnRight) {
                sprint = false;
                currentSpeed = speed;
                playFootsteps.currentCounterIndex = 0;
            }
            else {
                sprint = true;
                currentSpeed = sprintSpeed;
                playFootsteps.currentCounterIndex = 1;
            }

            Vector3 newPos = transform.forward * currentSpeed * Time.deltaTime;
            cc.Move(newPos);
            playFootsteps.Play();
        }
	}

    public void AddRotation(string rotation) {
        switch (rotation) {
            case "Left":
                turnLeft = true;
                break;
            case "Right":
                turnRight = true;
            default:
                turnLeft = false;
                turnRight = false;
        }
    }

    public void RemoveRotation(string rotation) {
        switch (rotation) {
            case "Left":
                turnLeft = false;
                break;
            case "Right":
                turnRight = false;
            default:
                turnLeft = false;
                turnRight = false;
        }
    }

#endif

    public void AddRotation(string rotation) {

    }
    public void RemoveRotation(string rotation) {
        
    }

    private Vector3 Transform2CameraView(Vector3 dir){
		Vector3 newDir = Camera.main.transform.TransformDirection(dir);
		newDir.y = 0;
		newDir = newDir.normalized * dir.magnitude;

		return newDir;
	}

	public bool isSprinting{
		set {sprint = value;}
		get { return sprint; }
	}

	public float CurrentSpeed{
		get { return cc.velocity.magnitude; }
	}

    public Vector3 PredictPosition() {
        Vector3 nextFramePosition = transform.position + transform.forward * currentSpeed;
        return nextFramePosition;
    }
}

/*
 * // Orientate to camera view
        targetMove = Transform2CameraView(targetMove);

		// Set max magnitude to 1.0f
		if (targetMove.magnitude > 1.0f) targetMove= targetMove.normalized;
		
		// Apply speed & gravity
		Vector3 move = targetMove;
		if (sprint) move *= sprintSpeed;
		else move *= speed;

                // IF GRAVITY IS DESIRED, use next line instead of "move += 0;
        //move.y += gravity * Time.deltaTime + Mathf.Min(cc.velocity.y, 0);
        move.y = 0;

		cc.Move(move * Time.deltaTime);

		// Rotation of character
        if (move != Vector3.zero){
            Vector3 lookDir = move;
            lookDir.y = 0;
            Vector3 newLook = Vector3.RotateTowards(transform.forward, lookDir, Mathf.Rad2Deg * rotationSpeed * Time.deltaTime, 0);
            transform.rotation = Quaternion.LookRotation(newLook);
        }
        */
