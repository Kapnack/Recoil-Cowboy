using System.Collections.Generic;
using Systems.Pool;
using UnityEngine;

namespace Chunks
{
    public class ChunkManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> chunkPrefabs;

        private Pool<Chunk> _pool;

        private readonly PoolData<Chunk>[] _activeChunks = new PoolData<Chunk>[3];

        private void Start()
        {
            _pool = new Pool<Chunk>(chunkPrefabs, transform);

            _pool.InitializeRandom(4, 1);

            Init();
        }

        private void Init()
        {
            PoolData<Chunk> go = new(Instantiate(chunkPrefabs[0], transform));

            go.Obj.transform.position = Vector3.zero;

            go.Component.SetUp();

            _activeChunks[0] = go;


            //------------------------------------------------------------------------

            go = _pool.Get();

            go.Component.LimitPass += DestroyBase;
            go.Component.LimitPass += SpawnNewChunk;

            _activeChunks[1] = go;

            AnchorObjects(_activeChunks[0], go);

            go.Component.SetUp();

            //--------------------------------------------------------------------------

            go = _pool.Get();

            _activeChunks[2] = go;

            AnchorObjects(_activeChunks[1], go);
            
            go.Component.SetUp();
        }

        private void SpawnNewChunk()
        {
            _pool.Return(_activeChunks[0]);

            PoolData<Chunk> newChunk = _pool.Get(1);

            AnchorObjects(_activeChunks[2], newChunk);

            ReOrganizeChunks(newChunk);

            newChunk.Component.SetUp();
        }

        private static void AnchorObjects(PoolData<Chunk> previous, PoolData<Chunk> next)
        {
            next.Obj.transform.position = new Vector3(0, previous.Component.ChunkLimitTop.position.y, 0);
        }

        private void ReOrganizeChunks(PoolData<Chunk> newChunk)
        {
            _activeChunks[0] = _activeChunks[1];
            _activeChunks[1] = _activeChunks[2];
            _activeChunks[2] = newChunk;

            _activeChunks[0].Component.LimitPass -= DestroyBase;
            _activeChunks[0].Component.LimitPass -= SpawnNewChunk;

            _activeChunks[1].Component.LimitPass += SpawnNewChunk;
        }

        private void DestroyBase()
        {
            Destroy(_activeChunks[0].Obj.gameObject);
            _activeChunks[0] = null;
        }
    }
}