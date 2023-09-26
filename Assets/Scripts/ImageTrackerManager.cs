using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTrackerManager : MonoBehaviour
{
    [SerializeField] private ARTrackedImageManager aRTrackedImageManager;
    private VideoPlayer videoPlayer;
    //El siguiente booleano nos ayuda a saber cuando la imagen puede o no ser rastreada
    private bool isImageTrackable;

    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    //Funcion que permite que el video se reproduzca solo cuando la imagen esta siendo captada
    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventData)
    {
        foreach(var trackedImage in eventData.added)
        {
            videoPlayer = trackedImage.GetComponentInChildren<VideoPlayer>();
            videoPlayer.Play();
        }

        foreach(var trackedImage in eventData.updated)
        {
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                if(!isImageTrackable)
                {
                    isImageTrackable = true;
                    videoPlayer.gameObject.SetActive(true);
                    videoPlayer.Play();
                }
            }
            else if(trackedImage.trackingState == TrackingState.Limited)
            {
                if(!isImageTrackable)
                {
                    isImageTrackable = false;
                    videoPlayer.gameObject.SetActive(true);
                    videoPlayer.Pause();
                }
            }
        }
    }

}
