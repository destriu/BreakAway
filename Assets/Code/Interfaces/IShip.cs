using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShip  
{
	#region Stat Properties
	float Health {get; set;}
	float Barrier {get; set;}
	float BoosterFuel {get;}
	#endregion

	#region Ship Actions
	void FirePrimary();// This should fire the current primary weapon of the ship
	void FireSpecial();// This should fire the current speacial weapon of the ship
	IEnumerable BarrierRecharge();// This will start recharging the barrier in intervals after a short delay
	void Move();// This should take care of all movement of the ship
	void Boost();// This should activate boosters on the ship
	void DestroyShip();// This should be called if the ships health hits 0
	#endregion
}
