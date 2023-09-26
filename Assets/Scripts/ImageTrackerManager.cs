using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

/*Script de tipo MonoBehavoiur que nos permite anclar la lógica desarrollada a continuación
a un objeto de juego*/
public class ImageTrackerManager : MonoBehaviour
{
    //Se hace referencia al componente ARTrackedImageManager usado para rastrear las imágenes
    [SerializeField] private ARTrackedImageManager aRTrackedImageManager;
    //Se referencia al componente VideoPlayer
    private VideoPlayer videoPlayer;
    //El siguiente booleano nos ayuda a saber cuando la imagen puede o no ser rastreada
    private bool isImageTrackable;

    /*OnEnable es un método propio de Unity, llamado cuando el componente del script se activa.
    En este caso se busca que el método se ejecute cuando se cambia la imagen rastreada*/
    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    /*OnDisable es un método propio de Unity, llamado cuando el componente del script se desactiva.
    Este método asegura en cierta forma que el evento sea removido cuando el script ya no se encuentr
    activo*/
    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    /*Funcion que permite que el video se reproduzca solo cuando la imagen esta siendo captada
    es llamado cada vez que la image rastreada cambia*/
    private void OnImageChanged(ARTrackedImagesChangedEventArgs eventData)
    {
        //Por cada imagen añadida al evento, se busca el coponente VideoPlayer
        foreach(var trackedImage in eventData.added)
        {
            videoPlayer = trackedImage.GetComponentInChildren<VideoPlayer>();
            videoPlayer.Play();
        }

        //Para cada una de las imágenes actualizadas se cambia el estado de rastreo
        foreach(var trackedImage in eventData.updated)
        {
            /*Si la imagen es rastreada de forma activa*/
            if(trackedImage.trackingState == TrackingState.Tracking)
            {
                if(!isImageTrackable)
                {
                    isImageTrackable = true;
                    videoPlayer.gameObject.SetActive(true);
                    videoPlayer.Play();
                }
            }
            /*Si la imagen es rastreada pero no se detecta con mucha legibilidad, por ello el estado
            Limited, */
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
