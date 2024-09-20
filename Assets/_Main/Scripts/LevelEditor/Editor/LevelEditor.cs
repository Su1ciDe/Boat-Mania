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
		private const float BUTTON_HEIGHT = 35F;
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
				// EditorGUI.ColorField(new Rect(0, 0, 100, 100), Color.black);
				// EditorGUILayout.ColorField(Color.black, GUILayout.Width(100), GUILayout.Height(BUTTON_HEIGHT));
				selectedColor = (ColorType)EditorGUILayout.EnumPopup(selectedColor, "Dropdown");
				GUILayout.Space(5);
				selectedBoat = (BoatType)EditorGUILayout.EnumPopup(selectedBoat, "Dropdown");
				GUILayout.Space(5);
				if (GUILayout.Button("Spawn", GUILayout.Width(BUTTON_WIDTH), GUILayout.Height(BUTTON_HEIGHT)))
				{
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

			SceneView.RepaintAll();
		}

		private static void Rotate(SceneView scene)
		{
			if (!Selection.activeGameObject || !Selection.activeGameObject.TryGetComponent(out Boat boat)) return;
			var boatPos = scene.camera.WorldToScreenPoint(boat.transform.position);
			GUILayout.BeginArea(new Rect(boatPos.x, scene.camera.pixelHeight - boatPos.y + 25, 100, 100));
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("↖️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, -45);
				}

				if (GUILayout.Button("↗️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 45);
				}
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("↙️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, -135);
				}

				if (GUILayout.Button("↘️", GUILayout.Width(BUTTON_HEIGHT), GUILayout.Height(BUTTON_HEIGHT)))
				{
					boat.transform.eulerAngles = new Vector3(0, 135);
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea();
		}
	}
}