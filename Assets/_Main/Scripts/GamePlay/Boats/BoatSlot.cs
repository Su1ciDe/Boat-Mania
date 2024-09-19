using GamePlay.Cars;
using UnityEngine;

namespace GamePlay.Boats
{
	public class BoatSlot : MonoBehaviour
	{
		public Car Car { get; private set; }

		public void SetCar(Car car)
		{
			Car = car;
		}
	}
}