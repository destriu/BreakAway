using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Move ();
public delegate void Fire();
public delegate void StopFire();
public class GameManager : MonoBehaviour 
{
	#region Variables
	public static Fire fire1;
	public static Fire fire2;
	public static StopFire stopFire1;
	public static Move move;
	public static Move noMovement;

	private static GameManager gm;
	private static GameObject player;

	private bool isPaused;

	//********** Properties **************//
	public static GameManager GM
	{
		get{
			return gm;
		}
	}

	public static GameObject Player
	{
		get{
			return player;
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
	void Awake()
	{
		gm = this;
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Start()
	{
		
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		GetFireInput();
		GetMovementInput();
	}

	private void GetMovementInput()
	{
		if((Input.GetAxis("Vertical") != 0 || Input.GetButton("Vertical")) || 
			(Input.GetAxis("Horizontal") != 0 || Input.GetButton("Horizontal")))
		{
			move();
		}
		else
		{
			noMovement();
		}
	}

	private void GetFireInput()
	{
		if(Input.GetButton("Fire1"))
		{
			fire1();
		}
		else
		{
			stopFire1();
		}

		if(Input.GetButton("Fire2"))
		{
			fire2();
		}
	}

	private void PauseGame()
	{
		
	}
	#endregion
}
