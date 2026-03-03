using TMPro;
using UnityEngine;

public class VersionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject versionCanvas;

    private TMP_Text _version;
    private string _versionFormat;

    private void Awake()
    {
        _version = versionCanvas.GetComponent<TMP_Text>();
        _versionFormat = _version.text;
        _version.text = string.Format(_versionFormat, 0);
    }

    private void Start()
    {
        _version.text = string.Format(_versionFormat, Application.version);
    }
}