using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class map_controller : MonoBehaviour
{
    [Serializable]
    public class map_data
    {
        public string map_name;
        public Sprite map_screen_sprite;
        public Sprite end_screen_sprite;
        public int end_screen_font_size;
        public Vector2 end_screen_text_offset;
    }

    //initialise variables
    public map_data[] map_library;


    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    public List<int> select_maps()
    {
        List<int> maps = new();
        List<int> maps_left = new(){ 1, 2, 3,4,5 };
        int map_choice = 0;
        for(int i =0; i <5; i++)
        {
            map_choice = Random.Range(0, maps_left.Count);
            maps.Add(maps_left[map_choice]);
            maps_left.RemoveAt(map_choice);
        }
        return maps;
    }
}
