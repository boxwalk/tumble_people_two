using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrolling_background : MonoBehaviour
{
    private RawImage img;
    [SerializeField] private float x_Speed;
    [SerializeField] private float y_speed;
    void Start()
    {
        img = GetComponent<RawImage>();
    }

    void Update()
    {
        img.uvRect = new Rect(img.uvRect.position + new Vector2(x_Speed, y_speed) * Time.deltaTime, img.uvRect.size);
    }
}
