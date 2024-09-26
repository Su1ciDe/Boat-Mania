using System.Linq;
using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Boats;
using TriInspector;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace Managers
{
	public class BoatManager : Singleton<BoatManager>
	{
		[SerializeField] private Transform boatHolder;

		private List<Boat> boats;
		public List<Boat> Boats => boats;

		private void Awake()
		{
			boats = boatHolder.GetComponentsInChildren<Boat>().ToList();
		}

		private void OnEnable()
		{
			Boat.OnBoatExited += OnBoatExited;
		}

		private void OnDisable()
		{
			Boat.OnBoatExited -= OnBoatExited;
		}

		private void OnBoatExited(Boat boat)
		{
			boats.Remove(boat);
		}

		#region Editor

#if UNITY_EDITOR
		public Boat SpawnBoat(ColorType colorType, BoatType boatType)
		{
			if (boatType == BoatType.None) return null;

			var boat = (Boat)PrefabUtility.InstantiatePrefab(GameManager.Instance.PrefabsSO.BoatPrefabs[boatType], boatHolder);
			boat.SetupEditor(colorType);

			CalculateBoats();

			return boat;
		}

		private void OnDrawGizmosSelected()
		{
			CalculateBoats();
		}

		[Button]
		private void CalculateBoats()
		{
			boatCounts.Clear();

			var boatsInLevel = boatHolder.GetComponentsInChildren<Boat>();
			foreach (var goalOption in boatsInLevel)
			{
				var found = false;

				var goalCount = boatCounts.Where(x => x.ColorType == goalOption.ColorType);
				foreach (var count in goalCount)
				{
					count.Count += (int)goalOption.BoatType;
					found = true;
				}

				if (!found)
					boatCounts.Add(new BoatCount(goalOption.ColorType, (int)goalOption.BoatType));
			}
		}

		[System.Serializable]
		private class BoatCount
		{
			[ReadOnly] public ColorType ColorType;
			[ReadOnly] public int Count;

			public BoatCount(ColorType color, int count)
			{
				ColorType = color;
				Count = count;
			}
		}

		[TableList(Draggable = false, AlwaysExpanded = true, HideAddButton = true, HideRemoveButton = true, ShowElementLabels = false)]
		[SerializeField] private List<BoatCount> boatCounts = new List<BoatCount>();

#endif

		#endregion
	}
}