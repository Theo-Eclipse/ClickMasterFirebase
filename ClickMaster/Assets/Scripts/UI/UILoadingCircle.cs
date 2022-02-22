using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILoadingCircle : MonoBehaviour
{
    [SerializeField] private Transform RotatingCircle;
    [SerializeField] private float RotationSpeed = 200.0f;
    [SerializeField] private CanvasRenderer text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotatingCircle.Rotate(0, 0, Time.deltaTime * RotationSpeed);
        text.SetAlpha(Mathf.Abs(Mathf.Sin(Time.time * 4 % (Mathf.PI * 2))));
    }
}
