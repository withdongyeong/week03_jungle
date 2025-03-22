using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType { Explosion, Laser }

    public ProjectileType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int damage = GetDamageByType(type);
            GameInfoManager.Instance.UpdateHP(damage);
        }
    }

    private int GetDamageByType(ProjectileType type)
    {
        return type switch
        {
            ProjectileType.Explosion => GlobalSettings.Instance.explosionDamage,
            ProjectileType.Laser => GlobalSettings.Instance.laserDamage,
            _ => 0
        };
    }
}
