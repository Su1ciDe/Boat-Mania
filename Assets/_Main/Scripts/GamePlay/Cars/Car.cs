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

		[SerializeField] private Transform model;
		[SerializeField] private Renderer[] renderers;

		[SerializeField] private float speed = 10;
		[SerializeField] private float rotationSpeed = 10;

		private static readonly int idleSpeed = Animator.StringToHash("IdleSpeed");

		private void Awake()
		{
			GetComponent<Animator>().SetFloat(idleSpeed, Random.Range(0.75f, 1.25f));
		}

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
				var mats = renderers[i].materials;
				mats[0] = mat;
				renderers[i].materials = mats;
			}
		}

		public Tween MovePath(Vector3[] path)
		{
			IsMoving = true;
			return transform.DOPath(path, speed).SetSpeedBased(true).OnWaypointChange(value =>
			{
				if (path.Length > value)
				{
					transform.DOLookAt(path[value], .1f);
				}
			}).OnComplete(OnMoveEnd);
		}

		private void OnMoveEnd()
		{
			model.GetChild(0).DOLocalRotate(new Vector3(20f, 0, 0), 0.2f).SetRelative().SetLoops(2, LoopType.Yoyo);

			IsMoving = false;
		}
	}
}