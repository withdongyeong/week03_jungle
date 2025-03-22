using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionAttackData", menuName = "Enemy/ExplosionAttackData")]
public class ExplosionAttackData : ScriptableObject
{
    public float minInterval = 5f;
    public float maxInterval = 8f;
    public float warningTime = 1.5f;
    public float effectTime = 2f;
    public float range = 3f;
    public float height = 1f;
    public Vector3 warningScale = new Vector3(1f, 1f, 1f);
}
