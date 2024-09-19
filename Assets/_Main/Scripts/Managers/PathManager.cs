using Fiber.Utilities;
using PathCreation;
using UnityEngine;

namespace Managers
{
	public class PathManager : Singleton<PathManager>
	{
		[SerializeField] private PathCreator[] paths;
		[SerializeField] private float threshold = 0.1f;

		public (PathCreator path, Vector3 point) FindPath(Vector3 boatPosition)
		{
			for (var i = 0; i < paths.Length; i++)
			{
				var point = paths[i].path.GetClosestPointOnPath(boatPosition);
				if (Mathf.Abs((point - boatPosition).sqrMagnitude) < threshold)
				{
					return (paths[i], point);
				}
			}

			return (null, default);
		}
	}
}