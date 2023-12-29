using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawn_point : MonoBehaviour
{
    private player_respawn player_respawn;
    private UI_controller UI_Controller;

    // Start is called before the first frame update
    void Start()
    {
        player_respawn = GameObject.FindGameObjectWithTag("Player").GetComponent<player_respawn>();
        UI_Controller = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_controller>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //fire respawn point
            player_respawn.respawn_point = transform.position;
            if(!(gameObject.name == "starting_respawn") && !(gameObject.name == "finish_respawn"))
            {
                UI_Controller.checkpoint_star_call();
                UI_Controller.checkpoint_number++;
            }
            Destroy(gameObject);
        }
    }
}
