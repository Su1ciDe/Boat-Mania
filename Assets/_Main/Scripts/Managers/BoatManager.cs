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

		public void RemoveBoat(Boat boat)
		{
			boats.Remove(boat);
			Destroy(boat.gameObject);
		}

		public int GetBoatCountInGrid()
		{
			int count = 0;
			for (var i = 0; i < boats.Count; i++)
			{
				if (!boats[i].CurrentHolder)
					count++;
			}

			return count;
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