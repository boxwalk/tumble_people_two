using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class verified_tracker : MonoBehaviour
{
    void Start()
    {
        game_controller gam = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        if (!(gam.verified && !gam.beginner_mode && !gam.better_walljump))
        {
            Destroy(transform.GetChild(0).gameObject);
            Destroy(gameObject);
        }
    }
}
