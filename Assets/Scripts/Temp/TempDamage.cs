using Character;
using UnityEngine;

public class TempDamage : MonoBehaviour
{
    [SerializeField] Transform transform;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("K");
            if (transform.TryGetComponent<IDamageable>(out var damageable))
            {
                Debug.Log("Damage");
                if (damageable is IHP hp)
                    hp.HP.Value -= 10;
                damageable.TakeDamage();
            }
        }
        
    }
}
