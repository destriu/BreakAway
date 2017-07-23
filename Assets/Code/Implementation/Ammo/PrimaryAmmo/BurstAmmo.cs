using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstAmmo : BaseProjectile
{
	#region Variables
	private SpriteRenderer sr;
	private ParticleSystem ps;
	private float radialDamage = 5f;
	private float radialForce = 10f;
	#endregion

	#region Setup
	private void Start()
	{
		base.Setup();
		destroyTime = .5f;
		sr = GetComponent<SpriteRenderer>();
		ps = GetComponent<ParticleSystem>();
	}
	#endregion

	#region Body
	private void FixedUpdate()
	{
		base.DestroyTimer();
	}

	// This should be overriden if there needs to be an effect after hitting a enemy
	protected override void AmmoEffect()
	{
		ps.Emit(1);
		sr.enabled = false;
		trailRender.enabled = false;
		StartCoroutine(DelayedDestroy());
	}

	private IEnumerator DelayedDestroy()
	{
		yield return new WaitForSeconds(ps.main.duration);
		ResetProjectile();
	}

	protected override void ResetProjectile()
	{
		ps.Stop();
		transform.position = resetPosition;
		Destroy(this);
	}

	private void OnParticleCollision(GameObject other)
	{
		Vector3 forceDirection = transform.position - other.transform.position;

		Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
		rb.AddForce(forceDirection * radialForce, ForceMode2D.Impulse);
	}
	#endregion
}
