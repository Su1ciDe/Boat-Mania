using DG.Tweening;
using Fiber.Managers;
using GamePlay.Boats;
using UnityEngine;
using Utilities;

namespace GamePlay.Cars
{
	[SelectionBase]
	public class Car : MonoBehaviour
	{
		public bool IsMoving { get; set; }
		public ColorType ColorType { get; set; }
		public BoatSlot CurrentSlot { get; set; }

		[SerializeField] private Renderer[] renderers;

		[SerializeField] private float speed = 10;
		[SerializeField] private float rotationSpeed = 10;

		public void Setup(ColorType colorType)
		{
			ColorType = colorType;
			ChangeColor(ColorType);
		}

		private void ChangeColor(ColorType colorType)
		{
			var mat = GameManager.Instance.ColorsSO.CarColors[colorType];
			for (var i = 0; i < renderers.Length; i++)
			{
				renderers[i].material = mat;
			}
		}

		public Tween MovePath(Vector3[] path)
		{
			IsMoving = true;
			return transform.DOPath(path, speed).SetSpeedBased(true).OnWaypointChange(value =>
			{
				// DOTween.Kill(gameObject.name + "_rotate");
				if (path.Length > value)
				{
					transform.DOLookAt(path[value], .1f);
				}
			}).OnComplete(() => IsMoving = false);
		}
	}
}