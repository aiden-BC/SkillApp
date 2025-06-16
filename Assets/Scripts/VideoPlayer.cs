using UnityEngine;
using UnityEngine.Video;

public class VideoUIButtonController : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    public void PlayVideo()
    {
        if (!videoPlayer.isPlaying)
            videoPlayer.Play();
    }

    public void PauseVideo()
    {
        if (videoPlayer.isPlaying)
            videoPlayer.Pause();
    }
}
