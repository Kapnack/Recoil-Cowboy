using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CactusScaler : MonoBehaviour
{
    [SerializeField] private GameObject cactusTop;
    [SerializeField] private GameObject cactusBody;
    [SerializeField] private GameObject cactusBase;
    [SerializeField] private int length = 1;

    private readonly List<GameObject> _cactusList = new();

    [ContextMenu("Change Cactus Length")]
    private void SetLength()
    {
        foreach (var cactus in _cactusList)
        {
            if (Application.isPlaying)
                Destroy(cactus);
            else
                DestroyImmediate(cactus);
        }

        var basePos = cactusBase.transform.position.y + cactusBase.transform.localScale.y;
        cactusTop.transform.position = new Vector3(0, basePos + transform.localScale.y * length, 0);

        var lastBase = cactusBase;

        for (int i = 0; i < length; i++)
        {
            var go = Instantiate(cactusBody, cactusTop.transform.position, Quaternion.identity);

            go.transform.SetParent(transform);

            basePos = cactusBase.transform.position.y + cactusBase.transform.localScale.y;
            go.transform.position = new Vector3(0, basePos + transform.localScale.y * i, 0);

            _cactusList.Add(go);
        }

        var boxCollider = GetComponent<BoxCollider>();
        basePos = cactusBase.transform.position.y + cactusBase.transform.localScale.y / 2 * length + 2;
        boxCollider.center = new Vector3(0, basePos, 0);
        boxCollider.size = new Vector3(1, transform.localScale.y * length + 1, 1);
    }
}