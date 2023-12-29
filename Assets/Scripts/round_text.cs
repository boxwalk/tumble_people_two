using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class round_text : MonoBehaviour
{
    //references
    private game_controller game_controller;
    private Image image;

    //sprites
    [SerializeField] private Sprite[] round_sprites;

    void Start()
    {
        game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        image.sprite = round_sprites[game_controller.round - 1];
    }
}
