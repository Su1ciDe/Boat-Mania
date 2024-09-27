using DG.Tweening;
using Fiber.Utilities;
using TMPro;
using UnityEngine;

namespace UI
{
	public class WarningUI : Singleton<WarningUI>
	{
		[SerializeField] private Transform warningPanel;
		[SerializeField] private TMP_Text txtWarning;

		public void ShowWarning(string message, float duration = 2, bool animated = false)
		{
			warningPanel.gameObject.SetActive(true);
			txtWarning.SetText(message);

			if (animated)
				warningPanel.DOScale(1.25f, .5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

			DOVirtual.DelayedCall(duration, HideWarning);
		}

		public void HideWarning()
		{
			warningPanel.gameObject.SetActive(false);
			warningPanel.DOKill();

			warningPanel.localScale = Vector3.one;
		}
	}
}