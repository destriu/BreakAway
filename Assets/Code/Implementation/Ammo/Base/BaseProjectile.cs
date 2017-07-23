using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseProjectile : MonoBehaviour 
{
	#region Stats
	private bool init;
	protected TrailRenderer trailRender;

	protected float damage;
	protected float moveSpeed;

	protected float duration;
	protected float destroyTime;

	protected Vector3 direction;
	protected Vector3 resetPosition;
	#endregion

	#region Setup
	private void Start()
	{
		Setup();
	}

	protected void Setup()
	{
		damage = 10f;
		moveSpeed = 35f;
		destroyTime = 2f;

		resetPosition = new Vector3(999f,999f,0);
		trailRender = GetComponent<TrailRenderer>();

		GetDirection();
		trailRender.enabled = true;
	}
	#endregion

	#region Body
	public void SetDirection(Vector2 dir)
	{
		direction = dir;
	}

	private void GetDirection()
	{
		if(direction == Vector3.zero)
		{
			direction = ( transform.position - GameManager.Player.transform.position ).normalized;
		}
	}

	void FixedUpdate()
	{
		DestroyTimer();
	}

	// This should be overriden if there needs to be an effect after hitting a enemy
	protected virtual void AmmoEffect()
	{
		ResetProjectile();
	}
		
	protected virtual void DestroyTimer()
	{
		// keep track of how long the projectile has been alive
		duration += Time.deltaTime;

		if(duration >= destroyTime)
		{
			AmmoEffect();
		}
		else
		{
			// projectile movement
			transform.position += direction * moveSpeed * Time.deltaTime;
		}
	}

	protected virtual void ResetProjectile()
	{
		trailRender.enabled = false;
		transform.position = resetPosition;
		Destroy(this);
	}

	private void OnCollision2DEnter(Collision2D coll)
	{
		if(coll.gameObject.tag != "Player")
		{
			AmmoEffect();
		}
	}
	#endregion
}
