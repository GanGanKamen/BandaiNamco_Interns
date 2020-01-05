using UnityEditor;
using UnityEngine;

public class MoveSceneViewCamera
{
    [MenuItem ("Window/シーンビューカメラをリセット")]
    static void PositionCamera ()
    {
		SceneView.lastActiveSceneView.pivot = new Vector3(0f, 12f, 0f);
		SceneView.lastActiveSceneView.rotation = Quaternion.Euler(90f, 0f, 0f);
		SceneView.lastActiveSceneView.orthographic = true;
        SceneView.lastActiveSceneView.size = 200f;
        Selection.activeGameObject = Camera.main.gameObject;
    }
}
