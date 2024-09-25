using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Fiber.Managers;
using Fiber.Utilities;
using Fiber.AudioSystem;
using Fiber.Utilities.Extensions;
using GamePlay.Cars;
using HolderSystem;
using Lofelt.NiceVibrations;
using Managers;
using PathCreation;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Utilities;

namespace GamePlay.Boats
{
	[SelectionBase]
	public class Boat : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		public static Boat SelectedBoat;

		public bool IsMoving { get; set; }
		public HolderSlot CurrentHolder { get; set; }
		public bool IsLoadingCars { get; set; }
		public bool IsCompleted { get; set; }

		[field: Title("Properties")]
		[field: SerializeField, ReadOnly] public BoatType BoatType;
		[field: SerializeField] public ColorType ColorType { get; private set; }

		[Title("References")]
		[SerializeField] private BoatSlot[] boatSlots;
		[SerializeField] private Transform[] rayPoints;
		[SerializeField] private Transform model;
		[SerializeField] private Renderer[] renderers;
		[SerializeField] private Transform enterPoint;
		public Transform EnterPoint => enterPoint;
		[SerializeField] private Transform ramp;
		[SerializeField] private float rampSize;
		[SerializeField] private Collider col;
		[SerializeField] private GameObject cover;
		[SerializeField] private GameObject arrow;
		[SerializeField] private Transform[] propellers;

		[Title("Parameters")]
		[SerializeField] private float speed = 5;
		[SerializeField] private float rotationSpeed = 10;
		[SerializeField] private Vector2 size;
		[Space]
		[SerializeField] private LayerMask boatLayerMask;
		[Space]
		[SerializeField] private float crashAngle = 10;
		[SerializeField] private float crashDuration = 0.5f;

		private const float HIGHLIGHT_DURATION = .25f;
		private const float HIGHLIGHT_SCALE = 1.25f;
		private const float PATH_END_LINE = 0.82f;
		private const float ROTATION_DURATION = .1f;

		private static readonly int idleSpeed = Animator.StringToHash("IdleSpeed");

		public event UnityAction OnBoatArrived;
		public static event UnityAction<Boat> OnBoatTapped;
		public static event UnityAction<Boat> OnBoatArrivedAny;

		private void Awake()
		{
			GetComponent<Animator>().SetFloat(idleSpeed, Random.Range(0.75f, 1.25f));
		}

		private void Move()
		{
			// Check if the boat can move. If it can't, crash into the boat in front
			if (CheckIfBlockedByCar()) return;

			var slot = Holder.Instance.GetFirstEmptySlot();
			if (!slot)
			{
				//TODO: show message
				return;
			}

			slot.SetBoat(this);
			CurrentHolder = slot;

			col.enabled = false;
			IsMoving = true;
			MovePropeller();

			transform.DOMove(transform.position + 100 * transform.forward, speed).SetEase(Ease.Linear).SetSpeedBased(true).OnUpdate(() =>
			{
				var foundPath = PathManager.Instance.FindPath(transform.position);
				if (foundPath.path is not null)
				{
					StartCoroutine(MoveToHolder(foundPath.path.path, foundPath.point, slot));
				}
			});
		}

		private IEnumerator MoveToHolder(VertexPath path, Vector3 point, HolderSlot holderSlot)
		{
			transform.DOKill();

			var dist = path.GetClosestDistanceAlongPath(point);
			var pos = path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
			var prevPos = Vector3.zero;
			while (prevPos != pos)
			{
				if (path.GetClosestTimeOnPath(pos) > PATH_END_LINE)
					break;

				transform.position = pos;
				transform.rotation = Quaternion.Lerp(transform.rotation, path.GetRotationAtDistance(dist, EndOfPathInstruction.Stop), rotationSpeed * Time.deltaTime);
				dist += speed * Time.deltaTime;
				yield return null;

				prevPos = pos;
				pos = path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
			}

			var movePos = new Vector3(holderSlot.transform.position.x, transform.position.y, transform.position.z);
			transform.DOLookAt(movePos, ROTATION_DURATION).SetId("rotation");
			transform.DOMove(movePos, speed).SetSpeedBased(true).OnComplete(() =>
			{
				DOTween.Kill("rotation");
				transform.DORotate(holderSlot.transform.eulerAngles, ROTATION_DURATION).SetId("rotation");
				transform.DOMove(holderSlot.transform.position, speed).SetSpeedBased(true).OnComplete(OnArrived);
			});
		}

		private void OnArrived()
		{
			IsMoving = false;

			transform.localScale = 1.5f * Vector3.one;
			StopPropeller();
			cover.transform.DOScale(0, .25f).OnComplete(() => cover.SetActive(false));
			ramp.gameObject.SetActive(true);
			ramp.DOScale(rampSize * Vector3.forward, .25f).SetRelative(true);
			arrow.SetActive(false);

			OnBoatArrived?.Invoke();
			OnBoatArrivedAny?.Invoke(this);
		}

