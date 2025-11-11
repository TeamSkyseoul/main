using Battle;
using Character;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour,IInteractor
{
    [Header("Interaction Settings")]
    [SerializeField] private float rayDistance = 5f;
    [SerializeField] private LayerMask interactMask = ~0;


    Camera interactCamera;
    IInteractable currentTarget;
    RaycastHit hitInfo;

    public Vector3 Position => transform.position;
    public Vector3 Forward => transform.forward;

    void Awake()
    {
        if (interactCamera == null) interactCamera = Camera.main;
       
    }

    void Update()=> HandleInteractionRay();
   

    void HandleInteractionRay()
    {
        IInteractable hitTarget = null;

        if (Physics.Raycast(interactCamera.transform.position, interactCamera.transform.forward, out hitInfo, rayDistance, interactMask))
            hitTarget = hitInfo.collider.GetComponentInParent<IInteractable>();

        if (currentTarget != hitTarget)
        {
            if (currentTarget != null) Cancel();
            currentTarget = hitTarget;
        }
    }
    public void BeginInteract(IActor actor) 
    {
        if(currentTarget!=null)
        if (currentTarget.CanBegin(actor)) currentTarget.Begin(actor);

    }
    public void Tick(IActor actor)=> currentTarget?.Tick(actor, Time.deltaTime);
    public void Cancel() => currentTarget?.Cancel();



#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (interactCamera == null)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(interactCamera.transform.position,
                        interactCamera.transform.position + interactCamera.transform.forward * rayDistance);
    }
#endif
}
