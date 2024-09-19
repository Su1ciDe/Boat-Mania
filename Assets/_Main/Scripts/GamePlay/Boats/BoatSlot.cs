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
			car.transform.SetParent(transform);
			car.transform.localPosition = Vector3.zero;
			car.transform.localRotation = Quaternion.identity;
		}
	}
}