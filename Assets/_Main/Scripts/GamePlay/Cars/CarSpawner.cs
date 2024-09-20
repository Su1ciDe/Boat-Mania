using System.Linq;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.Utilities.Extensions;
using GamePlay.Boats;
using HolderSystem;
using TriInspector;
using UnityEngine;
using Utilities;

namespace GamePlay.Cars
{
	public class CarSpawner : Singleton<CarSpawner>
	{
		public bool IsAdvancing { get; set; }

		[Title("References")]
		[SerializeField] private Transform spawnPoint;
		[SerializeField] private Transform exitPoint;
		[SerializeField] private Transform[] queuePoints;

		[Title("Editor")]
		[SerializeField] private List<ColorType> carColors = new List<ColorType>();
#if UNITY_EDITOR
		[SerializeField] private Randomizer[] randomizer;
		[SerializeField] private int randomCarCount;
#endif

		private readonly Queue<Car> carQueue = new Queue<Car>();

		private void OnEnable()
		{
			LevelManager.OnLevelLoad += OnLevelLoaded;
			Boat.OnBoatArrivedAny += OnBoatArrived;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelLoad -= OnLevelLoaded;
			Boat.OnBoatArrivedAny -= OnBoatArrived;
		}

		private void OnLevelLoaded()
		{
			var queuePositions = queuePoints.Select(x => x.position).Reverse().ToArray();
			for (var i = 0; i < queuePoints.Length; i++)
			{
				if (carColors.Count <= 0) return;

				var car = SpawnCar(carColors[0]);
				car.MovePath(queuePositions[..^i]).SetDelay(i * .1f);
				carQueue.Enqueue(car);
				carColors.RemoveAt(0);
			}
		}

		private Car SpawnCar(ColorType carColor)
		{
			var car = Instantiate(GameManager.Instance.PrefabsSO.CarPrefab, spawnPoint.position, spawnPoint.rotation, transform);
			car.gameObject.name = $"Car_{carColors.Count}";
			car.Setup(carColor);
			return car;
		}

		private void OnBoatArrived(Boat boat)
		{
			FillBoat(boat);
		}

		public void AdvanceLine(int advanceAmount)
		{
			var queuePositions = queuePoints.Select(x => x.position).Reverse().ToArray();
			IsAdvancing = true;
			Tween tween = null;

			int i = 0;
			foreach (var car in carQueue)
			{
				var path = new List<Vector3>();
				for (int j = i; j < i + advanceAmount; j++)
				{
					path.Add(queuePoints[j].position);
				}

				path.Reverse();
				tween = car.MovePath(path.ToArray());

				i++;
			}

			var spawnAmount = Mathf.Clamp(advanceAmount, 0, carColors.Count);
			for (int j = 0; j < spawnAmount; j++)
			{
				var car = SpawnCar(carColors[0]);
				carColors.RemoveAt(0);

				car.MovePath(queuePositions[..(spawnAmount - j)]).SetDelay(j * .1f);
				carQueue.Enqueue(car);
			}

			if (tween is not null)
			{
				tween.onComplete += () =>
				{
					IsAdvancing = false;

					if (carQueue.TryPeek(out var car))
						CheckHolders(car.ColorType);
				};
			}
		}

		private void CheckHolders(ColorType carColor)
		{
			var boat = Holder.Instance.GetBoatByType(carColor);
			if (boat && !boat.IsMoving && !boat.IsCompleted)
			{
				FillBoat(boat);
			}
		}

		private void FillBoat(Boat boat)
		{
			var emptySlotCount = boat.GetEmptySlotCount();
			var carCount = GetCarCountByType(boat.ColorType, emptySlotCount);
			var path = new List<Vector3> { exitPoint.position, new Vector3(boat.transform.position.x, exitPoint.position.y, exitPoint.position.z), boat.transform.position };
			boat.IsLoadingCars = true;
			Tween tween = null;
			for (var i = 0; i < carCount; i++)
			{
				var car = carQueue.Dequeue();

				if (!car.transform.position.Equals(queuePoints[0].position))
					path.Insert(0, queuePoints[0].position);

				tween = car.MovePath(path.ToArray());
				tween.onComplete += () => boat.SetCar(car);
			}

			if (tween is not null)
				tween.onComplete += () => boat.IsLoadingCars = false;

			AdvanceLine(carCount);
		}

		#region Helpers

		public List<Car> GetCarsByType(ColorType carColor, int maxCount)
		{
			var cars = new List<Car>();

			foreach (var car in carQueue)
			{
				if (car.ColorType == carColor)
				{
					cars.Add(car);
					if (cars.Count.Equals(maxCount))
						return cars;
				}
				else
				{
					return cars;
				}
			}

			return cars;
		}

		public int GetCarCountByType(ColorType carColor, int maxCount)
		{
			var count = 0;
			foreach (var car in carQueue)
			{
				if (car.ColorType == carColor)
				{
					count++;
					if (count.Equals(maxCount))
						return count;
				}
				else
					return count;
			}

			return count;
		}

		#endregion

		#region Randomizer

#if UNITY_EDITOR
		[DeclareHorizontalGroup("Randomizer")]
		[System.Serializable]
		private class Randomizer
		{
			[Group("Randomizer")] public ColorType ColorType;
			[Group("Randomizer")] public int Weight;
			[Group("Randomizer"), HideLabel, DisplayAsString] public string Percent;
		}

		[Button]
		private void Randomize()
		{
			carColors.Clear();

			var weight = randomizer.Select(x => x.Weight).ToList();
			var randomCars = randomizer.Select(x => x.ColorType).ToList();

			for (int i = 0; i < randomCarCount; i++)
			{
				carColors.Add(randomCars.WeightedRandom(weight));
			}
		}

		private void OnValidate()
		{
			var totalWeight = 0;
			foreach (var randomOption in randomizer)
			{
				totalWeight += randomOption.Weight;
			}

			foreach (var gridSpawnerOption in randomizer)
			{
				gridSpawnerOption.Percent = ((float)gridSpawnerOption.Weight / totalWeight * 100).ToString("F2") + "%";
			}
		}
#endif

		#endregion
	}
}