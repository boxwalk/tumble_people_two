using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class end_screen_controller : MonoBehaviour
{
    private Animator anim;
    private game_controller game_controller;
    private map_controller map_controller;

    //medals sprites
    [SerializeField] private Sprite no_medal;
    [SerializeField] private Sprite below_bronze_medal;
    [SerializeField] private Sprite bronze_medal;
    [SerializeField] private Sprite silver_medal;
    [SerializeField] private Sprite gold_medal;
    [SerializeField] private Sprite diamond_medal;
    [SerializeField] private Sprite ruby_medal;
    [SerializeField] private Sprite ultra_medal;

    //medlas logic
    private List<Sprite> medals;
    private List<List<int>> round_thresholds;
    [SerializeField] private List<int> round_one_thresholds;
    [SerializeField] private List<int> round_two_thresholds;
    [SerializeField] private List<int> round_three_thresholds;
    [SerializeField] private List<int> round_four_thresholds;
    [SerializeField] private List<int> big_medal_thresholds;
    private List<Sprite> big_medal_sprites;
    private Sprite big_medal_sprite;

    void Start()
    {
        anim = GetComponent<Animator>();
        map_controller = GameObject.FindGameObjectWithTag("map").GetComponent<map_controller>();
        game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        anim.SetInteger("rounds_played", game_controller.round);
        medals = new List<Sprite>() { no_medal, no_medal, no_medal, no_medal, no_medal };
        StartCoroutine(medals_selection());
        for (int i = 0; i < game_controller.round; i++)
        {
            transform.GetChild(2).GetChild((i * 4 )+ 1).GetComponent<Image>().sprite = map_controller.map_library[game_controller.maps_played[i]].end_screen_sprite;
            transform.GetChild(2).GetChild((i * 4 )+ 2).GetComponent<TextMeshProUGUI>().text = map_controller.map_library[game_controller.maps_played[i]].map_name;
            transform.GetChild(2).GetChild((i * 4) + 2).GetComponent<TextMeshProUGUI>().fontSize = map_controller.map_library[game_controller.maps_played[i]].end_screen_font_size;
            Transform temp_transform = transform.GetChild(2).GetChild((i * 4) + 2).GetComponent<Transform>();
            temp_transform.localPosition = new Vector2(temp_transform.localPosition.x + map_controller.map_library[game_controller.maps_played[i]].end_screen_text_offset.x, temp_transform.localPosition.y + map_controller.map_library[game_controller.maps_played[i]].end_screen_text_offset.y);
            if (game_controller.positions[i] == 0)
            {
                transform.GetChild(2).GetChild((i*4) +3).GetComponent<TextMeshProUGUI>().text = "eliminated";
            }else
            {
                transform.GetChild(2).GetChild((i * 4) + 3).GetComponent<TextMeshProUGUI>().text = game_controller.positions[i].ToString();
            }
        }
        for (int i = 5; i > game_controller.round; i--)
        {
            transform.GetChild(1).GetChild(i - 1).gameObject.SetActive(false);
            for(int j = 0; j < 4; j++)
            {
                transform.GetChild(2).GetChild((i-1) * 4 + j).gameObject.SetActive(false);
            }
        }

    }
    private IEnumerator medals_selection()
    {
        round_thresholds = new List<List<int>>{ round_one_thresholds, round_two_thresholds, round_three_thresholds, round_four_thresholds} ;
        yield return new WaitForSeconds(1);
        //select medals
        for (int i = 1; i <= game_controller.round; i++)
        {
            bool medal_selected = false;
            for (int j = 3; !medal_selected; j--)
            {
                if(game_controller.positions[i-1] == 0)
                {
                    //eliminated
                    medal_selected = true;
                } else
                {
                    if (i == 5)
                    {
                        //round 5
                        medal_selected = true;
                        if (game_controller.positions[i - 1] == 1)
                        {
                            medals[i - 1] = diamond_medal;
                        }
                    }
                    else
                    {
                        if (j == -1)
                        {
                            //diamond medal
                            medal_selected = true;
                            medals[i - 1] = diamond_medal;
                        }
                        else if (game_controller.positions[i - 1] <= round_thresholds[i - 1][j])
                        {
                            if (j == 2)
                            {
                                medals[i - 1] = bronze_medal;
                            }else if (j == 1)
                            {
                                medals[i - 1] = silver_medal;
                            }else if (j == 0)
                            {
                                medals[i - 1] = gold_medal;
                                if(!(game_controller.positions[i - 1] == 1))
                                {
                                    medal_selected = true;
                                }
                            }else if (j == 3)
                            {
                                medals[i - 1] = below_bronze_medal;
                            }
                        }
                        else
                        {
                            medal_selected = true;
                        }
                    }
                }
            }
        }
        for (int i = 0; i < 5; i++)
        {
            transform.GetChild(3).GetChild(i).GetComponent<Image>().sprite = medals[i];
        }
        if (!game_controller.is_eliminated)
        {
            //big medal
            big_medal_sprites = new List<Sprite>() { ultra_medal, ruby_medal, diamond_medal, gold_medal, silver_medal, bronze_medal };
            big_medal_sprite = no_medal;
            int total_position = game_controller.positions.Sum();
            for (int i = 0; big_medal_sprite == no_medal; i++)
            {
                if (total_position <= big_medal_thresholds[i])
                {
                    big_medal_sprite = big_medal_sprites[i];
                }
            }
            //set big medal
            transform.GetChild(3).GetChild(5).GetComponent<Image>().sprite = big_medal_sprite;
        } else
        {
            transform.GetChild(3).GetChild(5).GetComponent<Image>().sprite = no_medal;
        }
    }
}
