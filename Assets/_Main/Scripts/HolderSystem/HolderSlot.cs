using GamePlay.Boats;
using UnityEngine;

namespace HolderSystem
{
	public class HolderSlot : MonoBehaviour
	{
		public Boat Boat { get; set; }

		[SerializeField] private float size;
		public float Size => size;

		public void SetBoat(Boat boat)
		{
			Boat = boat;
		}
	}
}