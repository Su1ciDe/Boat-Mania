using System.Collections.Generic;
using Fiber.Managers;
using Fiber.Utilities;
using UnityEngine;

namespace HolderSystem
{
	public class Holder : Singleton<Holder>
	{
		[SerializeField] private float slotCount;
		[Space]
		[SerializeField] private float rotation;

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
				{
					return holderSlots[i];
				}
			}

			return null;
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
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireMesh(meshFilter.sharedMesh, new Vector3(i * holderSlotPrefab.Size - offset, 0, 0) + transform.position, Quaternion.Euler(new Vector3(90, rotation, 0)),
					new Vector3(3, 7));
			}
		}
#endif
	}
}