using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret_behaviour : MonoBehaviour
{
    //floats
    private float countdown_timer = 0f;
    [SerializeField] private float reload_time;

    //gameobjects
    [SerializeField] private GameObject bullet_prefab;
    [SerializeField] private GameObject shoot_point;
    private GameObject Player;

    //components
    private BoxCollider2D range_collider;
    private BoxCollider2D player_collider;
    private PolygonCollider2D player_collider_two;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        range_collider = transform.parent.GetComponent<BoxCollider2D>();
        player_collider = Player.GetComponent<BoxCollider2D>();
        player_collider_two = Player.GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //2D transform.look at version
        transform.right = Player.transform.position - transform.position;

        if (range_collider.IsTouching(player_collider) || range_collider.IsTouching(player_collider_two))
        {
            if(countdown_timer == 0)
            {
                countdown_timer = Time.time + reload_time;
            }else if(countdown_timer < Time.time)
            {
                //fire bullet
                Instantiate(bullet_prefab, shoot_point.transform.position, transform.rotation, GameObject.FindGameObjectWithTag("prefab").transform);
                //reset timer
                countdown_timer = 0;
            }
        }else
        {
            countdown_timer = 0;
        }
    }
}
