using Character;
using Effect;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class RetrieverComponent : MonoBehaviour, IRetriever
{
    public float Duration => duration;
    public Vector3 Offset => offset;
    public Vector3 Rotation => rotation;
    
    [SerializeField] float duration;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 rotation;
    [SerializeField] bool syncWithEffect;
    public void Retrieve(Transform actor) => StartCoroutine(RetrieveRoutine(actor));

    private IEnumerator RetrieveRoutine(Transform actor)
    { 
        actor.TryGetComponent<IAppearance>(out var appearance);

        appearance?.InvokeDissolve();

        yield return new WaitForSeconds(GetWaitTime(appearance));

        actor.gameObject.SetActive(false);  

        RepositionActor(actor);

        actor.gameObject.SetActive(true);

        appearance?.InvokeAppear();
    }

    private float GetWaitTime(IAppearance appearance) => (appearance != null&&syncWithEffect) ? appearance.Duration : Duration;

    private void RepositionActor(Transform actor)
    {
        actor.position = transform.position + Offset;
        actor.rotation = transform.rotation * Quaternion.Euler(Rotation);
    }
}





#if UNITY_EDITOR


[CustomEditor(typeof(RetrieverComponent))]
public class RetrieverComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var syncProp = serializedObject.FindProperty("syncWithEffect");
        EditorGUILayout.PropertyField(syncProp);

        if (!syncProp.boolValue)
        {
            var durationProp = serializedObject.FindProperty("duration");
            EditorGUILayout.PropertyField(durationProp);
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("offset"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotation"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
