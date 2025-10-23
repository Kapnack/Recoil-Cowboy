using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> chunkPrefabs;
    private readonly GameObject[] _chunksLoaded = new GameObject[3];
    private GameObject _chunkFolder;
    
    private void Awake()
    {
        _chunkFolder = new GameObject("Chunks")
        {
            transform =
            {
                parent = transform
            }
        };
        
        GameObject go = Instantiate(chunkPrefabs[0], _chunkFolder.transform, true);
        
        _chunksLoaded[0] = go;

        SpawnNewChunk();
        
        _chunksLoaded[0].GetComponent<Chunk>().LimitPass += SpawnNewChunk;
    }

    private void SpawnNewChunk()
    {
        GameObject newChunk = Instantiate(chunkPrefabs[Random.Range(1, chunkPrefabs.Count)], _chunkFolder.transform, true);
        
        Chunk chunk = newChunk.GetComponent<Chunk>();

        chunk.LimitPass += SpawnNewChunk;
        
        AnchorObjects(_chunksLoaded[3], newChunk);
        
        ReOrganizeChunks(newChunk);
    }
    
    private static void AnchorObjects(GameObject previous, GameObject next)
    {
        Chunk chunkOfGo = previous.GetComponent<Chunk>();
        
        next.transform.position = new Vector3(0, chunkOfGo.ChunkLimitTop.position.y, 0);
    }

    private void ReOrganizeChunks(GameObject newChunk)
    {
        if (_chunksLoaded[0] != null)
            Destroy(_chunksLoaded[0]);
        
        _chunksLoaded[0] = _chunksLoaded[1];
        _chunksLoaded[1] = _chunksLoaded[2];
        _chunksLoaded[2] = newChunk;
    }
}
