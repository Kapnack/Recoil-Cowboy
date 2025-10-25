using System.Collections.Generic;
using Systems.Pool;
using UnityEngine;

namespace Chunks
{
    public class ChunkManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> chunkPrefabs;

        private IPool _pool;

        private readonly GameObject[] _activeChunks = new GameObject[3];

        private void Start()
        {
            _pool = new Pool(chunkPrefabs, transform);

            _pool.InitializeRandom(4, 1);

            Init();
        }

        private void Init()
        {
            GameObject go = Instantiate(chunkPrefabs[0], transform);

            go.transform.position = Vector3.zero;

            Chunk chunk = go.GetComponent<Chunk>();
            chunk.SetUp();

            _activeChunks[0] = go;


            //------------------------------------------------------------------------

            go = _pool.GetRandom();

            chunk = go.GetComponent<Chunk>();
            
            chunk.LimitPass += DestroyBase;
            chunk.LimitPass += SpawnNewChunk;

            _activeChunks[1] = go;

            AnchorObjects(_activeChunks[0], go);

            chunk.SetUp();

            //--------------------------------------------------------------------------

            go = _pool.GetRandom();

            _activeChunks[2] = go;

            AnchorObjects(_activeChunks[1], go);

            chunk = go.GetComponent<Chunk>();
            chunk.SetUp();
        }

        private void SpawnNewChunk()
        {
            _pool.Return(_activeChunks[0]);

            GameObject newChunk = _pool.GetRandom(1);

            AnchorObjects(_activeChunks[2], newChunk);

            ReOrganizeChunks(newChunk);

            newChunk.GetComponent<Chunk>().SetUp();
        }

        private static void AnchorObjects(GameObject previous, GameObject next)
        {
            Chunk lastChunk = previous.GetComponent<Chunk>();

            next.transform.position = new Vector3(0, lastChunk.ChunkLimitTop.position.y, 0);
        }

        private void ReOrganizeChunks(GameObject newChunk)
        {
            _activeChunks[0] = _activeChunks[1];
            _activeChunks[1] = _activeChunks[2];
            _activeChunks[2] = newChunk;

            _activeChunks[0].GetComponent<Chunk>().LimitPass -= DestroyBase;
            _activeChunks[0].GetComponent<Chunk>().LimitPass -= SpawnNewChunk;

            _activeChunks[1].GetComponent<Chunk>().LimitPass += SpawnNewChunk;
        }

        private void ReturnObject(GameObject go)
        {
        }

        private void DestroyBase()
        {
            _activeChunks[0].SetActive(false);
            Destroy(_activeChunks[0]);
        }
    }
}