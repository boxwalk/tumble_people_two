using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_controller : MonoBehaviour
{
    //checkpoint star variables
    [SerializeField] private GameObject star_image;
    [SerializeField] private GameObject star_flash;
    private Animator anim;

    //checkpoint counter variables
    private GameObject checkpoint_counter;
    [HideInInspector] public int checkpoint_number = 1;

    //qualified counter variables
    private GameObject qualified_counter;
    private game_controller game_controller;

    //qualified message variables
    [HideInInspector] public Image qualified_message;

    void Start()
    {
        anim = GetComponent<Animator>();
        checkpoint_counter = GameObject.FindGameObjectWithTag("checkpoint_counter");
        qualified_counter = GameObject.FindGameObjectWithTag("qualified_counter");
        game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        qualified_message = transform.GetChild(0).GetChild(1).GetChild(0).gameObject.GetComponent<Image>();
    }
    void Update()
    {
        //checkpoint counter
        checkpoint_counter.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = checkpoint_number + "/8";
        //qualified counter
        qualified_counter.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = game_controller.players_qualified + "/" + game_controller.players_remaining_each_round[game_controller.round];
        if(qualified_counter.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text.Length < 5)
        {
            qualified_counter.transform.GetChild(2).GetComponent<TextMeshProUGUI>().fontSize = 110;
            qualified_counter.transform.GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(-1126,1011,0);
        }
        else{
            qualified_counter.transform.GetChild(2).GetComponent<TextMeshProUGUI>().fontSize = 90;
            qualified_counter.transform.GetChild(2).GetComponent<RectTransform>().localPosition = new Vector3(-1126, 997, 0);
        }
    }
    public void checkpoint_star_call()
    {
        StartCoroutine(checkpoint_star());
    }
    private IEnumerator checkpoint_star()
    {
        star_image.SetActive(true);
        star_flash.SetActive(true);
        anim.SetTrigger("checkpoint_star");
        yield return new WaitForSeconds(1);
        star_image.SetActive(false);
        star_flash.SetActive(false);
    }
    public void qualified()
    {
        checkpoint_counter.SetActive(false);
        qualified_counter.SetActive(false);
        anim.SetTrigger("qualified");
    }

}
