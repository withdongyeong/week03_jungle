using UnityEngine;

public class Mineral : MonoBehaviour
{
    public enum MineralType { Mineral1, Mineral2, Mineral3 }
    public MineralType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var manager = FindAnyObjectByType<MineralManager>();
            manager?.NotifyMineralCollected(gameObject);

            int score = type switch
            {
                MineralType.Mineral1 => GlobalSettings.Instance.mineral1Score,
                MineralType.Mineral2 => GlobalSettings.Instance.mineral2Score,
                MineralType.Mineral3 => GlobalSettings.Instance.mineral3Score,
                _ => 0
            };

            GameInfoManager.Instance.UpdateMineral(score);
        }
        else if (other.CompareTag("Projectile"))
        {
            var manager = FindAnyObjectByType<MineralManager>();
            manager?.NotifyMineralCollected(gameObject);
        }
    }
}
