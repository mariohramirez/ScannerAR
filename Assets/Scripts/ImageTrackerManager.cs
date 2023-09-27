using System;
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
    [SerializeField] private GameObject[] aRModelsToPlace;

    //Definimos un diccionario para los modelos que se van a usar para reproducir videos
    private Dictionary<string, GameObject> aRModels = new Dictionary<string, GameObject>();
    private Dictionary<string, bool> modelState = new Dictionary<string, bool>();
    private Dictionary<string, VideoPlayer> videoPlayer = new Dictionary<string, VideoPlayer>();
    //Se referencia al componente VideoPlayer
    private VideoPlayer videoPlayerUnique;
    private Quaternion initialRotation = Quaternion.Euler(0f, 90f, 90f);
    //El siguiente booleano nos ayuda a saber cuando la imagen puede o no ser rastreada
    private bool isImageTrackable;

    void Start()
    {
        /*Se itera un loop a para recorrer la coleccion aRModelsToPlace, el cual se 
        define por medio del motro de Unity con los distintos prefabs que seran cargados*/
        foreach (var aRModel in aRModelsToPlace)
        {
            /*Se crea una instancia de un nuevo Game Object, se ingresa como parametro el objeto
            que aRModel que se esta leyendo de la coleccion, vector3.zero para que la posicion sea
            el origen y la rotacion definida como ultimo parametro, conservando la definida en Unity*/
            GameObject newARModel = Instantiate(aRModel, Vector3.zero, initialRotation);
            //Se da el nombre al modelo
            newARModel.name = aRModel.name;
            //Se anade el GameObject al diccionario
            aRModels.Add(newARModel.name, newARModel);
            //Se asigna un estado de inactividad al GameObject que anadimos al diccionario
            newARModel.SetActive(false);
            modelState.Add(newARModel.name, false);
            videoPlayer.Add(newARModel.name, newARModel.GetComponentInChildren<VideoPlayer>());
        }
    }

    /*OnEnable es un método propio de Unity, llamado cuando el componente del script se activa.
    En este caso se busca que el método se ejecute cuando se cambia la imagen rastreada*/
    private void OnEnable()
    {
        aRTrackedImageManager.trackedImagesChanged += ImageFound;
       // aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }


    /*OnDisable es un método propio de Unity, llamado cuando el componente del script se desactiva.
    Este método asegura en cierta forma que el evento sea removido cuando el script ya no se encuentr
    activo*/
    private void OnDisable()
    {
        aRTrackedImageManager.trackedImagesChanged -= ImageFound;
        //aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void ImageFound(ARTrackedImagesChangedEventArgs eventData)
    {
        foreach(var trackedImage in eventData.added)
        {
            ShowARModel(trackedImage);
        }
        foreach(var trackedImage in eventData.updated)
        {
            /*Si la imagen es rastreada de forma activa*/
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                ShowARModel(trackedImage);
            }
            /*Si la imagen es rastreada pero no se detecta con mucha legibilidad, por ello el estado
            Limited, */
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                HideARModel(trackedImage);
            }
        }
    }

    private void ShowARModel(ARTrackedImage trackedImage)
    {
        /*Usamos el bool para verificar si el modelo se encuentra activado o no, hacemos la busqueda
        en el diccionario de estados de lo modelo, teniendo como indice el nombre de la imagen
        AR rastreada*/
        bool isModelActivated = modelState[trackedImage.referenceImage.name];
        if(!isModelActivated)
        {
            //Le llevamos al modelo AR que vamos a usar, lo que traemos del diccionario de modelos
            GameObject aRModel = aRModels[trackedImage.referenceImage.name];
            //Le damos la posicion al objeto segun la imagen rastreada
            aRModel.transform.position = trackedImage.transform.position;
            //Se activa el modelo
            aRModel.SetActive(true);
            //Se cambia el estado activo del modelo a verdadero
            modelState[trackedImage.referenceImage.name] = true;
            //Le damos al reproductor el video guardado en el diccionario
            videoPlayerUnique = videoPlayer[trackedImage.referenceImage.name];
            //El video se reproduce
            videoPlayerUnique.Play();
        }
        else
        {
            GameObject aRModel = aRModels[trackedImage.referenceImage.name];
            aRModel.transform.position = trackedImage.transform.position;
        }
    }

    private void HideARModel(ARTrackedImage trackedImage)
    {
        bool isModelActivated = modelState[trackedImage.referenceImage.name];
        if(isModelActivated)
        {
            GameObject aRModel = aRModels[trackedImage.referenceImage.name];
            aRModel.SetActive(false);
            modelState[trackedImage.referenceImage.name] = false;
            //videoPlayer.gameObject.SetActive(false);
            //        videoPlayer.Pause();
        }
    }


    /*Funcion que permite que el video se reproduzca solo cuando la imagen esta siendo captada
    es llamado cada vez que la image rastreada cambia*/
 private void OnImageChanged(ARTrackedImagesChangedEventArgs eventData)
    {
        //Por cada imagen añadida al evento, se busca el coponente VideoPlayer
        foreach (var trackedImage in eventData.added)
        {
         //   videoPlayer = trackedImage.GetComponentInChildren<VideoPlayer>();
           // videoPlayer.Play();
        }

        //Para cada una de las imágenes actualizadas se cambia el estado de rastreo
        foreach (var trackedImage in eventData.updated)
        {
            /*Si la imagen es rastreada de forma activa*/
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                if (!isImageTrackable)
                {
                    isImageTrackable = true;
                  //  videoPlayer.gameObject.SetActive(true);
                   // videoPlayer.Play();
                }
            }
            /*Si la imagen es rastreada pero no se detecta con mucha legibilidad, por ello el estado
            Limited, */
            else if (trackedImage.trackingState == TrackingState.Limited)
            {
                if (isImageTrackable)
                {
                    isImageTrackable = false;
                  //  videoPlayer.gameObject.SetActive(false);
                   // videoPlayer.Pause();
                }
            }
        }
    }

   
}
