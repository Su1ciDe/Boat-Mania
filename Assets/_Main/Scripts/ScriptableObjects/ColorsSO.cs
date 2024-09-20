using AYellowpaper.SerializedCollections;
using UnityEngine;
using Utilities;

namespace ScriptableObjects
{
	[CreateAssetMenu(fileName = "Colors", menuName = "Boat Mania/Colors", order = 0)]
	public class ColorsSO : ScriptableObject
	{
		public SerializedDictionary<ColorType, Material> BoatColors = new SerializedDictionary<ColorType, Material>();
		public SerializedDictionary<ColorType, Material> CarColors = new SerializedDictionary<ColorType, Material>();
	}
}