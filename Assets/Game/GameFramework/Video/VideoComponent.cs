using System;
using GameFramework.Base;
using GameFramework.Utility.Extension;
using GameFramework.Utility.Singleton;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace GameFramework.Video
{
    public class VideoComponent : GameFrameworkComponent
    {
        public enum enVideoPlayMode
        {
            CG = 0,
            UI = 1
        }

        public override int Priority => 50;

        private VideoPlayer videoPlayer = null;
        private AudioSource audioSource = null;
        private RenderTexture renderTexture = null;
        private GameObject videoObject = null;
        private RawImage videoRawImage = null;
        private bool isPauseOnFirstFrame = false;
        private bool stopOnClick = false;
        private Action videoStatrPlayCallback = null;
        private Action videoEndCallback = null;

        public bool IsVideoPlaying => videoPlayer != null && videoPlayer.isPlaying;

        private void InitVideoPlayer(VideoPlayer player)
        {
            if(player == null) return;
            player.playOnAwake = false;
            player.waitForFirstFrame = false;
            player.audioOutputMode = VideoAudioOutputMode.AudioSource;
            player.renderMode = VideoRenderMode.RenderTexture;
            player.aspectRatio = VideoAspectRatio.NoScaling;
            player.source = VideoSource.Url;
        }

        private RenderTexture setupRenderTexture(RenderTexture rt = null, enVideoPlayMode mode = enVideoPlayMode.CG)
        {
            int width = mode == enVideoPlayMode.CG
                ? AppConst.VideoConfig.CGVideoWidth
                : AppConst.VideoConfig.UIVideoWidth;
            int height = mode == enVideoPlayMode.CG
                ? AppConst.VideoConfig.CGVideoHeight
                : AppConst.VideoConfig.UIVideoHeight;
            RenderTexture currentRenderTexture = rt;
            if (rt == null || rt.width != width || rt.height != height)
            {
                if (rt != null)
                {
                    RenderTexture.ReleaseTemporary(rt);
                }
                currentRenderTexture = RenderTexture.GetTemporary(width,height,0,RenderTextureFormat.RGB565);
                currentRenderTexture.name = "VideoPlayerRenterTexture" + currentRenderTexture.GetHashCode();
                currentRenderTexture.autoGenerateMips = false;
            }
            return currentRenderTexture;
        }

        public void InitVideoComponent()
        {
            GameObject prefab = Resources.Load<GameObject>("UI/Panel_Video");
            if (prefab)
            {
                videoObject = Instantiate(prefab);
                videoObject.transform.SetParent(AppConst.GlobalCahce.PanelRoot);
                RectTransform rectTransform = videoObject.GetComponent<RectTransform>();
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.anchoredPosition3D = Vector3.zero;
                videoObject.transform.localScale = Vector3.one;
                videoRawImage = videoObject.transform.Find("RawImage_Video").GetComponent<RawImage>();
            }
            videoObject.SetActive(false);
            videoPlayer = gameObject.GetOrAddComponent<VideoPlayer>();
            audioSource = gameObject.GetOrAddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.Pause();
            InitVideoPlayer(videoPlayer);
            videoPlayer.SetTargetAudioSource(0, audioSource);
            renderTexture = setupRenderTexture();
            videoPlayer.prepareCompleted += VideoPrepareCompleted;
            videoPlayer.loopPointReached += VideoLoopPointReached;
            videoPlayer.errorReceived += VideoErrorReceived;
            videoPlayer.started += VideoStarted;
            videoPlayer.Prepare();
        }

        private void VideoPrepareCompleted(VideoPlayer player)
        {
            player.Play();
            videoStatrPlayCallback?.Invoke();
            videoStatrPlayCallback = null;
            if (isPauseOnFirstFrame)
            {
                player.Pause();
            }
        }
        
        private void VideoLoopPointReached(VideoPlayer player)
        {
            if (!player.isLooping)
            {
                StopVideo();
            }
        }
        
        private void VideoErrorReceived(VideoPlayer player ,string errorMesage)
        {
            StopVideo();
        } 
        
        private void VideoStarted(VideoPlayer player)
        {
            
        }

        private void SetVideoAspect(RawImage rawImage)
        {
            AspectRatioFitter asp = rawImage.gameObject.GetOrAddComponent<AspectRatioFitter>();
            if (asp)
            {
                asp.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
                asp.aspectRatio = renderTexture.width / (float)renderTexture.height;
            }
        }

        public void PlayVideo(string videoUrl,RawImage image,Action onStart,Action onEnd,bool isLoop = false,bool isPause = false ,bool isStopOnClick = false,enVideoPlayMode mode = enVideoPlayMode.CG)
        {
            if(videoPlayer == null) return;
            renderTexture = setupRenderTexture(renderTexture, mode);
            if (videoPlayer.isPlaying)
            {
                StopVideo();
            }
            if (image == null)
            {
                if (!videoObject.activeSelf)
                {
                    videoObject.SetActive(true);
                }
                videoRawImage.texture = renderTexture;
                SetVideoAspect(videoRawImage);
            }
            else
            {
                if (videoObject != null && videoObject.activeSelf)
                {
                    videoObject.SetActive(false);
                }
                if (videoRawImage != null)
                {
                    videoRawImage.texture = null;
                }
                image.texture = renderTexture;
                SetVideoAspect(image);
            }
            videoStatrPlayCallback = OnStart;
            videoEndCallback = onEnd;
            stopOnClick = isStopOnClick;
            isPauseOnFirstFrame = isPause;
            videoPlayer.url = videoUrl;
            videoPlayer.isLooping = isLoop;
            audioSource.volume = 1f;
            videoPlayer.targetTexture = renderTexture;
        }


        public void StopVideo()
        {
            if (videoPlayer != null && videoPlayer.isPlaying)
            {
                videoPlayer.Stop();
                if (videoObject != null && videoObject.activeSelf)
                    videoObject.SetActive(false);
                if (videoRawImage != null)
                    videoRawImage.texture = null;

                videoEndCallback?.Invoke();
                videoEndCallback = null;
            }
        }
        
        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            if (IsVideoPlaying && stopOnClick)
            {
                if ( Singleton<GameEntry>.GetInstance().InputComponent.HasTouch())
                {
                    StopVideo();
                }
            }
        }
        
       
        public override void Shutdown()
        {
            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
                renderTexture = null;
            }
        }
    }
}