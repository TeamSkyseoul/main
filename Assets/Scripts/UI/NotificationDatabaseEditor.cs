using GameUI;
using UnityEditor;
using UnityEngine;

public class NotificationDatabaseWindow : EditorWindow
{
    private NotificationDatabase db;
    private Vector2 scroll;

    private const int VisibleCount = 30; 

    [MenuItem("Tools/GameUI/Notification Database")]
    public static void Open()
    {
        GetWindow<NotificationDatabaseWindow>("Notification DB");
    }

    private void OnGUI()
    {
        db = (NotificationDatabase)EditorGUILayout.ObjectField(
            "Notification Database", db, typeof(NotificationDatabase), false);

        if (db == null)
        {
            EditorGUILayout.HelpBox("NotificationDatabase를 선택하세요.", MessageType.Info);
            return;
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Notifications", EditorStyles.boldLabel);

        // 헤더
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.Label("Index", GUILayout.Width(40));
        GUILayout.Label("Key", GUILayout.Width(150));
        GUILayout.Label("Format", GUILayout.ExpandWidth(true));
        GUILayout.Label("Clear", GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        scroll = EditorGUILayout.BeginScrollView(scroll);

        var notis = db.Notifications;

        for (int i = 0; i < VisibleCount; i++) 
        {
            NotificationData element = i < notis.Count ? notis[i] : null;

            EditorGUILayout.BeginHorizontal("box");

            GUILayout.Label(i.ToString(), GUILayout.Width(40));

            string newKey = EditorGUILayout.TextField(element != null ? element.key : "", GUILayout.Width(150));
            string newFormat = EditorGUILayout.TextField(element != null ? element.format : "", GUILayout.ExpandWidth(true));

          
            if (element != null && (newKey != element.key || newFormat != element.format))
            {
                Undo.RecordObject(db, "Edit Notification");
                element.key = newKey;
                element.format = newFormat;
                db.EditorReplaceAt(i, element);
                EditorUtility.SetDirty(db);
            }
            else if (element == null && (!string.IsNullOrEmpty(newKey) || !string.IsNullOrEmpty(newFormat)))
            {
                Undo.RecordObject(db, "Add Notification");
                var newData = new NotificationData { key = newKey, format = newFormat };
                db.EditorAdd(newData); 
                EditorUtility.SetDirty(db);
            }

            if (element != null && GUILayout.Button("X", GUILayout.Width(40)))
            {
                Undo.RecordObject(db, "Remove Notification");
                db.EditorRemoveAt(i);
                EditorUtility.SetDirty(db);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();

        if (GUILayout.Button("Rebuild Cache"))
        {
            db.EditorRebuildCache();
            EditorUtility.SetDirty(db);
        }
    }
}
