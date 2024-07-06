using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class game_controller : MonoBehaviour
{
    //states
    private string game_state = "pre_menu";
    private string menu_state = "start";

    //menu
    private GameObject main_menu_obj;
    private GameObject pre_menu_obj;
    [SerializeField] private float time_to_load_main_menu;
    [HideInInspector] public bool better_walljump = false;
    [HideInInspector] public bool beginner_mode = false;
    [HideInInspector] public bool low_def_mode = false;
    private bool settings_menu_open = false;
    [SerializeField] private List<Sprite> settings_sprites;

    //round selector
    private ragdoll_dropper ragdoll_Dropper;
    private GameObject map_Screen;
    [HideInInspector] public int round = 1;
    public int[] players_remaining_each_round;
    [SerializeField] private float[] delay_on_round_screen;
    public List<int> maps_list;

    //main_game
    public int map;
    private UI_controller UI_controller;
    [SerializeField] private Sprite victory_sprite;
    [SerializeField] private Sprite eliminated_sprite;
    [SerializeField] private GameObject enemy;
    [HideInInspector] public int players_qualified;
    [HideInInspector] public bool player_qualified;
    [HideInInspector] public bool is_eliminated = false;
    private List<int> skill_levels = new List<int>();
    [SerializeField] private List<int> skill_level_bases;
    [HideInInspector] public int player_skin;
    private skin_controller skin_controller;

    //ending screen
    private Button reset_button;
    /*[HideInInspector]*/ public List<int> positions;
    [HideInInspector] public List<int> maps_played = new List<int>();
    private map_controller map_controller;
    [HideInInspector] public bool verified = true;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    private void Start()
    {
        StartCoroutine(open_main_menu());
        main_menu_obj = GameObject.FindGameObjectWithTag("main_menu");
        pre_menu_obj = GameObject.FindGameObjectWithTag("pre_menu");
        main_menu_obj.SetActive(false);
        pre_menu_obj.SetActive(true);
        skin_controller = GameObject.FindGameObjectWithTag("skin").GetComponent<skin_controller>();
        get_player_skin();
        map_controller = GameObject.FindGameObjectWithTag("map").GetComponent<map_controller>();
        maps_list = map_controller.select_maps();
    }
    private void Update()
    {
        if (game_state == "main_game")
        {
            main_game();
        }
    }
    private void main_game()
    {
        if(players_qualified >= players_remaining_each_round[round] && !player_qualified && !is_eliminated)
        {
            //eliminated
            is_eliminated = true;
            eliminated();
        }
    }
    public void start_game()
    {
        if(menu_state == "start")
        {
            menu_state = "loading";
            SceneManager.LoadScene("round_display");
            game_state = "round_selector";
            StartCoroutine(load_Screen_logic());
        }
    }
    public void load_map_screen()
    {
        map_select();
        ragdoll_Dropper.transform.parent.gameObject.SetActive(false);
        map_Screen = GameObject.FindGameObjectWithTag("map_display");
        map_Screen.GetComponent<SpriteRenderer>().sprite = map_controller.map_library[map].map_screen_sprite;
        map_Screen.GetComponent<SpriteRenderer>().enabled = true;
        StartCoroutine(finish_map_screen());
    }
    public void load_main_game()
    {
        players_qualified = 0;
        player_qualified = false;
        SceneManager.LoadScene("main_game");
        game_state = "main_game";
        StartCoroutine(spawn_enemies());
    }
    private IEnumerator open_main_menu()
    {
        yield return new WaitForSeconds(time_to_load_main_menu);
        game_state = "main_menu";
        main_menu_obj.SetActive(true);
        pre_menu_obj.SetActive(false);
    }
    private IEnumerator load_Screen_logic()
    {
        load_skill_levels();
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(delay_on_round_screen[round-1]);
        ragdoll_Dropper = GameObject.FindGameObjectWithTag("dropper").GetComponent<ragdoll_dropper>();
        ragdoll_Dropper.drop_ragdolls(players_remaining_each_round[round-1]);
    }
    private IEnumerator finish_map_screen()
    {
        if(round == 1)
        {
            yield return new WaitForSeconds(0.5f);
        } else
        {
            yield return new WaitForSeconds(3);
        }
        load_main_game();
    }
    private void map_select()
    {
        map = maps_list[round - 1];
        maps_played.Add(map);
    }
    public void finish_race()
    {
        if(round == 5)
        {
            //win screen
            UI_controller.qualified_message.sprite = victory_sprite;
            StartCoroutine(open_win_screen());
        } else
        {
            StartCoroutine(race_wrapup());
        }
        UI_controller.qualified();
    }
    private IEnumerator race_wrapup()
    {
        yield return new WaitForSeconds(4);
        //increase the round number
        round++;
        //load the round scene again
        SceneManager.LoadScene("round_display");
        game_state = "round_selector";
        StartCoroutine(load_Screen_logic());
    }
    public void restart_game()
    {
        SceneManager.LoadScene("Main menu");
        Destroy(map_controller.gameObject);
        Destroy(skin_controller.gameObject);
        Destroy(gameObject);
    }
    private IEnumerator open_win_screen()
    {
        yield return new WaitForSeconds(4);
        AsyncOperation async_load = SceneManager.LoadSceneAsync("ending_screen");
        while (!async_load.isDone)
        {
            yield return null;
        }
        game_state = "ending_screen";
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        GameObject.FindGameObjectWithTag("lose screen").SetActive(false);
        reset_button = GameObject.FindGameObjectWithTag("reset_button").GetComponent<Button>();
        reset_button.onClick.AddListener(restart_game);
    }
    private IEnumerator spawn_enemies()
    {
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        for (int i = 1; i < players_remaining_each_round[round-1] ; i++)
        {
            GameObject instantiated_enemy = Instantiate(enemy,get_enemy_location(), Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
            enemy_logic enemy_script = instantiated_enemy.GetComponent<enemy_logic>();
            enemy_script.enemy_id = i ;
            enemy_script.skill_level = skill_levels[i - 1];
            int enemy_temp_skin = -1;
            while(enemy_temp_skin == -1)
            {
                enemy_temp_skin = skin_controller.random_skin();
                if(enemy_temp_skin == player_skin)
                {
                    enemy_temp_skin = -1;
                }
            }
            skin_controller.set_skin(enemy_temp_skin, instantiated_enemy);
        }
        UI_controller = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_controller>();

        //begginner mode
        if ((better_walljump || beginner_mode) && map == 4)
        {
            GameObject.FindGameObjectWithTag("autumn_saw_one").transform.parent.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            GameObject.FindGameObjectWithTag("autumn_saw_one").transform.parent.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        }
        if (beginner_mode && map == 2)
        {
            GameObject.FindGameObjectWithTag("beginner_platform_one").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.FindGameObjectWithTag("beginner_platform_one").GetComponent<PolygonCollider2D>().enabled = true;
            GameObject.FindGameObjectWithTag("beginner_platform_two").GetComponent<SpriteRenderer>().enabled = true;
            GameObject.FindGameObjectWithTag("beginner_platform_two").GetComponent<PolygonCollider2D>().enabled = true;
        }
        if(beginner_mode && map == 5)
        {
            Tilemap tilemap = GameObject.FindGameObjectWithTag("tilemap").GetComponent<Tilemap>();
            Tilemap foreground = GameObject.FindGameObjectWithTag("foreground").GetComponent<Tilemap>();
            tilemap.SetTile(new Vector3Int(-60, -750, 0), null);
            tilemap.SetTile(new Vector3Int(-78, -765, 0), null);

            foreground.SetTile(new Vector3Int(-60, -747, 0), null);
            foreground.SetTile(new Vector3Int(-60, -753, 0), null);
            foreground.SetTile(new Vector3Int(-57, -750, 0), null);
            foreground.SetTile(new Vector3Int(-63, -750, 0), null);

            foreground.SetTile(new Vector3Int(-78, -762, 0), null);
            foreground.SetTile(new Vector3Int(-78, -768, 0), null);
            foreground.SetTile(new Vector3Int(-81, -765, 0), null);
            foreground.SetTile(new Vector3Int(-75, -765, 0), null);

            tilemap.SetTile(new Vector3Int(-214, -814, 0), null);
            tilemap.SetTile(new Vector3Int(-226, -829, 0), null);
            tilemap.SetTile(new Vector3Int(-229, -829, 0), null);

            foreground.SetTile(new Vector3Int(-214, -811, 0), null);
            foreground.SetTile(new Vector3Int(-214, -817, 0), null);
            foreground.SetTile(new Vector3Int(-217, -814, 0), null);
            foreground.SetTile(new Vector3Int(-211, -814, 0), null);

            foreground.SetTile(new Vector3Int(-229, -826, 0), null);
            foreground.SetTile(new Vector3Int(-226, -826, 0), null);
            foreground.SetTile(new Vector3Int(-229, -832, 0), null);
            foreground.SetTile(new Vector3Int(-226, -832, 0), null);
            foreground.SetTile(new Vector3Int(-232, -829, 0), null);
            foreground.SetTile(new Vector3Int(-223, -829, 0), null);

            GameObject[] spikes_to_remove = GameObject.FindGameObjectsWithTag("putrid_beginner_spikes_remove");
            foreach (GameObject obj in spikes_to_remove)
            {
                Destroy(obj.transform.parent.gameObject);
            }

            GameObject pipeway_Saw = GameObject.FindGameObjectWithTag("pipeway_saw").transform.parent.gameObject;
            pipeway_Saw.GetComponent<SpriteRenderer>().enabled = false;
            pipeway_Saw.GetComponent<CircleCollider2D>().enabled = false;

            foreground.SetTile(new Vector3Int(-178, -987, 0), null);
            foreground.SetTile(new Vector3Int(-178, -990, 0), null);
            foreground.SetTile(new Vector3Int(-178, -993, 0), null);
            foreground.SetTile(new Vector3Int(-178, -1007, 0), null);
            foreground.SetTile(new Vector3Int(-178, -1010, 0), null);
            foreground.SetTile(new Vector3Int(-178, -1013, 0), null);

            foreground.SetTile(new Vector3Int(-89, -791, 0), null);
            foreground.SetTile(new Vector3Int(-86, -791, 0), null);
            foreground.SetTile(new Vector3Int(-83, -791, 0), null);
            foreground.SetTile(new Vector3Int(-80, -791, 0), null);
            foreground.SetTile(new Vector3Int(-77, -791, 0), null);
            foreground.SetTile(new Vector3Int(-74, -791, 0), null);
            foreground.SetTile(new Vector3Int(-71, -791, 0), null);
            foreground.SetTile(new Vector3Int(-68, -791, 0), null);
            foreground.SetTile(new Vector3Int(-65, -791, 0), null);
            foreground.SetTile(new Vector3Int(-62, -791, 0), null);
            foreground.SetTile(new Vector3Int(-59, -791, 0), null);
            foreground.SetTile(new Vector3Int(-56, -791, 0), null);

            foreground.SetTile(new Vector3Int(-15, -768, 0), null);
            foreground.SetTile(new Vector3Int(-12, -768, 0), null);
            foreground.SetTile(new Vector3Int(-9, -768, 0), null);
            foreground.SetTile(new Vector3Int(-6, -768, 0), null);
            foreground.SetTile(new Vector3Int(-3, -768, 0), null);

            foreground.SetTile(new Vector3Int(-82, -720, 0), null);
            foreground.SetTile(new Vector3Int(-82, -723, 0), null);
            foreground.SetTile(new Vector3Int(-82, -726, 0), null);
            foreground.SetTile(new Vector3Int(-82, -729, 0), null);
            foreground.SetTile(new Vector3Int(-82, -732, 0), null);
            foreground.SetTile(new Vector3Int(-82, -735, 0), null);
            foreground.SetTile(new Vector3Int(-82, -738, 0), null);
            foreground.SetTile(new Vector3Int(-82, -741, 0), null);
            foreground.SetTile(new Vector3Int(-82, -744, 0), null);
            foreground.SetTile(new Vector3Int(-82, -747, 0), null);

            GameObject[] spikes_to_add = GameObject.FindGameObjectsWithTag("putrid_beginner_spikes_add");
            foreach (GameObject obj in spikes_to_add)
            {
                obj.transform.parent.gameObject.GetComponent<BoxCollider2D>().enabled = true;
            }
        }
    }

    public void eliminated()
    {
        UI_controller.qualified_message.sprite = eliminated_sprite;
        UI_controller.qualified();
        StartCoroutine(open_lose_screen());
    }
    private IEnumerator open_lose_screen()
    {
        positions.Add(0);
        yield return new WaitForSeconds(3);
        AsyncOperation async_load =  SceneManager.LoadSceneAsync("ending_screen");
        while (!async_load.isDone)
        {
            yield return null;
        }
        game_state = "ending_screen";
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        GameObject.FindGameObjectWithTag("victory screen").SetActive(false);
        reset_button = GameObject.FindGameObjectWithTag("reset_button").GetComponent<Button>();
        reset_button.onClick.AddListener(restart_game);
    }
    private void load_skill_levels()
    {
        skill_levels.Clear();
        for (int i = skill_level_bases[round-1]; i < 31; i++)
        {
            if((round == 1 && i < 20) || (round == 2 && i < 10))
            {
                skill_levels.Add(0);
            }else
            {
                skill_levels.Add(i);
            }
        }
    }
    private void get_player_skin()
    {
        player_skin = skin_controller.random_skin();
    }

    private Vector3 get_enemy_location()
    {
        if(map == 1)
        {
            return new Vector3(Random.Range(0f, -1.5f), Random.Range(0f, 1.5f), 0);
        } else if(map == 2)
        {
            return new Vector3(Random.Range(11f, 13f), Random.Range(256f, 258f), 0);
        }else if (map == 3)
        {
            return new Vector3(Random.Range(629f, 635f), Random.Range(139f, 136f), 0);
        }else if (map == 4)
        {
            return new Vector3(Random.Range(-285.5f, -288.4f), Random.Range(7.24f, 8.08f), 0);
        }
        else if (map == 5)
        {
            return new Vector3(Random.Range(25.0f, 30.0f), Random.Range(-450.08f, -453.8f), 0);
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    //settings menu
    public void settings_button_click()
    {
        settings_menu_open = !settings_menu_open;
        if (settings_menu_open)
        {
            main_menu_obj.transform.GetChild(2).GetComponent<Animator>().SetTrigger("open_settings");
        }else
        {
            main_menu_obj.transform.GetChild(2).GetComponent<Animator>().SetTrigger("close_settings");
        }
    }
    public void beginner_mode_click()
    {
        beginner_mode = !beginner_mode;
        if (beginner_mode)
        {
            main_menu_obj.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = settings_sprites[0];
        }else
        {
            main_menu_obj.transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().sprite = settings_sprites[1];
        }
    }
    public void better_walljump_click()
    {
        better_walljump = !better_walljump;
        if (better_walljump)
        {
            main_menu_obj.transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<Image>().sprite = settings_sprites[2];
        }
        else
        {
            main_menu_obj.transform.GetChild(2).GetChild(0).GetChild(2).GetComponent<Image>().sprite = settings_sprites[3];
        }
    }
    public void low_def_click()
    {
        low_def_mode = !low_def_mode;
        if (low_def_mode)
        {
            main_menu_obj.transform.GetChild(2).GetChild(0).GetChild(4).GetComponent<Image>().sprite = settings_sprites[4];
        }
        else
        {
            main_menu_obj.transform.GetChild(2).GetChild(0).GetChild(4).GetComponent<Image>().sprite = settings_sprites[5];
        }
    }
}
