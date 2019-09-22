using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxInfo : MonoBehaviour
{
    public GameObject Layer;

    public float SpeedCoefficient = 0.5f;
}

public class ParallaxHandler : MonoBehaviour
{
    private Camera _camera;
    private Vector3 _cameraLastPosition;

    [SerializeField]
    private float[] speedCoefficients;

    [SerializeField]
    private GameObject[] _parallaxLayers;


    private float speedCoefficientBase = 0.5f;

    void Start()
    {
        _camera = Camera.main;
        _cameraLastPosition = _camera.transform.position;

       
    }

    void Update()
    {
        for(int i = 0; i < _parallaxLayers.Length; i++) 
        {
            GameObject parallaxLayer = _parallaxLayers[i];

            float speedCoefficient =  0.5f;

            if(i < speedCoefficients.Length) {
                speedCoefficient = speedCoefficients[i];
            }
            
            Vector3 moveVector = ((_cameraLastPosition - _camera.transform.position)*speedCoefficient);
            Vector3 newPosition = new Vector3(
                parallaxLayer.transform.position.x - moveVector.x,
                parallaxLayer.transform.position.y, // - moveVector.y,
                parallaxLayer.transform.position.z
            );

            parallaxLayer.transform.position = newPosition;
        }
        foreach(var gameObject in _parallaxLayers) 
        {
            
        }

        _cameraLastPosition = _camera.transform.position;
        
    }
}
