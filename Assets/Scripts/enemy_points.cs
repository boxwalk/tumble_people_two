using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_points : MonoBehaviour
{
    public bool contains_command;
    public string command;
    public bool contains_trigger;
    public string trigger;
    public bool contains_threshold;
    public int lower_threshold;
    public int upper_threshold;
    private int actual_percentage;
    public bool once_per_respawn;
    public bool has_delay_string;
    public string delay_string;
    public bool has_unlock;
    public bool is_locked;
    public int key_code;
    public Vector2 transport_destination;
    public bool contains_flying_command;
    public string flying_command;
    public float flying_command_time;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.1f);
        if(upper_threshold == 200)
        {
            actual_percentage = upper_threshold - lower_threshold - 100;
        } else if(lower_threshold == -100)
        {
            actual_percentage = upper_threshold - lower_threshold - 100;
        } else
        {
            actual_percentage = upper_threshold - lower_threshold;
        }
        if (contains_threshold)
        {
            gameObject.name = command + trigger + actual_percentage;
        }else
        {
            gameObject.name = command + trigger;
        }
        if (has_delay_string)
        {
            gameObject.name = "wait" + delay_string + command;
        }
        if (contains_flying_command)
        {
            gameObject.name = flying_command + flying_command_time;
        }
    }

}
