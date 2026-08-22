using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace IndependenceGame.MainMenu
{
    public class MenuVideoBackground : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RawImage videoDisplay;
        [SerializeField] private Image staticFallbackImage;

        [Header("Video Configuration")]
        [SerializeField] private string streamingAssetsFileName = "bg_main_menu_cinematic.mp4";
        [SerializeField] private float fadeInDuration = 0.5f;

        private void Awake()
        {
            if (videoPlayer == null) videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null) videoPlayer = gameObject.AddComponent<VideoPlayer>();

            if (videoDisplay != null)
            {
                videoDisplay.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            StartCoroutine(InitializeAndPlayVideo());
        }

        private IEnumerator InitializeAndPlayVideo()
        {
            string videoPath = Path.Combine(Application.streamingAssetsPath, streamingAssetsFileName);
            if (!File.Exists(videoPath))
            {
                videoPath = Path.Combine(Application.dataPath, "UI/MainMenu/Videos", streamingAssetsFileName);
            }

            if (File.Exists(videoPath))
            {
                videoPlayer.playOnAwake = false;
                videoPlayer.waitForFirstFrame = true;
                videoPlayer.isLooping = true;
                videoPlayer.source = VideoSource.Url;
                videoPlayer.url = Path.GetFullPath(videoPath);
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
                videoPlayer.audioOutputMode = VideoAudioOutputMode.None;

                videoPlayer.Prepare();

                float timeout = 5.0f;
                while (!videoPlayer.isPrepared && timeout > 0f)
                {
                    timeout -= Time.unscaledDeltaTime;
                    yield return null;
                }

                if (videoPlayer.isPrepared)
                {
                    videoPlayer.Play();
                    // Wait a couple frames for RenderTexture to receive the decoded frame
                    yield return new WaitForSecondsRealtime(0.15f);

                    if (videoDisplay != null)
                    {
                        videoDisplay.gameObject.SetActive(true);
                        float elapsed = 0f;
                        while (elapsed < fadeInDuration)
                        {
                            elapsed += Time.unscaledDeltaTime;
                            float a = Mathf.Clamp01(elapsed / fadeInDuration);
                            var c = videoDisplay.color;
                            c.a = a;
                            videoDisplay.color = c;
                            yield return null;
                        }
                    }
                }
            }
        }
    }
}
