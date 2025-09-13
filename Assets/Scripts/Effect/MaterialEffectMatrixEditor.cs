#if UNITY_EDITOR
using Effect;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MaterialEffectMatrix))]
public class MaterialEffectMatrixEditor : Editor
{
    private MaterialEffectMatrix matrix;
    
    private const float RowLabelWidth = 100f;
    private const float CellWidth = 80f;
    private const float CellSpacing = 3f; 

    private void OnEnable()
    {
        matrix = (MaterialEffectMatrix)target;
        EnsureMatrixSize();
    }

    public override void OnInspectorGUI()
    {
        if (!ValidateDatabase())
            return;

        DrawDatabaseReference();
        DrawMatrix();

        if (GUILayout.Button("Rebuild Matrix"))
            EnsureMatrixSize();
    }

    #region Validation & Setup
    private bool ValidateDatabase()
    {
        if (matrix.materialTypeDatabase == null)
        {
            EditorGUILayout.HelpBox("MaterialTypeDatabase가 설정되지 않았습니다.", MessageType.Warning);
            matrix.materialTypeDatabase = (MaterialTypeDatabase)EditorGUILayout.ObjectField(
                "Material Type Database",
                matrix.materialTypeDatabase,
                typeof(MaterialTypeDatabase),
                false
            );
            return false;
        }
        return true;
    }

    private void EnsureMatrixSize()
    {
        if (matrix.materialTypeDatabase == null) return;

        int size = matrix.materialTypeDatabase.rows.Count;
        int expected = size * (size + 1) / 2;

        while (matrix.rows.Count < expected)
            matrix.rows.Add(new MaterialEffectData());

        while (matrix.rows.Count > expected)
            matrix.rows.RemoveAt(matrix.rows.Count - 1);
    }
    #endregion

    #region Drawing
    private void DrawDatabaseReference()
    {
        EditorGUILayout.Space();
        matrix.materialTypeDatabase = (MaterialTypeDatabase)EditorGUILayout.ObjectField(
            "Material Type Database",
            matrix.materialTypeDatabase,
            typeof(MaterialTypeDatabase),
            false
        );
    }

    private void DrawMatrix()
    {
        var types = matrix.materialTypeDatabase.rows;
        int size = types.Count;

        if (size == 0)
        {
            EditorGUILayout.HelpBox("MaterialTypeDatabase에 정의된 타입이 없습니다.", MessageType.Info);
            return;
        }

        EditorGUILayout.LabelField("Material Effect Matrix", EditorStyles.boldLabel);

        DrawMatrixHeader(types);
        DrawMatrixRows(types, size);
    }

    private void DrawMatrixHeader(System.Collections.Generic.List<MaterialTypeData> types)

    {
        GUIStyle centeredLabel = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };

        GUILayout.BeginHorizontal();
        GUILayout.Space(RowLabelWidth);
        for(int i =0; i<types.Count; i++)
        {
            GUILayout.Label(types[i].materialType, centeredLabel, GUILayout.Width(CellWidth));
        }
     
        GUILayout.EndHorizontal();
    }

    private void DrawMatrixRows(System.Collections.Generic.List<MaterialTypeData> types, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(types[i].materialType, GUILayout.Width(RowLabelWidth));

            for (int j = 0; j < size; j++)
                DrawMatrixCell(i, j, size);

            GUILayout.EndHorizontal();
        }
    }

    private void DrawMatrixCell(int i, int j, int size)
    {
        if (j < i)
        {
            GUILayout.Space(CellWidth + CellSpacing);
            return;
        }

        int index = GetIndex(i, j, size);
        if (index < 0 || index >= matrix.rows.Count)
        {
            GUILayout.Space(CellWidth + CellSpacing);
            return;
        }

        var data = matrix.rows[index];
        if (GUILayout.Button(CheckWriteData(data) ? "✔" : "-", GUILayout.Width(CellWidth)))
        {
            MaterialEffectDataPopup.Open(data);
        }
    }
    bool CheckWriteData(MaterialEffectData data)
    { 
        if (string.IsNullOrEmpty(data.ParticleAddress) && string.IsNullOrEmpty(data.DecalAddress) && string.IsNullOrEmpty(data.SFX))
            return false;

        if (data.ParticleDuration < 0f || data.DecalDuration < 0f)
            return false;

        return true;
    }

    #endregion

    #region Utility
    private int GetIndex(int i, int j, int size)
    {
        if (i > j) (i, j) = (j, i);
        return i * size - (i * (i - 1)) / 2 + (j - i);
    }
    #endregion
}
#endif
