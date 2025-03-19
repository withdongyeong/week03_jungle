using UnityEngine;

public class Camera : MonoBehaviour
{
    public GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + new Vector3(-5.5f, 2.6f, 0);
    }
}
