using System.Collections.Generic;
using System.Linq;
using Fiber.Utilities;
using GamePlay;
using GamePlay.Boats;
using UnityEngine;

namespace Managers
{
	public class BoatManager : Singleton<BoatManager>
	{
		[SerializeField] private Transform boatHolder;
		
		private List<Boat> boats;

		private void Awake()
		{
			boats = boatHolder.GetComponentsInChildren<Boat>().ToList();
		}
	}
}