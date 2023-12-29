using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_respawn : MonoBehaviour
{
    //Public variables 
    [HideInInspector] public Vector3 respawn_point = new Vector3 (0,0,0);

    //Components
    private Transform my_transform;
    private Rigidbody2D rb;
    private Animator anim;
    private player_movement player_movement;
    private BoxCollider2D col;

    //particles
    [SerializeField] private ParticleSystem death_particles;

    //skin variables
    private skin_controller skin_controller;
    private game_controller game_controller;
    private Animator UI_controller;

    //spawnign variables
    public bool is_testing = false;
    private Vector3 start_point;
    private Camera dark_cam;
    private Camera cam;

    private void Start()
    {
        dark_cam = GameObject.FindGameObjectWithTag("dark_cam").GetComponent<Camera>();
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        if (!is_testing)
        {
            game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        }

        //get start position
        if (!is_testing)
        {
            if(game_controller.map == 1)
            {
                search_start("skyline");
            } else if(game_controller.map == 2)
            {
                search_start("raceway");
            } else if (game_controller.map == 3)
            {
                search_start("shinobi");
            } else if (game_controller.map == 4)
            {
                search_start("ascent");
            }
            else if (game_controller.map == 5)
            {
                search_start("pipeway");
            }
        } else
        {
            start_point = transform.position;
        }
        transform.position = start_point;

        my_transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player_movement = GetComponent<player_movement>();
        col = GetComponent<BoxCollider2D>();
        UI_controller = GameObject.FindGameObjectWithTag("UI").GetComponent<Animator>();

        //skin setting
        skin_controller = GameObject.FindGameObjectWithTag("skin").GetComponent<skin_controller>();
        get_skin();
    }
    private void Update()
    {
        if(game_controller.better_walljump && player_movement.is_wall_jumping)
        {
            UI_controller.SetBool("is_walljumping", true);
        }
        else
        {
            UI_controller.SetBool("is_walljumping", false);
        }
        /*
        if (game_controller.better_walljump)
        {
            if (player_movement.is_grounded())
            {
                col.offset = new Vector2(col.offset.x, 0.1095167f);
                col.size = new Vector2(col.size.x, 1.318594f);
            }else
            {
                col.offset = new Vector2(col.offset.x, 0.221916f);
                col.size = new Vector2(col.size.x, 1.093795f);
            }
        }
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "kill")
        {
            death();
        }
        if (collision.gameObject.tag == "dark")
        {
            dark_cam.enabled = true;
            cam.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "dark")
        {
            cam.enabled = true;
            dark_cam.enabled = false;
        }
    }

    public void death()
    {
        //death here
        death_particle();
        my_transform.position = respawn_point;
        rb.velocity = new Vector2(0, 0);
        anim.SetTrigger("death");
        player_movement.enabled = false;
        StartCoroutine(reset_player_movement());
        anim.SetBool("is_walking", false);
        player_movement.cancel_boost();
        if(player_movement.player_mode == "flying_machine")
        {
            player_movement.player_mode = "normal";
            player_movement.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            player_movement.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            player_movement.gameObject.GetComponent<CircleCollider2D>().enabled = false;
            anim.SetLayerWeight(2, 0);
            anim.SetLayerWeight(3, 0);
            anim.SetBool("flying_machine", false);
            player_movement.cam_animator.SetTrigger("zoom_in");
        }
        if (player_movement.player_mode == "submarine")
        {
            player_movement.player_mode = "normal";
            player_movement.cam_animator.SetTrigger("zoom_in");
            anim.SetLayerWeight(2, 0);
            anim.SetLayerWeight(3, 0);
            anim.SetBool("submarine", false);
            player_movement.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            player_movement.gameObject.GetComponent<BoxCollider2D>().enabled = false;
            player_movement.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            player_movement.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    private IEnumerator reset_player_movement()
    {
        while (true)
        {
            yield return null;
            if (player_movement.is_grounded())
            {
                player_movement.enabled = true;
                StopCoroutine(reset_player_movement());
            }
        }
    }

    private void death_particle()
    {
        Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
    }

    private void get_skin()
    {
        skin_controller.set_skin(game_controller.player_skin, gameObject);
    }
    private void search_start(string tag)
    {
        start_point = GameObject.FindGameObjectWithTag(tag).GetComponent<Transform>().position;
    }
    public void remove_submarine()
    {
        player_movement.player_mode = "normal";
        player_movement.cam_animator.SetTrigger("zoom_in");
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 0);
        anim.SetBool("submarine", false);
        player_movement.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        player_movement.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        player_movement.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
        player_movement.gameObject.GetComponent<CircleCollider2D>().enabled = false;
    }
}
