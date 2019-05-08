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
        GUILayout.Label("Team count: " + tm.Teams.Count);
        if (GUILayout.Button("Add team"))
        {
            AddTeam();
        }

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
    }

    private void AddTeam() => tm.AddTeam();
    private void RemoveTeam(Team team) => tm.RemoveTeam(team);
}
