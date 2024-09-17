using AYellowpaper.SerializedCollections;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Colors", menuName = "Boat Mania/Colors", order = 0)]
	public class ColorsSO : ScriptableObject
	{
		public SerializedDictionary<ColorType, Material> Colors = new SerializedDictionary<ColorType, Material>();
	}
}