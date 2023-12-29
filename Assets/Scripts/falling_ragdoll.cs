using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class falling_ragdoll : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "ragdoll_destroy")
        {
            Destroy(gameObject);
        }
    }
}
