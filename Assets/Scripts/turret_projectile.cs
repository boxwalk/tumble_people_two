using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret_projectile : MonoBehaviour
{
    //values for projectile 
    [SerializeField] private float speed;
    private Rigidbody2D rb;
    [SerializeField] private float auto_destruct_time;
    [SerializeField] private GameObject destroy_particles;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        StartCoroutine(auto_destroy());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            //kills player
            collision.gameObject.GetComponent<player_respawn>().death();
            //then destroys gameobject
            destroy_particle();
            Destroy(gameObject);
        } else if (collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            //just destroy gameobject
            destroy_particle();
            Destroy(gameObject);
        }
    }
    private IEnumerator auto_destroy()
    {
        yield return new WaitForSeconds(auto_destruct_time);
        Destroy(gameObject);
    }
    private void destroy_particle()
    {
        Instantiate(destroy_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
    }
}
