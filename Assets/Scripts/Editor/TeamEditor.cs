using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TeamManager))]
public class TeamEditor : Editor
{
    TeamManager tm;

    private void OnEnable()
    {
        tm = (TeamManager)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.BeginVertical("Box");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Team count: " + tm.Teams.Count);
        if (GUILayout.Button("Add team"))
        {
            AddTeam();
        }
        GUILayout.EndHorizontal();

        tm.Teams.ForEach(e =>
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("Team " + e.Id);
            e.Color = EditorGUILayout.ColorField(e.Color);
            if (GUILayout.Button("X"))
            {
                RemoveTeam(e);
                return;
            }

            GUILayout.EndHorizontal();
        });
        GUILayout.EndVertical();

        EditorUtility.SetDirty(target);
        EditorApplication.MarkSceneDirty();
        serializedObject.ApplyModifiedProperties();
    }

    private void AddTeam() => tm.AddTeam();
    private void RemoveTeam(Team team) => tm.RemoveTeam(team);
}
