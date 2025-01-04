#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(QuestCanvas))]
public class QuestCanvasEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        QuestCanvas questCanvas = (QuestCanvas)target; 

        if (GUILayout.Button("Initialize Quest"))
        {
            questCanvas.Init(); 
            Debug.Log("Quest Initialized!"); 
        }
    }
}
#endif