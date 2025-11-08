using UnityEngine;

public class RequireObject : MonoBehaviour
{
    [SerializeField] Component origin;
    Component required;

    private void Awake()
    {
        RequireComponentOfType();
    }

    void RequireComponentOfType()
    {
        if (origin == null) return;
        var type = origin.GetType();

        required = FindAnyObjectByType(type) as Component;
        if (required != null) return;

        if (origin == null) required = new GameObject("").AddComponent(type);
        else required = Instantiate(origin.gameObject).GetComponent(type);

        DontDestroyOnLoad(required.gameObject);
    }

    void OnDestroy()
    {
        if (required != null) DestroyImmediate(required.gameObject);
    }
}
