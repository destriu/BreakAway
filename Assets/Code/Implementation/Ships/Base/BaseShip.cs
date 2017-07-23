using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

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
	public float specialCoolDown;
	private bool isFiringPrimary;
	private bool isSpecialOnCoolDown;

	public float turnRate;
	public float moveSpeed;
	public float maxSpeed;
	private bool isMoving;
	private Vector2 velocity;
	private Vector2 direction;
	private Rigidbody2D shipRigidBody;

	public enum AmmoType
	{
		singleShot,
		doubleShot,
		tripleShot,
		burstShot,
		pieringShot
	}

	public AmmoType ammoType;

	private List<GameObject> ammo;
	public GameObject[] ammoSpawnPositions;
	private Queue<GameObject> ammoQueue;

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

	public bool IsMoving
	{
		get{
			return isMoving;
		}
	}
	#endregion

	#region Setup
	// Use this for initialization
	void Start () 
	{
		//shipRigidBody = GetComponent<Rigidbody2D>();
		gm = GameManager.GM;
		GameManager.move += MoveShip;
		GameManager.noMovement += MovementStopped;

		GameManager.fire1 += StartFiringPrimary;
		GameManager.fire2 += FireSpecial;
		GameManager.stopFire1 += StopFiringPrimary;

		ammo = new List<GameObject>();
		ammo = GameObject.FindGameObjectsWithTag("Ammo").OfType<GameObject>().ToList();
		ammo = ammo.OrderBy(go => PadNumbers(go.name)).ToList();

		ammoQueue = new Queue<GameObject>();
		for(int i = 0; i < ammo.Count; i++)
		{
			ammoQueue.Enqueue(ammo[i]);
		}
	}

	private string PadNumbers(string input) 
	{ 
		return Regex.Replace(input, "[0-9]+", match => match.Value.PadLeft(10, '0')); 
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
		isMoving = true;
		velocity = new Vector2 (Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) * moveSpeed * Time.deltaTime;
		direction  = velocity.normalized;

		float z = transform.rotation.eulerAngles.z;
		z -= Input.GetAxis("Horizontal") * turnRate * Time.deltaTime;

		transform.eulerAngles = new Vector3( 0, 0, Mathf.Atan2( Input.GetAxis("Vertical"), Input.GetAxis("Horizontal")) * Mathf.Rad2Deg - 90f);

		transform.position += (Vector3)velocity;
	}

	private void MovementStopped()
	{
		isMoving = false;
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
		while(isFiringPrimary)
		{
			Debug.Log("Firing");
			SetupAndFireAmmo();
			yield return new WaitForSeconds(fireRate);
		}
	}

	// This should stop the ship from firing any more projectiles from it's primary weapon
	private void StopFiringPrimary()
	{
		if(isFiringPrimary)
		{
			isFiringPrimary = false;
		}
	}

	private void SetupAndFireAmmo()
	{
		GameObject[] ammoInUse = new GameObject[0];
		switch(ammoType)
		{
		case AmmoType.singleShot:
			ammoInUse = GetAmmoInUse<BaseProjectile>(numAmmoNeeded:1);
			break;

		case AmmoType.doubleShot:
			ammoInUse = GetAmmoInUse<BaseProjectile>(numAmmoNeeded:2);
			break;

		case AmmoType.tripleShot:
			ammoInUse = GetAmmoInUse<BaseProjectile>(numAmmoNeeded:3);
			break;

		case AmmoType.burstShot:
			ammoInUse = GetAmmoInUse<BurstAmmo>(numAmmoNeeded:1);
			break;

		case AmmoType.pieringShot:
			ammoInUse = GetAmmoInUse<BaseProjectile>(numAmmoNeeded:1);
			break;
		}
			
		for(int i = 0; i < ammoInUse.Length; i++)
		{
			ammoInUse[i].transform.position = ammoSpawnPositions[i + (ammoType == AmmoType.doubleShot ? 1 : 0)].transform.position;

			if(ammoType == AmmoType.doubleShot)
			{
				ammoInUse[i].GetComponent<BaseProjectile>().SetDirection(direction == Vector2.zero ? Vector2.up : direction);
			}
		}
	}

	// Should return a list containing ammo from the ammoQueue to the amount sent in
	private GameObject[] GetAmmoInUse<T>(int numAmmoNeeded)
	{
		GameObject[] ammoInUse = new GameObject[numAmmoNeeded];
		for(int i = 0; i < numAmmoNeeded; i++)
		{
			ammoInUse[i] = ammoQueue.Dequeue();
			ammoQueue.Enqueue(ammoInUse[i]);

			ammoInUse[i].AddComponent(typeof(T));
		}

		return ammoInUse;
	}

	// This should fire the current speacial weapon of the ship
	private void FireSpecial()
	{
		if(!isSpecialOnCoolDown)
		{
			Debug.Log("Special Fired");
			isSpecialOnCoolDown = true;
			StartCoroutine(SpecialCoolDown());
		}
		else
		{
			Debug.Log("Special is on cool down");
		}
	}

	private IEnumerator SpecialCoolDown()
	{
		yield return new WaitForSeconds(specialCoolDown);
		isSpecialOnCoolDown = false;
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
