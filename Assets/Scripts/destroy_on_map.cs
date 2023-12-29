using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_on_map : MonoBehaviour
{
    [SerializeField] private int map_number;
    private game_controller game;
    private void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        if(game.map != map_number)
        {
            Destroy(gameObject);
        }
    }
}
