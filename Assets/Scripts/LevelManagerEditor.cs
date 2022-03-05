using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
    string mapname = "";
    int maxcolor = Entity.color.Length;

    private void OnEnable()
    {
        LevelManager controller = (LevelManager)target;
        List<Level> levels = LevelManager.GetLevelList();

        if (levels.Count > controller.CurrentLevel)
        {
            mapname = levels[controller.CurrentLevel].MapName;
            maxcolor = levels[controller.CurrentLevel].MaxColor;
        }
    }

    void OnSceneGUI()
    {
        LevelManager controller = (LevelManager)target;
        
        Handles.BeginGUI();
        {
            GUIStyle style = new GUIStyle("box");
            GUILayout.BeginArea(new Rect(10, 10, 350, 200), style);
            {
                GUILayout.Label("LEVEL SAVER");  
                mapname = GUILayout.TextField(mapname);
                if (GUILayout.Button("Save level"))
                {
                    controller.SaveCurrentMap(mapname, maxcolor);
                }
                List<Level> levels = LevelManager.GetLevelList();
                string[] options = new string[levels.Count];
                for (int i = 0; i < levels.Count; i++) options[i] = levels[i].MapName;

                GUILayout.Label("");
                GUILayout.Label("LEVEL MANAGER");
                if (levels.Count > controller.CurrentLevel)
                {
                    int index = EditorGUILayout.Popup("Current level: ", controller.CurrentLevel, options);
                    maxcolor = EditorGUILayout.IntSlider(maxcolor, 1, Entity.color.Length);
                    if (index != controller.CurrentLevel)
                    {
                        mapname = levels[index].MapName;
                        maxcolor = levels[index].MaxColor;
                        controller.SetLevel(index);
                    }
                }
                if (GUILayout.Button("Delete level"))
                {
                    controller.DeleteLevel(levels[controller.CurrentLevel]);
                    controller.SetLevel(0);
                }
            }
            GUILayout.EndArea();
        }
        Handles.EndGUI();
    }
}

#endif