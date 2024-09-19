using Fiber.Managers;
using UnityEngine;
using Utilities;

namespace GamePlay.Cars
{
	public class Car : MonoBehaviour
	{
		public ColorType ColorType { get; set; }

		[SerializeField] private Renderer[] renderers;

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
	}
}