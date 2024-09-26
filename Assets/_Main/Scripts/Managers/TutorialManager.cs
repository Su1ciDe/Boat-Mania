using System.Collections;
using Fiber.UI;
using Fiber.Managers;
using Fiber.Utilities;
using GamePlay.Player;
using UnityEngine;

namespace Managers
{
	public class TutorialManager : MonoBehaviour
	{
		private TutorialUI tutorialUI => TutorialUI.Instance;

		private void OnEnable()
		{
			LevelManager.OnLevelStart += OnLevelStarted;
			LevelManager.OnLevelUnload += OnLevelUnloaded;
		}

		private void OnDisable()
		{
			LevelManager.OnLevelStart -= OnLevelStarted;
			LevelManager.OnLevelUnload -= OnLevelUnloaded;
		}

		private void OnDestroy()
		{
			Unsub();
		}

		private void OnLevelUnloaded()
		{
			Unsub();
		}

		private void OnLevelStarted()
		{
			if (LoadingPanelController.Instance && LoadingPanelController.Instance.IsActive)
			{
				StartCoroutine(WaitLoadingScreen());
			}
			else
			{
				LevelStart();
			}
		}

		private void Unsub()
		{
			StopAllCoroutines();

			if (TutorialUI.Instance)
			{
				tutorialUI.HideFocus();
				tutorialUI.HideHand();
				tutorialUI.HideText();
				tutorialUI.HideFakeButton();
			}
		}

		private IEnumerator WaitLoadingScreen()
		{
			yield return new WaitUntilAction(ref LoadingPanelController.Instance.OnLoadingFinished);

			LevelStart();
		}

		private void LevelStart()
		{
			if (LevelManager.Instance.LevelNo.Equals(1))
			{
				StartCoroutine(Level1Tutorial());
			}

			if (LevelManager.Instance.LevelNo.Equals(2))
			{
				StartCoroutine(Level2Tutorial());
			}
		}

		#region Level 1

		private IEnumerator Level1Tutorial()
		{
			Player.Instance.CanInput = false;
			tutorialUI.SetBlocker(true);

			yield return new WaitForSeconds(0.5f);
			Player.Instance.CanInput = false;

			var boat = BoatManager.Instance.Boats[0];
			var pos = boat.transform.position;

			tutorialUI.ShowText("Tap to move the boats.");
			tutorialUI.ShowFocus(pos, Helper.MainCamera);
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				StartCoroutine(OnBoatMoveLevel1_1());
			}, pos, Helper.MainCamera);
		}

		private IEnumerator OnBoatMoveLevel1_1()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			yield return new WaitForSeconds(0.5f);

			tutorialUI.ShowFocus(Vector3.zero, Helper.MainCamera, false, 0, 2);
			tutorialUI.ShowText("Fill the boats with the cars in the same color!");

			yield return new WaitForSeconds(2);

			var boat = BoatManager.Instance.Boats[1];
			var pos = boat.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				StartCoroutine(OnBoatMoveLevel1_2());
			}, pos, Helper.MainCamera);
		}

		private IEnumerator OnBoatMoveLevel1_2()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			yield return null;

			var boat = BoatManager.Instance.Boats[2];
			var pos = boat.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				StartCoroutine(OnBoatMoveLevel1_3());
			}, pos, Helper.MainCamera);
		}

		private IEnumerator OnBoatMoveLevel1_3()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			yield return null;

			var boat = BoatManager.Instance.Boats[3];
			var pos = boat.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				Level1TutorialComplete();
			}, pos, Helper.MainCamera);
		}

		private void Level1TutorialComplete()
		{
			tutorialUI.HideText();
			tutorialUI.HideFocus();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			tutorialUI.SetBlocker(false);
			Player.Instance.CanInput = true;
		}

		#endregion

		#region Level 2

		private IEnumerator Level2Tutorial()
		{
			Player.Instance.CanInput = false;
			tutorialUI.SetBlocker(true);

			yield return new WaitForSeconds(0.5f);
			Player.Instance.CanInput = false;

			var boat = BoatManager.Instance.Boats[1];
			var pos = boat.transform.position;

			tutorialUI.ShowFocus(pos, Helper.MainCamera);
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				StartCoroutine(OnBoatMoveLevel2_1());
			}, pos, Helper.MainCamera);
		}

		private IEnumerator OnBoatMoveLevel2_1()
		{
			tutorialUI.HideText();
			tutorialUI.HideFocus();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			yield return new WaitForSeconds(0.5f);

			tutorialUI.ShowText("The boats need their way clear to move!");
			tutorialUI.ShowFocus(Vector3.zero, Helper.MainCamera, false, 0, 2);

			yield return new WaitForSeconds(2);

			var boat = BoatManager.Instance.Boats[0];
			var pos = boat.transform.position;

			tutorialUI.ShowFocus(pos, Helper.MainCamera);
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				StartCoroutine(OnBoatMoveLevel2_2());
			}, pos, Helper.MainCamera);
		}

		private IEnumerator OnBoatMoveLevel2_2()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			yield return null;

			var boat = BoatManager.Instance.Boats[1];
			var pos = boat.transform.position;
			tutorialUI.ShowTap(pos, Helper.MainCamera);
			tutorialUI.SetupFakeButton(() =>
			{
				boat.Move();
				Level2TutorialComplete();
			}, pos, Helper.MainCamera);
		}

		private void Level2TutorialComplete()
		{
			tutorialUI.HideFocus();
			tutorialUI.HideText();
			tutorialUI.HideHand();
			tutorialUI.HideFakeButton();

			tutorialUI.SetBlocker(false);
			Player.Instance.CanInput = true;
		}

		#endregion
	}
}