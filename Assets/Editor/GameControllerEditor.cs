using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameController))]
public class GameControllerEditor : Editor {

	// public override void OnInspectorGUI () {
	// }

	void OnSceneGUI () {
		GameController c = target as GameController;
		Transform transform = c.transform;

		c.monsterSpawnY = transform.InverseTransformPoint(Handles.PositionHandle(
			transform.TransformPoint(new Vector3(0, c.monsterSpawnY, 0)), transform.rotation)).y;

		Vector3 monsterSpawnLeftPos = new Vector3(c.monsterSpawnMinX, c.monsterSpawnY, 0);
		Vector3 monsterSpawnRightPos = new Vector3(c.monsterSpawnMaxX, c.monsterSpawnY, 0);
		Handles.DrawLine(monsterSpawnLeftPos, monsterSpawnRightPos);

		Handles.BeginGUI();

		Camera.current.WorldToScreenPoint(transform.position + new Vector3(0, c.monsterSpawnY, 0));

		Handles.EndGUI();
	}
}
