using UnityEngine;
using Battle;
using Character;

[CreateAssetMenu(menuName = "Interaction/HackAction")]
public class HackAction : InteractAction
{
    [Header("Hack Settings")]
    [SerializeField] private GameObject hackVfx;
    [SerializeField] private AudioClip hackSfx;
    [SerializeField] private float delay = 0.2f;

    public override void Execute(Transform actor)
    {
        if (!actor.TryGetComponent<IWakeable>(out var wakeable)) return;

        wakeable.Wake();

        Debug.Log($"[HackAction] {actor.name} 해킹 성공!");

        if (hackVfx)
            Object.Instantiate(hackVfx, actor.position, Quaternion.identity);

        if (hackSfx)
            AudioSource.PlayClipAtPoint(hackSfx, actor.position);

      
    }
}
