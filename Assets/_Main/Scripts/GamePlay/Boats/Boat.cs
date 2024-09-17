using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities.Extensions;
using Lofelt.NiceVibrations;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GamePlay.Boats
{
	public class Boat : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public static Boat SelectedBoat;

		public bool IsMoving { get; set; }
		public bool IsInHolder { get; set; }

		[Title("References")]
		[SerializeField] private BoatSlot[] boatSlots;
		[SerializeField] private Transform[] rayPoints;
		[SerializeField] private Transform model;
		[Space]
		[SerializeField] private LayerMask boatLayerMask;

		[Title("Parameters")]
		[SerializeField] private float speed = 5;
		[SerializeField] private Vector2 size;
		[Space]
		[SerializeField] private float crashAngle = 10;
		[SerializeField] private float crashDuration = 0.5f;

		private const float HIGHLIGHT_DURATION = .25f;
		private const float HIGHLIGHT_SCALE = 1.25f;

		public static event UnityAction<Boat> OnBoatTapped;
		public static event UnityAction<Boat> OnBoatMoved;

		private void Awake()
		{
		}

		private void Move()
		{
			if (!CheckIfBlockedByCar())
			{
				// TODO: move to path and to holder
			}
		}

		private bool CheckIfBlockedByCar()
		{
			var hitBoats = new List<Boat>();
			var hitDistance = float.MaxValue;

			for (int i = 0; i < rayPoints.Length; i++)
			{
				if (!Physics.Raycast(rayPoints[i].position, rayPoints[i].forward, out var hit, 100, boatLayerMask)) continue;
				if (!hit.rigidbody || !hit.rigidbody.TryGetComponent(out Boat car)) continue;

				hitBoats.AddIfNotContains(car);
				if (hitDistance > hit.distance)
					hitDistance = hit.distance;
			}

			if (hitBoats.Count == 0)
			{
				return false;
			}
			else
			{
				IsMoving = true;
				var prevPos = transform.position;
				transform.DOMove(transform.position + (hitDistance - size.y / 2f) * transform.forward, speed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
				{
					//TODO: crash particle

					for (var i = 0; i < hitBoats.Count; i++)
					{
						hitBoats[i].Crash(this);
					}

					transform.DOMove(prevPos, speed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() => IsMoving = false);
				});

				return true;
			}
		}

		private void Crash(Boat boat)
		{
			if (DOTween.IsTweening(transform, true)) return;

			//TODO: maybe change to be more linear
			var dir = (boat.transform.position - transform.position).normalized;
			transform.DOPunchRotation(crashAngle * dir, crashDuration, 4).SetTarget(transform);
		}

		#region Inputs

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!Player.Player.Instance.CanInput) return;
			if (IsMoving) return;
			if (IsInHolder) return;

			SelectedBoat = this;

			Highlight();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!Player.Player.Instance.CanInput) return;
			if (!SelectedBoat) return;
			if (IsMoving) return;
			if (IsInHolder) return;

			if (eventData.pointerEnter && !eventData.pointerEnter.Equals(SelectedBoat.gameObject))
			{
				HideHighlight();
			}
			else if (eventData.pointerEnter)
			{
				if (IsInHolder)
				{
					SelectedBoat = null;
					return;
				}

				transform.DOComplete();
				HideHighlight();

				Move();

				HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);
			}
			else
			{
				HideHighlight();
			}

			SelectedBoat = null;
		}

		#endregion

		public void Highlight()
		{
			// transform.DOComplete();
			// transform.DOScale(HIGHLIGHT_SCALE, HIGHLIGHT_DURATION).SetEase(Ease.OutBack);
		}

		public void HideHighlight()
		{
			// transform.DOKill();
			// transform.DOScale(1, HIGHLIGHT_DURATION).SetEase(Ease.InBack).OnKill(() => { transform.localScale = Vector3.one; });
		}

		#region Editor

		private void SetupEditor()
		{
		}

		#endregion
	}
}