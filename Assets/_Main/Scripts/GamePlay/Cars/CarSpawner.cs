using System.Linq;
using System.Collections;
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
		[TableList(Draggable = false, AlwaysExpanded = true, HideAddButton = true, HideRemoveButton = true, ShowElementLabels = false)]
		[SerializeField] private List<CarCount> carCounts = new List<CarCount>();
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
			if (IsAdvancing) return;

			FillBoat(boat);

			CheckWin();
		}

		public void AdvanceLine(int advanceAmount)
		{
			var queuePositions = queuePoints.Select(x => x.position).Reverse().ToArray();
			if (carQueue.Count > 0)
				IsAdvancing = true;

			Tween tween = null;

			int i = 0;
			foreach (var car in carQueue)
			{
				var path = new List<Vector3>();
				for (int j = i; j < i + advanceAmount; j++)
					path.Add(queuePoints[j].position);

				path.Reverse();
				tween = car.MovePath(path.ToArray());

				i++;
			}

			var spawnAmount = Mathf.Clamp(advanceAmount, 0, carColors.Count);
			for (int j = 0; j < spawnAmount; j++)
			{
				var car = SpawnCar(carColors[0]);
				carColors.RemoveAt(0);

				car.MovePath(queuePositions[..(advanceAmount - j)]).SetDelay(j * .1f);
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
			if (!boat || boat.IsMoving) return;

			FillBoat(boat);
		}

		private void FillBoat(Boat boat)
		{
			if (boat.IsCompleted) return;

			var emptySlotCount = boat.GetEmptySlotCount();
			var carCount = GetCarCountByType(boat.ColorType, emptySlotCount);
			var path = new List<Vector3> { exitPoint.position, new Vector3(boat.transform.position.x, exitPoint.position.y, exitPoint.position.z), boat.EnterPoint.position };
			if (carCount > 0)
				boat.IsLoadingCars = true;
			Tween tween = null;
			for (var i = 0; i < carCount; i++)
			{
				var car = carQueue.Dequeue();

				if (!car.transform.position.Equals(queuePoints[0].position))
					path.Insert(0, queuePoints[0].position);

				var slot = boat.SetCar(car, false);
				var tempPath = new List<Vector3>(path) { slot.transform.position };
				tween = car.MovePath(tempPath.ToArray());
				tween.onComplete += () => boat.SetToSlotPosition(car);
			}

			if (tween is not null)
				tween.onComplete += () => boat.IsLoadingCars = false;

			AdvanceLine(carCount);
		}

		private Coroutine checkWinCoroutine;

		private void CheckWin()
		{
			if (checkWinCoroutine is not null)
			{
				StopCoroutine(checkWinCoroutine);
				checkWinCoroutine = null;
			}

			checkWinCoroutine = StartCoroutine(CheckWinCoroutine());
		}

		private IEnumerator CheckWinCoroutine()
		{
			yield return null;
			yield return new WaitUntil(() => !IsAdvancing);
			yield return new WaitForSeconds(1);

			if (carQueue.Count.Equals(0) && carColors.Count.Equals(0))
			{
				LevelManager.Instance.Win();
			}

			yield return new WaitUntil(() => !Holder.Instance.IsAnyBoatLoadingCars());
			yield return null;

			if (!Holder.Instance.GetFirstEmptySlot())
			{
				LevelManager.Instance.Lose();
			}
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

		[System.Serializable]
		private class CarCount
		{
			[ReadOnly] public ColorType ColorType;
			[ReadOnly] public int Count;

			public CarCount(ColorType color, int count)
			{
				ColorType = color;
				Count = count;
			}
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

			CalculateCars();
		}

		private void OnValidate()
		{
			var totalWeight = 0;
			foreach (var randomOption in randomizer)
				totalWeight += randomOption.Weight;

			foreach (var gridSpawnerOption in randomizer)
				gridSpawnerOption.Percent = ((float)gridSpawnerOption.Weight / totalWeight * 100).ToString("F2") + "%";

			CalculateCars();
		}

		private void CalculateCars()
		{
			carCounts.Clear();
			foreach (var goalOption in carColors)
			{
				var found = false;

				var goalCount = carCounts.Where(x => x.ColorType == goalOption);
				foreach (var count in goalCount)
				{
					count.Count++;
					found = true;
				}

				if (!found)
					carCounts.Add(new CarCount(goalOption, 1));
			}
		}
#endif

		#endregion
	}
}