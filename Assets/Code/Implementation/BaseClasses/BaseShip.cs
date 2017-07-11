using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShip : MonoBehaviour
{
	#region Stats
	//********** General ************//
	private GameManager gm;

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
	private bool isBoostActivated;

	public float fireRate;
	private bool isFiringPrimary;

	public float turnRate;
	public float moveSpeed;
	public float maxSpeed;
	public Vector2 maxVelocity;
	public Vector2 maxTurnVelocity;

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
		//shipRigidBody = GetComponent<Rigidbody2D>();
		gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
		GameManager.move += MoveShip;
	}
	#endregion

	#region Ship Actions
	void FixedUpdate () 
	{
		
	}

	//**************************** Movement ******************************//
	// This should take care of all movement of the ship
	private void MoveShip()
	{
		Vector2 velocity = Vector2.zero;
		velocity = new Vector2 (Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;

		float z = transform.rotation.eulerAngles.z;
		z -= Input.GetAxis("Horizontal") * turnRate * Time.deltaTime;

		transform.eulerAngles = new Vector3( 0, 0, Mathf.Atan2( Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg - 90f);

		transform.position += (Vector3)velocity;
	}

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
		while(true)
		{
			yield return new WaitForSeconds(fireRate);
			Debug.Log("Firing");
		}
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
