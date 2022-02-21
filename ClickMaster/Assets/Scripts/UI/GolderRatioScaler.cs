using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas), typeof(CanvasScaler)), DisallowMultipleComponent]
public class GolderRatioScaler : MonoBehaviour
{
    [SerializeField] private CanvasScaler canvasScaler;
    // Start is called before the first frame update
    void Start() => canvasScaler.scaleFactor = (Screen.dpi / 100.0f) * 0.618f;// 1.618 - Golden ration.   
    private void Reset() => canvasScaler = GetComponent<CanvasScaler>();
}
