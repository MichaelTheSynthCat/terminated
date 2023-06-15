using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WorldCanvasController should be attached to World Canvas, so it can be referenced from any script.
public class WorldCanvasController : MonoBehaviour
{
    public static WorldCanvasController canvas_controller;
    public static GameObject canvas;
    // Start is called before the first frame update
    private void Awake()
    {
        canvas_controller = this;
        canvas = gameObject;
    }
}
