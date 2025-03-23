using UnityEngine;

public class UpdatePlayerPositionToMaterial : MonoBehaviour
{
    public Transform player;
    public Material groundMaterial;

    void Update()
    {
        Vector3 pos = player.position;
        groundMaterial.SetVector("_PlayerPos", new Vector4(pos.x, pos.y, pos.z, 0));
    }

}
