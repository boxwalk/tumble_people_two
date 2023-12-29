using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class medal_logic : MonoBehaviour
{
    [SerializeField] private Vector2 diamond_size;
    [SerializeField] private Vector2 ruby_size;
    [SerializeField] private Vector2 ultra_size;
    private Image img;
    private RectTransform size;

    void Start()
    {
        img = GetComponent<Image>();
        size = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (img.sprite.name == "medals_9")
        {
            //diamond
            size.sizeDelta = diamond_size;
        }else if (img.sprite.name == "medals_5")
        {
            //ruby
            size.sizeDelta = ruby_size;
        }else if (img.sprite.name == "medals_10")
        {
            //ultra
            size.sizeDelta = ultra_size;
        }else { 
            //normal
            size.sizeDelta = new Vector2(1507, 2805);
        }
    }
}
