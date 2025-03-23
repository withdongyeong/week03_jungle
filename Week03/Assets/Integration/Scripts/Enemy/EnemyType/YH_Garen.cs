using System;
using UnityEngine;

public class YH_Garen : MonoBehaviour
{
    [SerializeField]
    private float speed = 100f;
    private void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
