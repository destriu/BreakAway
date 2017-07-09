using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShip : MonoBehaviour
{
	#region Stats
	//********** Ship Stats ********//
	public float health = 100f;
	public float maxHealth = 100f;

	public float barrier = 50f;
	public float maxBarrier = 50f;
	public float barrierRechargeRate;
	private bool isBarrierRecharging;

	private float boostFuel;
	private float maxBoostFuel;
	public float boostModifier;

	public float fireRate;
	private bool isFiringPrimary;

	public float moveSpeed;
	public Vector2 maxVelocity;

	private Rigidbody2D shipRigidBody;

	//********* Properties *********//
	public float Health
	{
		get{
			return health;
		}

		set{
			float newHealth = health + value;
			if ( newHealth <= 0 )
			{
				health = 0;
				DestroyShip();
			}
			else if(newHealth > maxHealth)
			{
				health = maxHealth;
			}
		}
	}

	public float Barrier
	{
		get{
			return barrier;
		}

		set{
			float newBarrier = barrier + value;
			if(newBarrier < maxBarrier)
			{
				barrier = (newBarrier < 0) ? 0 : newBarrier;
				if(!isBarrierRecharging)
				{
					isBarrierRecharging = true;
					StartCoroutine(BarrierRecharge());
				}

			}
			else if(newBarrier > maxBarrier)
			{
				barrier = maxBarrier;
			}
		}
	}

	public float BoostFuel
	{
		get{
			return boostFuel;
		}

		set{
			float newBoostFuel = boostFuel + value;
			if(newBoostFuel < maxBoostFuel)
			{
				boostFuel = (newBoostFuel < 0) ? newBoostFuel : 0;
			}
			else if(newBoostFuel > maxBoostFuel)
			{
				boostFuel = maxBoostFuel;
			}
		}
	}
	#endregion

	#region Setup
	// Use this for initialization
	void Start () 
	{
		shipRigidBody = GetComponent<Rigidbody2D>();
	}
	#endregion

	#region Ship Actions
	void FixedUpdate () 
	{
		Move();
	}

	//**************************** Movement ******************************//
	// This should take care of all movement of the ship
	private void Move()
	{
		Vector2 direction = new Vector2( Input.GetAxis("Horizontal") > 0 ? 1f: -1f, Input.GetAxis("Vertical") > 0 ? 1f: -1f );
		if(Input.GetButton("Horizontal"))
		{
			
			//Debug.Log("Moving horizontal " + Input.GetAxis("Horizontal") );
			Vector2 forceVector = (moveSpeed * Time.deltaTime * 60f) * Vector2.right * direction.x;
			Debug.Log(forceVector);
			shipRigidBody.AddForce(forceVector);
		}

		if(Input.GetButton("Vertical"))
		{
			//Debug.Log("Moving vertical " + Input.GetAxis("Vertical") );
			Vector2 forceVector = (moveSpeed * Time.deltaTime * 60f) * Vector2.up * direction.y;
			Debug.Log(forceVector);
			shipRigidBody.AddForce(forceVector);
		}

		// adjust velocity on x or y axis if either goes above the max
		Vector2 currentVelocity = shipRigidBody.velocity;
		shipRigidBody.velocity = new Vector2( Mathf.Abs(currentVelocity.x) > maxVelocity.x ? maxVelocity.x * direction.x : currentVelocity.x, Mathf.Abs(currentVelocity.y) > maxVelocity.y ? maxVelocity.y * direction.y : currentVelocity.y );
	}

	//private void 

	// This should activate boosters on the ship
	private void Boost()
	{
		
	}

	//*************************** Weapon Firing **************************//
	private void StartFiringPrimary()
	{
		if(!isFiringPrimary)
		{
			isFiringPrimary = true;
			StartCoroutine(FirePrimary());
		}
	}

	// This should fire the current primary weapon of the ship
	protected IEnumerator FirePrimary()
	{
		yield return new WaitForSeconds(fireRate);
	}

	// This should stop the ship from firing any more projectiles from it's primary weapon
	private void StopFiringPrimary()
	{
		if(isFiringPrimary)
		{
			isFiringPrimary = false;
			StopCoroutine("FirePrimary");
		}
	}

	// This should fire the current speacial weapon of the ship
	private void FireSpecial()
	{
		
	}

	//************************* Barrier *************************************//
	// This will start recharging the barrier in intervals after a s
	private IEnumerator BarrierRecharge()
	{
		yield return new WaitForSeconds(4f);

		while(barrier < maxBarrier)
		{
			Barrier += barrierRechargeRate;
			yield return new WaitForSeconds(1f);
		}

		isBarrierRecharging = false;
	}

	//************************ Ship Death ***********************************//
	// This should be called if the ships health hits 0
	protected virtual void DestroyShip()
	{
		
	}
	#endregion


}
