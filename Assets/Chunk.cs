using System;
using Systems.TagClassGenerator;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private Transform chunkLimitTop;

    public Transform ChunkLimitTop => chunkLimitTop;

    public event Action LimitPass;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(Tags.Player))
            LimitPass?.Invoke();
    }
}
