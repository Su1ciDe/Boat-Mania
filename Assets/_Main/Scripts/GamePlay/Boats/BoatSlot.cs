using GamePlay.Cars;
using UnityEngine;

namespace GamePlay.Boats
{
	public class BoatSlot : MonoBehaviour
	{
		public Car Car { get; private set; }

		public void SetCar(Car car, bool setPosition = true)
		{
			Car = car;
			Car.transform.SetParent(transform);
			if (setPosition)
			{
				SetPosition(Car);
			}
		}

		public void SetPosition(Car car)
		{
			car.transform.localPosition = Vector3.zero;
			car.transform.localRotation = Quaternion.identity;
		}
	}
}