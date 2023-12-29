using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class round_display_features : MonoBehaviour
{
    //references
    private game_controller game_controller;
    private SpriteRenderer rend;

    //sprites
    [SerializeField] private Sprite[] round_sprites;

    void Start()
    {
        game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        rend.sprite = round_sprites[game_controller.round-1];
    }
}
