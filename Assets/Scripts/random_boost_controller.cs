using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class random_boost_controller : MonoBehaviour
{
    public int random_boost;
    private void Start()
    {
        random_boost = Random.Range(0, 3);
        transform.GetChild(random_boost).gameObject.SetActive(true);
    }
}
