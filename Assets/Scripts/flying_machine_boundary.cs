using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flying_machine_boundary : MonoBehaviour
{
    private BoxCollider2D col;
    private player_movement player_movement;
    [SerializeField] private string target_mode;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        player_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<player_movement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(target_mode == "flying_machine")
        {
            if (player_movement.player_mode == target_mode)
            {
                col.enabled = true;
            }
            else
            {
                col.enabled = false;
            }
        }
        else
        {
            if (player_movement.submarine_Activated && player_movement.player_mode == target_mode)
            {
                col.enabled = true;
            }
            else
            {
                col.enabled = false;
            }
        }
    }
}
