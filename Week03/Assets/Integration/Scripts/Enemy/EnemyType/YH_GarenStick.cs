using UnityEngine;

public class YH_GarenStick : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameInfoManager.Instance.UpdateHP(GlobalSettings.Instance.spinDamage);
        }
    }
}
