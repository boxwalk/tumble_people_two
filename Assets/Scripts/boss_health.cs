using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss_health : MonoBehaviour
{
    [SerializeField] private int starting_health;
    private int health;
    void Start()
    {
        health = starting_health;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }
}
