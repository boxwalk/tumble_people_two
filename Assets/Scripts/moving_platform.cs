using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_platform : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private int start_point_index;
    [SerializeField] private Transform[] points;
    private int i;
    private GameObject prefab_parent;

    void Start()
    {
        transform.position = points[start_point_index].position;
        prefab_parent = GameObject.FindGameObjectWithTag("prefab");
    }

    void Update()
    {
        //check for new point
        if(Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if(i == points.Length)
            {
                i = 0;
            }
        }
        //move the platform
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.layer == 7)
        {
            collision.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(null);
        } else if(collision.gameObject.layer == 7)
        {
            collision.transform.SetParent(prefab_parent.transform);
        }
    }
}