		private bool CheckIfBlockedByCar()
		{
			var hitBoats = new List<(Boat boat, float hitDistance)>();
			var hitDistance = float.MaxValue;

			for (int i = 0; i < rayPoints.Length; i++)
			{
				if (!Physics.Raycast(rayPoints[i].position, rayPoints[i].forward, out var hit, 100, boatLayerMask)) continue;
				if (!hit.rigidbody || !hit.rigidbody.TryGetComponent(out Boat car)) continue;

				hitBoats.AddIfNotContains((car, hit.distance));
				if (hitDistance > hit.distance)
					hitDistance = hit.distance;
			}

			if (hitBoats.Count == 0) return false;

			IsMoving = true;
			MovePropeller();

			var prevPos = transform.position;
			transform.DOMove(transform.position + (hitDistance - size.y / 2f) * transform.forward, speed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
			{
				var crashPos = transform.position + size.y / 2f * transform.forward;
				ParticlePooler.Instance.Spawn("Crash", crashPos, transform.rotation);
				AudioManager.Instance.PlayAudio(AudioName.Crash).SetVolume(0.6f);

				for (var i = 0; i < hitBoats.Count; i++)
				{
					if (Mathf.Approximately(hitDistance, hitBoats[i].hitDistance))
					{
						hitBoats[i].boat.Crash(this);
					}
				}

				transform.DOMove(prevPos, speed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
				{
					IsMoving = false;
					StopPropeller();
				});
			});

			return true;
		}

		private void Crash(Boat boat)
		{
			if (DOTween.IsTweening(transform, true)) return;

			var dir = (boat.transform.position - transform.position).normalized;
			transform.DOPunchRotation(crashAngle * dir, crashDuration, 7).SetTarget(transform);
		}

		private void ChangeColor(ColorType colorType)
		{
			if (!GameManager.Instance) return;
			ColorType = colorType;
			var mat = GameManager.Instance.ColorsSO.BoatColors[colorType];
			for (var i = 0; i < renderers.Length; i++)
			{
				if (Application.isPlaying)
				{
					renderers[i].material = mat;
				}
				else
				{
					renderers[i].sharedMaterial = mat;
				}
			}
		}

		public BoatSlot SetCar(Car car, bool setPosition = true)
		{
			var slot = GetFirstEmptySlot();
			if (!slot) return null;

			slot.SetCar(car, setPosition);
			car.CurrentSlot = slot;

			if (GetEmptySlotCount().Equals(0))
			{
				IsCompleted = true;

				StartCoroutine(ExitFromHolder());
			}

			return slot;
		}

		private IEnumerator ExitFromHolder()
		{
			yield return new WaitUntil(() => !IsLoadingCars);
			yield return null;
			yield return new WaitUntil(() => !IsAnyCarMoving());
			yield return null;

			ramp.DOScale(new Vector3(ramp.localScale.x, ramp.localScale.y, 0), .2f).OnComplete(() => ramp.gameObject.SetActive(false));

			CurrentHolder.Boat = null;
			CurrentHolder = null;

			CarSpawner.Instance.StopCheckWin();

			AudioManager.Instance.PlayAudio(AudioName.BoatMove);

			var exitPosition = Holder.Instance.ExitPoint.transform.position;
			var pos = transform.position + (transform.position.z - exitPosition.z) * -transform.forward;
			transform.DOMove(pos, speed).SetSpeedBased(true).OnComplete(() =>
			{
				transform.DOLookAt(new Vector3(-exitPosition.x, exitPosition.y, exitPosition.z), 0.1f, AxisConstraint.None, Vector3.up);
				transform.DOMove(exitPosition, speed).SetSpeedBased(true).OnComplete(() =>
				{
					transform.DOKill();
					Destroy(gameObject);
				});
			});
		}

		public void SetToSlotPosition(Car car)
		{
			car.CurrentSlot.SetPosition(car);
		}

		private void MovePropeller()
		{
			for (var i = 0; i < propellers.Length; i++)
				propellers[i].DOLocalRotate(360 * Vector3.forward, .5f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart);
		}

		private void StopPropeller()
		{
			for (var i = 0; i < propellers.Length; i++)
				propellers[i].DOKill();
		}

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

		#region Helpers

		public int GetEmptySlotCount()
		{
			int count = 0;
			for (var i = 0; i < boatSlots.Length; i++)
			{
				if (!boatSlots[i].Car)
					count++;
			}

			return count;
		}

		public BoatSlot GetFirstEmptySlot()
		{
			for (var i = 0; i < boatSlots.Length; i++)
			{
				if (!boatSlots[i].Car)
					return boatSlots[i];
			}

			return null;
		}

		public int GetSlotIndex(BoatSlot slot)
		{
			for (var i = 0; i < boatSlots.Length; i++)
			{
				if (boatSlots[i].Equals(slot))
					return i;
			}

			return -1;
		}

		#endregion

		#region Inputs

		public void OnPointerDown(PointerEventData eventData)
		{
			if (!Player.Player.Instance.CanInput) return;
			if (IsMoving) return;
			if (CurrentHolder) return;

			SelectedBoat = this;

			Highlight();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if (!Player.Player.Instance.CanInput) return;
			if (!SelectedBoat) return;
			if (IsMoving) return;
			if (CurrentHolder) return;

			if (eventData.pointerEnter && !eventData.pointerEnter.Equals(SelectedBoat.gameObject))
			{
				HideHighlight();
			}
			else if (eventData.pointerEnter)
			{
				if (CurrentHolder)
				{
					SelectedBoat = null;
					return;
				}

				transform.DOComplete();
				HideHighlight();

				Move();
				OnBoatTapped?.Invoke(this);

				HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);
			}
			else
			{
				HideHighlight();
			}

			SelectedBoat = null;
		}

		#endregion

		public bool IsAnyCarMoving()
		{
			for (int i = 0; i < boatSlots.Length; i++)
			{
				if (boatSlots[i].Car && boatSlots[i].Car.IsMoving)
					return true;
			}

			return false;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			ChangeColor(ColorType);
		}
#endif

		#region Editor

		public void SetupEditor(ColorType colorType)
		{
			ChangeColor(colorType);
		}

		#endregion
	}
}