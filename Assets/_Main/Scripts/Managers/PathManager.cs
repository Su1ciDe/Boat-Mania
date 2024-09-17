using Fiber.Utilities;
using PathCreation;
using UnityEngine;

namespace Managers
{
	public class PathManager : Singleton<PathManager>
	{
		[SerializeField] private PathCreator[] paths;
	}
}