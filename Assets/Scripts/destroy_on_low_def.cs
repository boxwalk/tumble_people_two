using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroy_on_low_def : MonoBehaviour
{
    void Start()
    {
        if (GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>().low_def_mode)
        {
            Destroy(gameObject);
        }
    }

}
