using Fiber.Managers;
using Lofelt.NiceVibrations;
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

		[SerializeField] private BoatSlot[] boatSlots;
		[Space]
		[SerializeField] private LayerMask boatLayerMask;

		public static event UnityAction<Boat> OnBoatTapped;
		public static event UnityAction<Boat> OnBoatMoved;

		private void Awake()
		{
		}

		#region Inputs

		public void OnPointerDown(PointerEventData eventData)
		{
			if (IsInHolder) return;

			SelectedBoat = this;

			Highlight();
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			HapticManager.Instance.PlayHaptic(HapticPatterns.PresetType.RigidImpact);
		}

		#endregion

		private void Highlight()
		{
		}

		#region Editor

		private void SetupEditor()
		{
		}

		#endregion
	}
}