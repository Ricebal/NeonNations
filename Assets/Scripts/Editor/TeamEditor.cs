using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(TeamManager))]
public class TeamEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.BeginVertical("Box");
        GUILayout.BeginHorizontal();
        GUILayout.Label("Team count: " + TeamManager.Singleton.Teams.Count);
        if (GUILayout.Button("Add team"))
        {
            AddTeam();
        }
        GUILayout.EndHorizontal();

        TeamManager.Singleton.Teams.ForEach(e =>
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
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        serializedObject.ApplyModifiedProperties();
    }

    private void AddTeam() => TeamManager.AddTeam();
    private void RemoveTeam(Team team) => TeamManager.RemoveTeam(team);
}
