using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinning_saw : MonoBehaviour
{
    [SerializeField] private float spinning_speed;
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, spinning_speed * Time.deltaTime));
    }
}
