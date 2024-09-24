using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Boats;
using TriInspector;
using UnityEngine;
using Utilities;

namespace HolderSystem
{
	public class Holder : Singleton<Holder>
	{
		[Title("Parameters")]
		[SerializeField] private float slotCount;
		[Space]
		[SerializeField] private float rotation;

		[Title("References")]
		[SerializeField] private Transform exitPoint;
		public Transform ExitPoint => exitPoint;

		private readonly List<HolderSlot> holderSlots = new List<HolderSlot>();

		private void Awake()
		{
			Setup();
		}

		private void Setup()
		{
			var holderSlotPrefab = GameManager.Instance.PrefabsSO.HolderSlotPrefab;
			var offset = slotCount * holderSlotPrefab.Size / 2f - holderSlotPrefab.Size / 2f;
			for (int i = 0; i < slotCount; i++)
			{
				var slot = Instantiate(holderSlotPrefab, transform);
				slot.transform.localPosition = new Vector3(i * holderSlotPrefab.Size - offset, 0, 0);
				slot.transform.localEulerAngles = new Vector3(0, rotation, 0);
				holderSlots.Add(slot);
			}
		}

		#region Helpers

		public HolderSlot GetFirstEmptySlot()
		{
			for (var i = 0; i < holderSlots.Count; i++)
			{
				if (!holderSlots[i].Boat)
					return holderSlots[i];
			}

			return null;
		}

		public Boat GetBoatByType(ColorType colorType)
		{
			for (var i = 0; i < holderSlots.Count; i++)
			{
				if (holderSlots[i].Boat && holderSlots[i].Boat.ColorType == colorType && !holderSlots[i].Boat.IsCompleted)
					return holderSlots[i].Boat;
			}

			return null;
		}

		public bool IsAnyBoatLoadingCars()
		{
			for (var i = 0; i < holderSlots.Count; i++)
			{
				if (holderSlots[i].Boat && holderSlots[i].Boat.IsLoadingCars && holderSlots[i].Boat.IsAnyCarMoving())
					return true;
			}

			return false;
		}

		public bool IsAnyBoatMoving()
		{
			for (int i = 0; i < holderSlots.Count; i++)
			{
				if (holderSlots[i].Boat && holderSlots[i].Boat.IsMoving)
					return true;
			}

			return false;
		}

		#endregion

#if UNITY_EDITOR
		private void OnDrawGizmos()
		{
			var holderSlotPrefab = GameManager.Instance.PrefabsSO.HolderSlotPrefab;
			var meshFilter = holderSlotPrefab.GetComponentInChildren<MeshFilter>();
			var offset = slotCount * holderSlotPrefab.Size / 2f - holderSlotPrefab.Size / 2f;
			for (int i = 0; i < slotCount; i++)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireMesh(meshFilter.sharedMesh, new Vector3(i * holderSlotPrefab.Size - offset, 0, 0) + transform.position, Quaternion.Euler(new Vector3(90, rotation, 0)),
					new Vector3(4.5f, 10));
			}
		}
#endif
	}
}