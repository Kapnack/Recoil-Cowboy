using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerManager : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    public Action CloseCallback;
    
    [Header("Pause Button")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private Button exitPlayerButton;
    
    private const string ButtonTextResume = "Resume";
    private const string ButtonTextPause = "Pause"; 
    private void Awake()
    {
        _videoPlayer = GetComponentInChildren<VideoPlayer>();
        _videoPlayer.isLooping = true;
        
        pauseButton.onClick.AddListener(OnPauseVideoPlayer);
        exitPlayerButton.onClick.AddListener(OnExitVideoPlayer);
    }

    private void OnPauseVideoPlayer()
    {
        if (_videoPlayer.isPlaying)
        {
            _videoPlayer.Pause();
            pauseButton.image.color = Color.grey;
            pauseText.text = ButtonTextResume;
        }
        else
        {
            _videoPlayer.Play();
            pauseButton.image.color = Color.white;
            pauseText.text = ButtonTextPause;
        }
    }

    private void OnExitVideoPlayer()
    {
        CloseCallback?.Invoke();
        Destroy(gameObject);
    }
}