using UnityEngine;

public class Mineral : MonoBehaviour
{
    public enum MineralType { Mineral1, Mineral2, Mineral3 }
    public MineralType type;
    public enum MineralHitType
    {
        CollectedByPlayer,
        DestroyedByProjectile
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            int score = type switch
            {
                MineralType.Mineral1 => GlobalSettings.Instance.mineral1Score,
                MineralType.Mineral2 => GlobalSettings.Instance.mineral2Score,
                MineralType.Mineral3 => GlobalSettings.Instance.mineral3Score,
                _ => 0
            };

            GameInfoManager.Instance.UpdateMineral(score);
            FindAnyObjectByType<MineralManager>()?.NotifyMineralCollected(gameObject, MineralHitType.CollectedByPlayer);
        }
        else if (other.CompareTag("Projectile"))
        {
            FindAnyObjectByType<MineralManager>()?.NotifyMineralCollected(gameObject, MineralHitType.DestroyedByProjectile);
        }
    }

}
