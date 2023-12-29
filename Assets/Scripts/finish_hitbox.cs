using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class finish_hitbox : MonoBehaviour
{
    private game_controller game_controller;

    void Start()
    {
        game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && game_controller.players_qualified < game_controller.players_remaining_each_round[game_controller.round] && !game_controller.is_eliminated)
        {
            //race ended
            game_controller.player_qualified = true;
            game_controller.players_qualified++;
            game_controller.positions.Add(game_controller.players_qualified);
            game_controller.finish_race();
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            game_controller.player_qualified = true;
            game_controller.players_qualified++;
            game_controller.positions.Add(game_controller.players_qualified);
            game_controller.finish_race();
            Destroy(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            game_controller.is_eliminated = true;
            game_controller.eliminated();
            Destroy(gameObject);
        }
    }
}
