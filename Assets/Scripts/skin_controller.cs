using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class skin_controller : MonoBehaviour
{
    [Serializable]
    private class skin_data
    {
        public string skin_name;
        public List<Sprite> skin_parts;
    }

    //initialise variables
    [SerializeField] private skin_data[] skin_library;


    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private List<Sprite> get_sprite(int skin_index)
    {
        return skin_library[skin_index].skin_parts;
    }
    
    public void set_skin(int skin_index, GameObject obj)
    {
        //create lists of parts and gameobjects
        List<Sprite> skin_pieces = get_sprite(skin_index);
        List<GameObject> body_parts = new List<GameObject>();
        body_parts.Add(obj.transform.GetChild(0).GetChild(0).GetChild(0).gameObject); //head
        body_parts.Add(obj.transform.GetChild(0).GetChild(0).gameObject); //body
        body_parts.Add(obj.transform.GetChild(0).GetChild(0).GetChild(1).gameObject); //left arm
        body_parts.Add(obj.transform.GetChild(0).GetChild(1).gameObject); //left leg
        body_parts.Add(obj.transform.GetChild(0).GetChild(2).gameObject); //right leg
        body_parts.Add(obj.transform.GetChild(0).GetChild(0).GetChild(2).gameObject); //right arm

        //loop through and assign them
        for (int i = 0; i < 6; i++)
        {
            assign_sprite(body_parts[i], skin_pieces[i]);
        }
    }

    private void assign_sprite(GameObject obj, Sprite sprite)
    {
        obj.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public int random_skin()
    {
        return Random.Range(0, skin_library.Length);
    }
}
