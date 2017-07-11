using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Move ();
public class GameManager : MonoBehaviour 
{
	#region Variables
	public static Move move;

	private GameObject player;

	private bool isMoving;
	private bool isRotating;
	private bool isPaused;

	//********** Properties **************//
	public bool IsMoving
	{
		get{
			return isMoving;
		}
	}

	public bool IsRotating
	{
		get{
			return isRotating;
		}
	}

	public bool IsPaused
	{
		get{
			return isPaused;
		}
	}
	#endregion

	#region Body
	void Start()
	{
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		GetMovementInput();
	}

	private void GetMovementInput()
	{
		Vector2 direction = new Vector2();
	
		isRotating = Input.GetAxis("Horizontal") != 0 || Input.GetButton("Horizontal");
		isMoving = Input.GetAxis("Vertical") != 0 || Input.GetButton("Vertical");

		if(isMoving || isRotating)
		{
			move();
		}
	}
	#endregion
}
