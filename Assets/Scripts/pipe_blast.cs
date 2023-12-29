using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pipe_blast : MonoBehaviour
{
    [SerializeField] GameObject particle_package;
    [SerializeField] float initial_Wait_time;
    [SerializeField] float wait_time;
    [SerializeField] private Vector3 particle_pos;
    private BoxCollider2D col;
    void Start()
    {
        col = transform.GetChild(0).GetComponent<BoxCollider2D>();
        col.enabled = false;
        StartCoroutine(pre_blast());
    }
    private IEnumerator blast()
    {
        yield return new WaitForSeconds(wait_time);
        col.enabled = true;
        GameObject particles = Instantiate(particle_package, particle_pos, Quaternion.identity,GameObject.FindGameObjectWithTag("prefab").transform);
        particles.transform.eulerAngles = new Vector3(0, 0, 28.271f);
        yield return new WaitForSeconds(0.5f);
        col.enabled = false;
        StartCoroutine(blast());
    }
    private IEnumerator pre_blast()
    {
        yield return new WaitForSeconds(initial_Wait_time);
        StartCoroutine(blast());
    }

}
