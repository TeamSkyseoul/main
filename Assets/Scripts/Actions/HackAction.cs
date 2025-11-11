using UnityEngine;
using Battle;
using Character;

[CreateAssetMenu(menuName = "Interaction/HackAction")]
public class HackAction : InteractAction
{
    [Header("Hack Settings")]
    [SerializeField] GameObject hackVfx;
    [SerializeField] AudioClip hackSfx;
    [SerializeField] float delay = 0.2f;

    public override void Execute(IActor actor, IActor target)
    {
        if (target is not IHackable hackable) return;

        hackable.Wake(actor);

        Vector3 position = Vector3.zero;

        if (target is Transform transform) position = transform.position;
 
        if (hackVfx) Object.Instantiate(hackVfx, position, Quaternion.identity);
        if (hackSfx) AudioSource.PlayClipAtPoint(hackSfx, position);

    }

       

}
