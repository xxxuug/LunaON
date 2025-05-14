using UnityEngine;

public interface IDamageable
{
    string Tag { get; set; }
    bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default);
}
