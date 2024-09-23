using System;
using GamePlay.Boats;
using Managers;
using UnityEditor;
using UnityEngine;
using Utilities;

namespace LevelEditor
{
	[InitializeOnLoad]
	public class LevelEditor : Editor
	{
		private const float BUTTON_HEIGHT = 25f;
		private const float BUTTON_WIDTH = 135F;

		private static ColorType selectedColor;
		private static BoatType selectedBoat;

		static LevelEditor()
		{
			SceneView.duringSceneGui -= OnDuringSceneGui;
			SceneView.duringSceneGui += OnDuringSceneGui;

			if (!BoatManager.Instance) return;
		}

		private static void OnDuringSceneGui(SceneView scene)
		{
			Handles.BeginGUI();
			{
				Spawn();
				Rotate(scene);
				HotKeys(scene);
			}
			Handles.EndGUI();
		}

		private static void Spawn()
		{
			if (!BoatManager.Instance) return;
			GUILayout.BeginArea(new Rect(5, 5, 135, 200));
			{
				GUI.color = GetColor(selectedColor);
				selectedColor = (ColorType)EditorGUILayout.EnumPopup(selectedColor, "Dropdown");
				GUILayout.Space(5);
				GUI.color = Color.white;
				selectedBoat = (BoatType)EditorGUILayout.EnumPopup(selectedBoat, "Dropdown");
				GUILayout.Space(5);
				if (GUILayout.Button("Spawn", GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT)))
				{
					if (selectedBoat == BoatType.None) return;

					var boat = BoatManager.Instance.SpawnBoat(selectedColor, selectedBoat);
					Selection.activeGameObject = boat.gameObject;
				}
			}
			GUILayout.EndArea();
		}

		private static void HotKeys(SceneView scene)
		{
			if (!scene.hasFocus) return;

			var e = Event.current;
			if (e.type != EventType.KeyDown) return;

			selectedColor = Event.current.keyCode switch
			{
				KeyCode.Alpha1 => ColorType._1Blue,
				KeyCode.Alpha2 => ColorType._2Green,
				KeyCode.Alpha3 => ColorType._3Orange,
				KeyCode.Alpha4 => ColorType._4Pink,
				KeyCode.Alpha5 => ColorType._5Purple,
				KeyCode.Alpha6 => ColorType._6Red,
				KeyCode.Alpha7 => ColorType._7Yellow,
				_ => selectedColor
			};

			selectedBoat = Event.current.keyCode switch
			{
				KeyCode.A => BoatType._4,
				KeyCode.S => BoatType._6,
				KeyCode.D => BoatType._10,
				_ => selectedBoat
			};

			if (Event.current.keyCode == KeyCode.Space)
			{
				Spawn();
			}

			SceneView.RepaintAll();
		}

		private static void Rotate(SceneView scene)
		{
			if (!Selection.activeGameObject || !Selection.activeGameObject.TryGetComponent(out Boat boat)) return;
			var boatPos = scene.camera.WorldToScreenPoint(boat.transform.position);
			GUILayout.BeginArea(new Rect(boatPos.x, scene.camera.pixelHeight - boatPos.y + 25, 125, 125));
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("↖️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, -45);
				}

				if (GUILayout.Button("↑", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 0);
				}

				if (GUILayout.Button("↗️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 45);
				}

				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("←", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, -90);
				}

				GUILayout.Space(BUTTON_HEIGHT + 3 );

				if (GUILayout.Button("→", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 90);
				}

				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("↙️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, -135);
				}

				if (GUILayout.Button("↓", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 180);
				}

				if (GUILayout.Button("↘️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 135);
				}

				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
		}

		private static Color GetColor(ColorType colorType)
		{
			var color = selectedColor switch
			{
				ColorType._1Blue => Color.blue,
				ColorType._2Green => Color.green,
				ColorType._3Orange => new Color(1f, 0.5f, 0),
				ColorType._4Pink => Color.magenta,
				ColorType._5Purple => new Color(.7f, .25f, 1f),
				ColorType._6Red => Color.red,
				ColorType._7Yellow => Color.yellow,
				_ => throw new ArgumentOutOfRangeException()
			};

			return color;
		}
	}
}