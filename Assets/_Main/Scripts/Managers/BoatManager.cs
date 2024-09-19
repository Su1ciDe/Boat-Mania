using System.Collections.Generic;
using System.Linq;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay;
using GamePlay.Boats;
using UnityEditor;
using UnityEngine;
using Utilities;

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

		#region Editor

#if UNITY_EDITOR
		public Boat SpawnBoat(ColorType colorType, BoatType boatType)
		{
			var boat = (Boat)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.BoatPrefabs[boatType], boatHolder);
			boat.SetupEditor(colorType);

			return boat;
		}
#endif

		#endregion
	}
}