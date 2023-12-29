using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ragdoll_dropper : MonoBehaviour
{
    //game objects 
    [SerializeField] private Transform min_x;
    [SerializeField] private Transform max_x;
    [SerializeField] private GameObject[] ragdoll_prefabs;

    //game controller
    private game_controller game_Controller;

    private void Start()
    {
        game_Controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
    }

    public void drop_ragdolls(int number_to_drop)
    {
        StartCoroutine(ragdoll_drop(number_to_drop));
    }

    private IEnumerator ragdoll_drop(int number_to_drop)
    {
        for (int i = 1; i <= number_to_drop; i++)
        {
            yield return new WaitForSeconds(0.025f);
            Instantiate(ragdoll_prefabs[Random.Range(0, 3)],new Vector2(Random.Range(min_x.position.x, max_x.position.x), transform.position.y) ,Quaternion.identity);
        }
        yield return new WaitForSeconds(2.5f);
        game_Controller.load_map_screen();
    }
}
