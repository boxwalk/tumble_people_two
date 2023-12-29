using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class position_displays : MonoBehaviour
{
    //sizes
    [SerializeField] private int one_digit_text_size;
    [SerializeField] private int two_digit_text_size;
    [SerializeField] private int eliminated_text_size;

    //positions
    [SerializeField] private Vector2 starting_position;
    [SerializeField] private Vector2 one_digit_position;
    [SerializeField] private Vector2 two_digit_position;
    [SerializeField] private Vector2 eliminated_position;

    //colors
    [SerializeField] private Color regular_color;
    [SerializeField] private Color eliminated_color;
    [SerializeField] private Color first_color;
    [SerializeField] private Color second_color;
    [SerializeField] private Color third_color;

    //components
    private TextMeshProUGUI text_component;
    private RectTransform _transform;

    void Start()
    {
        text_component = GetComponent<TextMeshProUGUI>();
        _transform = GetComponent<RectTransform>();
    }
    void Update()
    {
        if (text_component.text == "eliminated")
        {
            text_component.fontSize = eliminated_text_size;
            _transform.anchoredPosition = new Vector2(eliminated_position.x + starting_position.x, eliminated_position.y + starting_position.y);
            text_component.color = eliminated_color;
        }
        else if (text_component.text.Length == 1)
        {
            text_component.fontSize = one_digit_text_size;
            _transform.anchoredPosition = new Vector2(one_digit_position.x + starting_position.x, one_digit_position.y + starting_position.y);
            if(text_component.text == "1")
            {
                text_component.color = first_color;
            }else if (text_component.text == "2")
            {
                text_component.color = second_color;
            }else if (text_component.text == "3")
            {
                text_component.color = third_color;
            }else
            {
                text_component.color = regular_color;
            }
        }
        else
        {
            text_component.fontSize = two_digit_text_size;
            _transform.anchoredPosition = new Vector2(two_digit_position.x + starting_position.x, two_digit_position.y + starting_position.y);
            text_component.color = regular_color;
        }
    }
}
