using AYellowpaper.SerializedCollections;
using Fiber.Utilities;
using GamePlay.Boats;
using GamePlay.Cars;
using HolderSystem;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Prefabs", menuName = "Boat Mania/Prefabs", order = 1)]
	public class PrefabsSO : ScriptableObject
	{
		public HolderSlot HolderSlotPrefab;
		public Car CarPrefab;
		public SerializedDictionary<BoatType, Boat> BoatPrefabs = new SerializedDictionary<BoatType, Boat>();
	}
}