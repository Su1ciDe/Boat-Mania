using GamePlay.Boats;
using UnityEngine;

namespace HolderSystem
{
	public class HolderSlot : MonoBehaviour
	{
		public Boat Boat { get; private set; }

		[SerializeField] private float size;
		public float Size => size;

		public void SetBoat(Boat boat)
		{
			Boat = boat;
			Boat.OnBoatMoved += OnBoatArrived;
		}

		private void OnBoatArrived()
		{
			Boat.OnBoatMoved -= OnBoatArrived;

			Debug.Log("arrived");
		}
	}
}