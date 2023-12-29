using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class door_behaviour : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private Vector3 destination;
    private GameObject player;
    private bool touching_player = false;
    private Rigidbody2D rb;
    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        rb = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(touching_player && Input.GetKeyDown(KeyCode.E))
        {
            //transport player
            rb.velocity = new Vector2(0, 0);
            player.transform.position = destination;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            touching_player = true;
            anim.SetBool("touching_player", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            touching_player = false;
            anim.SetBool("touching_player", false);
        }
    }
}
