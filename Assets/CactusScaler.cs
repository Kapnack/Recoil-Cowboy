#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CactusScaler : MonoBehaviour
{
    [SerializeField] private GameObject cactusTop;
    [SerializeField] private GameObject cactusBody;
    [SerializeField] private GameObject cactusBase;

    private readonly List<GameObject> _cactusList = new();

    [SerializeField] private int length = 5;

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

        float basePos = cactusBase.transform.localPosition.y + cactusBase.transform.localScale.y;
        cactusTop.transform.localPosition = new Vector3(0, basePos + transform.localScale.y * length, 0);

        GameObject lastBase = cactusBase;

        for (int i = 0; i < length; i++)
        {
            GameObject go = Instantiate(cactusBody, cactusTop.transform.localPosition, Quaternion.identity);

            go.transform.SetParent(transform);

            basePos = cactusBase.transform.localPosition.y + cactusBase.transform.localScale.y;
            go.transform.localPosition = new Vector3(0, basePos + transform.localScale.y * i, 0);
            go.transform.localRotation = cactusBase.transform.localRotation;
            _cactusList.Add(go);
        }

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        basePos = cactusBase.transform.localPosition.y + cactusBase.transform.localScale.y / 2 * length + 2;
        boxCollider.center = new Vector3(0, basePos, 0);
        boxCollider.size = new Vector3(1, transform.localScale.y * length + 1, 1);
    }
}
#endif