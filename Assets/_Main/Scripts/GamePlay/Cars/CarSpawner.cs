using System.Collections.Generic;
using Fiber.Utilities;
using UnityEngine;
using Utilities;

namespace GamePlay.Cars
{
	public class CarSpawner : Singleton<CarSpawner>
	{
		[SerializeField] private Transform[] spawnPoint;
		[SerializeField] private Transform[] queuePoints;
		[SerializeField] private List<ColorType> carColors = new List<ColorType>();

		private void Awake()
		{
			Spawn();
		}

		private void Spawn()
		{
		}
	}
}