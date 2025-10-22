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
        matrix.EnsureMatrixSize();
        matrix.BuildCache();
    }

    public override void OnInspectorGUI()
    {
        if (!ValidateDatabase())
            return;

        DrawDatabaseReference();
        EditorGUILayout.Space();
        DrawMatrix();

        EditorGUILayout.Space();
        if (GUILayout.Button("Rebuild Matrix"))
        {
            matrix.EnsureMatrixSize();
            matrix.BuildCache();
            matrix.SaveCacheToRows();
            EditorUtility.SetDirty(matrix);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"현재 Matrix의 크기: {matrix.materialTypeDatabase.rows.Count}", EditorStyles.miniBoldLabel);

    }

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

    private void DrawDatabaseReference()
    {
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
        for (int i = 0; i < types.Count; i++)
            GUILayout.Label(types[i].materialType, centeredLabel, GUILayout.Width(CellWidth));
        GUILayout.EndHorizontal();
    }

    private void DrawMatrixRows(System.Collections.Generic.List<MaterialTypeData> types, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(types[i].materialType, GUILayout.Width(RowLabelWidth));

            for (int j = 0; j < size; j++)
                DrawMatrixCell(types, i, j, size);

            GUILayout.EndHorizontal();
        }
    }

    private void DrawMatrixCell(System.Collections.Generic.List<MaterialTypeData> types, int i, int j, int size)
    {
        if (j < i)
        {
            GUILayout.Space(CellWidth + CellSpacing);
            return;
        }

        var typeA = types[i].materialType;
        var typeB = types[j].materialType;
        var data = matrix.GetOrCreate(typeA, typeB);

        if (GUILayout.Button(CheckWriteData(data) ? "✔" : "-", GUILayout.Width(CellWidth)))
        {
            MaterialEffectDataPopup.Open(data);
        }
    }

    private bool CheckWriteData(MaterialEffectData data)
    {
        if (data == null) return false;
        if (string.IsNullOrEmpty(data.ParticleAddress) &&
            string.IsNullOrEmpty(data.DecalAddress) &&
            string.IsNullOrEmpty(data.SFX))
            return false;

        if (data.ParticleDuration < 0f || data.DecalDuration < 0f)
            return false;

        return true;
    }
}
#endif
