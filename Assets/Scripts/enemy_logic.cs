using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_logic : MonoBehaviour
{
    //movement and commands variables
    [SerializeField] private string current_command = "none";
    private player_movement player_movement;
    private float speed;
    private float jump_height;
    private bool jump_trigger = false;
    [SerializeField] private int chance_number; //serialized so i don't have to debug.log it
    private bool facing_right = true;
    private bool is_wall_sliding;
    private float wall_slide_speed;
    private bool is_wall_jumping;
    private float wall_jump_time;
    private Vector2 wall_jump_power;
    private string current_override;
    private List<Vector3> one_use_points = new List<Vector3>();
    private string former_command;
    [SerializeField] private float min_qualified_walk_time;
    [SerializeField] private float max_qualified_walk_time;
    private float inital_wait_time;
    private float ground_check_radius = 0.2f;
    private List<int> unlocked_points = new List<int>();
    private bool is_touching_wind;
    private bool is_touching_down_wind;
    private float time_in_wind;
    private float max_wind_speed;
    private bool touching_boost = false;
    private bool boost_active = false;
    private float boost_speed;
    private float max_boost_speed;
    private float boost_decay_time;
    private float boosted_wall_jump_time;
    private Vector2 boosted_wall_jump_power;
    private bool is_touching_wall_boost;
    private string player_mode = "normal";
    private bool fm_command_left = false;
    private bool fm_command_right = false;
    private bool fm_command_up = false;
    private bool fm_command_down = false;
    private bool is_touching_ice;
    private float ice_speed;
    private CircleCollider2D circle;
    private bool is_touching_water = false;
    private bool is_touching_subnarine_special_zone = false;
    private bool is_touching_submarine_special_zone_2 = false;
    private ParticleSystem water_splash;
    private float submarine_speed_decay_time;
    private float submarine_fall_max_speed;
    private float submarine_blast_recharge_time;
    private float vertical_blast_strength;
    private float horizontal_blast_strength;
    private bool submarine_blast_charged = false;
    private bool charge_time_set = false;
    private float recharged_time;
    private GameObject submarine_blast_particles_A;
    private GameObject submarine_blast_particles_B;
    private bool left_blast_trigger = false;
    private bool right_blast_trigger = false;
    private bool up_blast_trigger = false;
    private float laser_cycle = 0;
    private bool former_laser_cycle = false;
    private float former_saw_pos = 0;
    private bool saw_dir_right = true;
    private bool former_Wind;
    private float base_wind_speed;
    private float time_stuck = 0;
    [SerializeField] private float time_stuck_to_delete;
    private Vector3 former_position;
    private bool is_fm_qualified = false;
    private float time_vehicle_stuck;

    //respawning and death variables
    private List<int> respawn_points_visited = new List<int>();
    private Vector3 respawn_point;
    private bool is_respawning = false;

    //values from game_controller
    private game_controller game_controller;
    public int enemy_id = 0;
    public int skill_level;
    private UI_controller UI_controller;

    //components
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D box;
    private PolygonCollider2D poly;

    //checks
    [SerializeField] private Transform ground_check;
    [SerializeField] private Transform wall_check;

    //materials
    [SerializeField] private PhysicsMaterial2D friction;
    [SerializeField] private PhysicsMaterial2D no_friction;

    //layermasks
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private LayerMask wall_mask;

    //particles
    [SerializeField] private ParticleSystem death_particles;

    //wait requirements
    private Transform moving_laser_one;
    private SpriteRenderer laser_on_off_tick;
    private Transform moving_saw_one;
    private Transform moving_platform_one;
    private Transform moving_saw_two;
    private BoxCollider2D pipe_blast_one;

    //flying machine values
    private float flying_machine_speed;
    private float upward_speed;
    private float slow_fall_speed;
    private List<Coroutine> coroutine_list = new List<Coroutine>();

    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        poly = GetComponent<PolygonCollider2D>();
        box = GetComponent<BoxCollider2D>();
        circle = GetComponent<CircleCollider2D>();

        //Get movement values from player movement
        player_movement = GameObject.FindGameObjectWithTag("Player").GetComponent<player_movement>();
        player_respawn player_respawn = GameObject.FindGameObjectWithTag("Player").GetComponent<player_respawn>();
        speed = player_movement.speed;
        jump_height = player_movement.jump_height;
        wall_slide_speed = player_movement.wall_slide_speed;
        wall_jump_time = player_movement.wall_jump_time;
        wall_jump_power = player_movement.wall_jump_power;
        max_wind_speed = player_movement.max_wind_speed;
        max_boost_speed = player_movement.max_boost_speed;
        boost_decay_time = player_movement.boost_decay_time;
        boosted_wall_jump_power = player_movement.boosted_wall_jump_power;
        boosted_wall_jump_time = player_movement.boosted_wall_jump_time;
        flying_machine_speed = player_movement.flying_machine_speed;
        upward_speed = player_movement.upward_speed;
        slow_fall_speed = player_movement.slow_fall_speed;
        ice_speed = player_movement.ice_speed;
        water_splash = player_movement.water_splash;
        submarine_speed_decay_time = player_movement.submarine_speed_decay_time;
        submarine_fall_max_speed = player_movement.submarine_fall_max_speed;
        submarine_blast_recharge_time = player_movement.submarine_blast_recharge_time;
        horizontal_blast_strength = player_movement.horizontal_blast_strength;
        vertical_blast_strength = player_movement.vertical_blast_strength;
        submarine_blast_particles_A = player_movement.submarine_blast_particles_A;
        submarine_blast_particles_B = player_movement.submarine_blast_particles_B;

        //initial delay
        inital_wait_time = Random.Range(0.1f, 5);

        //get reference to game_controller
        game_controller = GameObject.FindGameObjectWithTag("GameController").GetComponent<game_controller>();
        UI_controller = GameObject.FindGameObjectWithTag("UI").GetComponent<UI_controller>();

        //get references to wait requirements
        if (game_controller.map == 2)
        {
            moving_laser_one = GameObject.FindGameObjectWithTag("moving_laser_one").transform.parent.gameObject.GetComponent<Transform>();
            laser_on_off_tick = GameObject.FindGameObjectWithTag("on_off_laser_one").transform.parent.gameObject.GetComponent<SpriteRenderer>();
            moving_saw_one = GameObject.FindGameObjectWithTag("moving_saw_one").transform.parent.gameObject.GetComponent<Transform>();
            moving_platform_one = GameObject.FindGameObjectWithTag("moving_platform_one").GetComponent<Transform>();
        }

        if (game_controller.map == 3)
        {
            moving_saw_two = GameObject.FindGameObjectWithTag("moving_saw_two").transform.parent.gameObject.GetComponent<Transform>();
            moving_platform_one = GameObject.FindGameObjectWithTag("shinobi_moving_platform").GetComponent<Transform>();
        }

        if (game_controller.map == 4)
        {
            moving_platform_one = GameObject.FindGameObjectWithTag("autumn_platform").GetComponent<Transform>();
            moving_saw_one = GameObject.FindGameObjectWithTag("autumn_saw_one").transform.parent.gameObject.GetComponent<Transform>();
            moving_saw_two = GameObject.FindGameObjectWithTag("autumn_saw_two").transform.parent.gameObject.GetComponent<Transform>();
        }
        if (game_controller.map == 5)
        {
            pipe_blast_one = GameObject.FindGameObjectWithTag("pipe_blast_one").transform.GetChild(0).GetComponent<BoxCollider2D>();
            laser_on_off_tick = GameObject.FindGameObjectWithTag("pipeway_laser").transform.parent.gameObject.GetComponent<SpriteRenderer>();
            moving_saw_one = GameObject.FindGameObjectWithTag("pipeway_saw").transform.parent.gameObject.GetComponent<Transform>();
        }

    }

    void Update()
    {
        if (!is_respawning && Time.time > inital_wait_time)
        {
            if (player_mode == "normal")
            {
                if (!is_wall_jumping)
                { execute_command(); }
                wall_jump_logic();
                wind_logic();
                boost_logic();
            }
            else if(player_mode == "submarine")
            {
                submarine_logic();
            }
            else
            {
                flying_machine_logic();
            }
            update_material();
            update_animations();
            layering_update();
            putrid_pipeways_laser();
            special_shinobi_sanctuary_logic();
            stuck_check();
        }
    }

    private void execute_command()
    {
        //colliders
        poly.enabled = false;
        box.enabled = true;
        circle.enabled = false;
        //animations
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 0);
        anim.SetBool("flying_machine", false);
        anim.SetBool("submarine", false);
        rb.gravityScale = 3.5f;

        //execute main command
        if (current_command == "move_right")
        {
            //move right
            if (is_touching_ice && is_grounded())
            {
                rb.velocity = new Vector2(ice_speed + boost_speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(speed + boost_speed, rb.velocity.y);
            }
            //check for flip
            if (!facing_right)
            {
                flip();
            }
        }
        else if (current_command == "move_left")
        {
            //move left
            if (is_touching_ice && is_grounded())
            {
                rb.velocity = new Vector2(-ice_speed - boost_speed, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-speed - boost_speed, rb.velocity.y);
            }
            //check for flip
            if (facing_right)
            {
                flip();
            }
        }
        else if (current_command == "qualified")
        {
            //check for start coroutine
            if (former_command != "qualified")
            {
                //qualified
                game_controller.players_qualified++;
                if (former_command == "move_right")
                {
                    current_command = "move_right";
                }
                else
                {
                    current_command = "move_left";
                }
                StartCoroutine(qualified_wrapup());

            }
        }

        //check for jump
        if (jump_trigger && is_grounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jump_height);
            jump_trigger = false;
        }

        //set former command for next frame
        former_command = current_command;
    }

    private void wall_jump_logic()
    {
        //check for wall_slide
        if (is_walled() && !is_grounded() && (current_command == "move_right" || current_command == "move_left"))
        {
            is_wall_sliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wall_slide_speed, float.MaxValue)); //clamps your falling velocity above the wall slide speed
            is_wall_jumping = false;
            CancelInvoke(nameof(stop_wall_jump));
        }
        else
        {
            is_wall_sliding = false;
        }
        if (is_wall_sliding && jump_trigger)
        {
            //wall jump
            jump_trigger = false;
            is_wall_jumping = true;
            if (is_touching_wall_boost)
            {
                Invoke(nameof(stop_wall_jump), boosted_wall_jump_time);
            }
            else
            {
                Invoke(nameof(stop_wall_jump), wall_jump_time);
            }
            if (current_command == "move_right")
            {
                current_command = "move_left";
                if (is_touching_wall_boost)
                {
                    rb.velocity = new Vector2(boosted_wall_jump_power.x * -1, boosted_wall_jump_power.y);
                    anim.SetTrigger("wall_boost");
                }
                else
                {
                    rb.velocity = new Vector2(wall_jump_power.x * -1, wall_jump_power.y);
                }
                flip();
            }
            else
            {
                current_command = "move_right";
                if (is_touching_wall_boost)
                {
                    rb.velocity = new Vector2(boosted_wall_jump_power.x, boosted_wall_jump_power.y);
                    anim.SetTrigger("wall_boost");
                }
                else
                {
                    rb.velocity = new Vector2(wall_jump_power.x, wall_jump_power.y);
                }
                flip();
            }
        }
    }

    private void stop_wall_jump()
    {
        is_wall_jumping = false;
    }

    private void flip()
    {
        //flip the enemy
        facing_right = !facing_right;
        Vector3 localscale = transform.localScale;
        localscale.x *= -1f;
        transform.localScale = localscale;
    }

    private void death()
    {
        death_particle();
        transform.position = respawn_point;
        rb.velocity = new Vector2(0, 0);
        current_command = current_override;
        is_respawning = true;
        StartCoroutine(restart_movement());
        anim.SetBool("is_walking", false);
        anim.SetTrigger("death");
        one_use_points.Clear();
        unlocked_points.Clear();
        cancel_boost();
        if (player_mode == "flying_machine")
        {
            player_mode = "normal";
            poly.enabled = false;
            box.enabled = true;
            anim.SetLayerWeight(2, 0);
            anim.SetBool("flying_machine", false);
        }
        if (player_mode == "submarine")
        {
            player_mode = "normal";
            circle.enabled = false;
            box.enabled = true;
            anim.SetLayerWeight(3, 0);
            anim.SetBool("submarine", false);
        }
        foreach (Coroutine coroutine in coroutine_list)
        {
            StopCoroutine(coroutine);
        }
        coroutine_list.Clear();
        fm_command_down = false;
        fm_command_left = false;
        fm_command_right = false;
        fm_command_up = false;
    }

    private IEnumerator restart_movement()
    {
        yield return new WaitForSeconds(1);
        is_respawning = false;
    }

    private void update_material()
    {
        if (is_grounded())
        {
            //friction needed
            rb.sharedMaterial = friction;
        }
        else
        {
            //no friction needed
            rb.sharedMaterial = no_friction;
        }
    }

    private bool is_grounded()
    {
        return Physics2D.OverlapCircle(ground_check.position, ground_check_radius, ground_mask);
    }

    private bool is_walled()
    {
        return Physics2D.OverlapCircle(wall_check.position, 0.2f, wall_mask);
    }

    private void update_animations()
    {
        if (current_command == "move_left" || current_command == "move_right" || current_command == "qualified")
        {
            anim.SetBool("is_walking", true);
        }
        else
        {
            anim.SetBool("is_walking", false);
        }
        anim.SetFloat("y_velocity", rb.velocity.y);
        if (!is_grounded() && anim.GetBool("is_grounded"))
        {
            anim.SetTrigger("leave_ground");
        }
        anim.SetBool("is_grounded", is_grounded());
        if (is_wall_sliding && !anim.GetBool("wall_sliding"))
        {
            anim.SetTrigger("start_wall_slide");
        }
        anim.SetBool("wall_sliding", is_wall_sliding);
    }

    private void layering_update()
    {
        int vague_layer = 200 + enemy_id * 7;
        transform.GetChild(2).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = vague_layer; //brightness effect
        transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 1; //head
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 2; //body
        transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 3; //right arm
        transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 4; //left arm
        transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 5; //left leg
        transform.GetChild(0).GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 6; //right leg

        //submarine
        transform.GetChild(0).GetChild(4).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 8; //main submarine
        transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = vague_layer; //window
        transform.GetChild(0).GetChild(4).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder = vague_layer + 1; //blocker 1
        transform.GetChild(0).GetChild(4).GetChild(2).GetComponent<SpriteRenderer>().sortingOrder = vague_layer + 2; // blocker 2
        transform.GetChild(0).GetChild(4).GetChild(3).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 9; //booster 1
        transform.GetChild(0).GetChild(4).GetChild(4).GetComponent<SpriteRenderer>().sortingOrder = vague_layer - 9; // booster 2
        transform.GetChild(0).GetChild(4).GetChild(5).GetComponent<SpriteRenderer>().sortingOrder = vague_layer; //eye glow 1
        transform.GetChild(0).GetChild(4).GetChild(6).GetComponent<SpriteRenderer>().sortingOrder = vague_layer; //eye glow 2
        transform.GetChild(0).GetChild(4).GetChild(7).GetComponent<SpriteRenderer>().sortingOrder = vague_layer; //charge glow 1
        transform.GetChild(0).GetChild(4).GetChild(8).GetComponent<SpriteRenderer>().sortingOrder = vague_layer; //charge glow 2
    }

    private void death_particle()
    {
        Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
    }

    private IEnumerator qualified_wrapup()
    {
        float wait_time = Random.Range(min_qualified_walk_time, max_qualified_walk_time);
        yield return new WaitForSeconds(wait_time);
        current_command = "wait";
    }

    private void wind_logic()
    {
        if (is_touching_wind)
        {
            if (!former_Wind)
            {
                base_wind_speed = rb.velocity.y;
            }
            time_in_wind += Time.deltaTime;
            float wind_velocity = Mathf.Clamp(base_wind_speed + time_in_wind * 10, float.MinValue, max_wind_speed);
            rb.velocity = new Vector2(rb.velocity.x, wind_velocity);
        }
        if (is_touching_down_wind)
        {
            if (!former_Wind)
            {
                base_wind_speed = rb.velocity.y;
            }
            time_in_wind += Time.deltaTime;
            float wind_velocity = Mathf.Clamp(base_wind_speed - (time_in_wind * 15), -max_wind_speed * 1.5f, float.MaxValue);
            rb.velocity = new Vector2(rb.velocity.x, wind_velocity);
        }
        former_Wind = is_touching_wind || is_touching_down_wind;
    }
    private void boost_logic()
    {
        if (touching_boost && !boost_active)
        {
            boost_active = true;
            boost_speed = max_boost_speed;
            StartCoroutine(boost_run());
        }
        if (!boost_active)
        {
            boost_speed = 0;
        }
    }
    private IEnumerator boost_run()
    {
        anim.SetTrigger("boost");
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(boost_decay_time / 100);
            boost_speed -= max_boost_speed / 100;
        }
        boost_active = false;
    }
    private void cancel_boost()
    {
        StopCoroutine(boost_run());
        boost_active = false;
        boost_speed = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            //command point logic
            command_point_logic(collision.gameObject.GetComponent<enemy_points>());
        }
        else if (collision.gameObject.tag == "kill" || collision.gameObject.tag == "enemy_kill")
        {
            //death logic
            death();

        }
        else if (collision.gameObject.tag == "enemy_respawn")
        {
            //respawn point logic
            enemy_respawn_point respawn = collision.gameObject.GetComponent<enemy_respawn_point>();
            if (!respawn_points_visited.Contains(respawn.respawn_number))
            {
                //set respawn point logic
                respawn_points_visited.Add(respawn.respawn_number);
                current_command = respawn.override_command;
                current_override = respawn.override_command;
                respawn_point = respawn.gameObject.transform.position;
                one_use_points.Clear();
                unlocked_points.Clear();
            }

        }
        else if (collision.gameObject.tag == "bouncy")
        {
            rb.velocity = new Vector2(rb.velocity.x, collision.gameObject.GetComponent<bouncy_logic>().bounce_height);
            anim.SetBool("is_grounded", true);
        }
        //wind
        if (collision.gameObject.tag == "wind")
        {
            is_touching_wind = true;
        }
        if (collision.gameObject.tag == "down_wind")
        {
            is_touching_down_wind = true;
        }
        //boost
        if (collision.gameObject.tag == "boost")
        {
            touching_boost = true;
        }
        //wall boost
        if (collision.gameObject.tag == "wall_boost")
        {
            is_touching_wall_boost = true;
        }
        // flying machine
        if (collision.gameObject.tag == "flying_machine" && !(player_mode == "flying_machine"))
        {
            player_mode = "flying_machine";
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z);
            Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
        }
        //ice
        if (collision.gameObject.tag == "ice")
        {
            is_touching_ice = true;
        }
        // submarine
        if (collision.gameObject.tag == "submarine" && !(player_mode == "submarine"))
        {
            player_mode = "submarine";
            Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
            submarine_blast_charged = false;
            recharged_time = Time.time + submarine_blast_recharge_time;
            charge_time_set = true;
        }
        if (collision.gameObject.tag == "water")
        {
            is_touching_water = true;
            Instantiate(water_splash, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
        }
        if (collision.gameObject.tag == "submarine_special")
        {
            is_touching_subnarine_special_zone = true;
        }
        if (collision.gameObject.tag == "submarine_special_two")
        {
            is_touching_submarine_special_zone_2 = true;
        }
        if (collision.gameObject.tag == "remove_submarine" && player_mode == "submarine")
        {
            player_mode = "normal";
            anim.SetLayerWeight(2, 0);
            anim.SetLayerWeight(3, 0);
            anim.SetBool("submarine", false);
            box.enabled = true;
            poly.enabled = false;
            circle.enabled = false;
            Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
        }

    }

    private void command_point_logic(enemy_points point)
    {
        if ((!point.contains_threshold || (point.contains_threshold && chance_number <= point.upper_threshold && chance_number > point.lower_threshold)) && (!one_use_points.Contains(point.transform.position)) && (!point.is_locked || point.is_locked && unlocked_points.Contains(point.key_code)))
        {
            //check for unlock
            if (point.has_unlock && !point.is_locked)
            {
                unlocked_points.Add(point.key_code);
            }
            //check for one use point
            if (point.once_per_respawn)
            {
                one_use_points.Add(point.transform.position);
            }
            if (point.has_delay_string)
            {
                wait_system(point.command, point.delay_string);
            }
            else
            {
                //check for and set command
                if (point.contains_command)
                {
                    current_command = point.command;
                }
                //check for and set trigger
                if (point.contains_trigger)
                {
                    if (point.trigger == "jump")
                    {
                        jump_trigger = true;
                    }
                    else if (point.trigger == "roll_chance")
                    {
                        chance_number = Random.Range(1, 101) + skill_level;
                        //Debug.Log(chance_number);
                    }
                    else if (point.trigger == "clear_unlocks")
                    {
                        unlocked_points.Clear();
                    }
                    else if (point.trigger == "shinobi_routes")
                    {
                        //shinobi sanctuary routes
                        int shinobi_choice = Random.Range(0, 4);
                        if (shinobi_choice == 3)
                        {
                            shinobi_choice = GameObject.FindGameObjectWithTag("boost_controller").GetComponent<random_boost_controller>().random_boost;
                        }

                        unlocked_points.Add(shinobi_choice + 3);
                    }
                    else if (point.trigger == "clear_one_use")
                    {
                        one_use_points.Clear();
                    }
                    else if(point.trigger == "left_blast")
                    {
                        left_blast_trigger = true;
                    }
                    else if (point.trigger == "right_blast")
                    {
                        right_blast_trigger = true;
                    }
                    else if (point.trigger == "up_blast")
                    {
                        up_blast_trigger = true;
                    }else if (point.trigger == "submarine_startup")
                    {
                        StartCoroutine(submarine_startup());
                    }
                    else if (point.trigger == "laser_up_blast" && on_off_laser_pipeway())
                    {
                        up_blast_trigger = true;
                    }
                    else if (point.trigger == "saw_up_blast" && moving_saw_pipeway())
                    {
                        up_blast_trigger = true;
                    }
                }
                //check for door
                if (point.transport_destination != new Vector2(0, 0))
                {
                    transform.localPosition = point.transport_destination;
                }
                //check for flying machine command
                if (point.contains_flying_command)
                {
                    coroutine_list.Add(
                    StartCoroutine(flying_machine_command(point.flying_command, point.flying_command_time))
                    );
                }
            }
        }
    }

    //wait logic
    private void wait_system(string finish_command, string delay_string)
    {
        current_command = "wait";
        if (delay_string == "moving_laser_one")
        {
            StartCoroutine(moving_laser_one_wait(finish_command));
        }
        else if (delay_string == "moving_laser_two")
        {
            StartCoroutine(moving_laser_two_wait(finish_command));
            jump_trigger = false;
        }
        else if (delay_string == "on_off_laser_one")
        {
            StartCoroutine(on_off_laser_one_wait(finish_command));
        }
        else if (delay_string == "moving_saw_one")
        {
            StartCoroutine(moving_saw_one_wait(finish_command));
        }
        else if (delay_string == "on_off_laser_two")
        {
            StartCoroutine(on_off_laser_two_wait(finish_command));
        }
        else if (delay_string == "on_off_laser_three")
        {
            StartCoroutine(on_off_laser_three_wait(finish_command));
        }
        else if (delay_string == "moving_platform_one")
        {
            StartCoroutine(moving_platform_one_wait(finish_command));
        }
        else if (delay_string == "moving_platform_two")
        {
            StartCoroutine(moving_platform_two_wait(finish_command));
        }
        else if (delay_string == "shinobi_saw")
        {
            StartCoroutine(shinobi_saw_wait(finish_command));
        }
        else if (delay_string == "moving_platform_three")
        {
            StartCoroutine(moving_platform_three_wait(finish_command));
        }
        else if (delay_string == "moving_platform_four")
        {
            StartCoroutine(moving_platform_four_wait(finish_command));
        }
        else if (delay_string == "moving_platform_five")
        {
            StartCoroutine(moving_platform_five_wait(finish_command));
        }
        else if (delay_string == "autumn_saw_one")
        {
            StartCoroutine(autumn_saw_one(finish_command));
        }
        else if (delay_string == "autumn_saw_two")
        {
            StartCoroutine(autumn_saw_two(finish_command));
        }
        else if (delay_string == "pipe_blast_one")
        {
            StartCoroutine(pipe_blast_one_wait(finish_command));
        }
        else if (delay_string == "pipe_blast_two")
        {
            StartCoroutine(pipe_blast_two_wait(finish_command));
        }
    }

    //revamped raceway wait systems
    private IEnumerator moving_laser_one_wait(string finish_command)
    {
        while (moving_laser_one.localPosition.y > 45.5f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(1, 2));
        current_command = finish_command;
    }
    private IEnumerator moving_laser_two_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_laser_one.localPosition.y < 58f)
        {
            yield return null;
        }
        while (moving_laser_one.localPosition.y > 50f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        current_command = finish_command;
    }
    private IEnumerator on_off_laser_one_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (laser_on_off_tick.enabled == true)
        {
            yield return null;
        }
        while (laser_on_off_tick.enabled == false)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0.9f, 1));
        current_command = finish_command;
    }
    private IEnumerator moving_saw_one_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_saw_one.localPosition.y < 68)
        {
            yield return null;
        }
        while (moving_saw_one.localPosition.y > 68)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        current_command = finish_command;
    }
    private IEnumerator on_off_laser_two_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (laser_on_off_tick.enabled == false)
        {
            yield return null;
        }
        while (laser_on_off_tick.enabled == true)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(1.5f, 1.8f));
        current_command = finish_command;
    }
    private IEnumerator on_off_laser_three_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (laser_on_off_tick.enabled == false)
        {
            yield return null;
        }
        while (laser_on_off_tick.enabled == true)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.2f));
        current_command = finish_command;
    }
    private IEnumerator moving_platform_one_wait(string finish_command)
    {
        ground_check_radius = 0.3f;
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_platform_one.localPosition.y < 38)
        {
            yield return null;
        }
        while (moving_platform_one.localPosition.y > 38)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        current_command = finish_command;
    }
    private IEnumerator moving_platform_two_wait(string finish_command)
    {
        //revamped raceway settings
        StartCoroutine(restore_ground_check());
        min_qualified_walk_time = 0.2f;
        max_qualified_walk_time = 1.2f;

        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_platform_one.localPosition.y < 49)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        current_command = finish_command;
    }
    private IEnumerator restore_ground_check()
    {
        yield return new WaitForSeconds(2.5f);
        ground_check_radius = 0.2f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //wind
        if (collision.gameObject.tag == "wind")
        {
            is_touching_wind = false;
            time_in_wind = 0;
        }
        if (collision.gameObject.tag == "down_wind")
        {
            is_touching_down_wind = false;
            time_in_wind = 0;
        }
        //boost
        if (collision.gameObject.tag == "boost")
        {
            touching_boost = false;
        }
        //wall boost
        if (collision.gameObject.tag == "wall_boost")
        {
            is_touching_wall_boost = false;
        }
        if (collision.gameObject.tag == "ice")
        {
            is_touching_ice = false;
        }
        if (collision.gameObject.tag == "water")
        {
            is_touching_water = false;
        }
        if (collision.gameObject.tag == "submarine_special")
        {
            is_touching_subnarine_special_zone = false;
        }
        if (collision.gameObject.tag == "submarine_special_two")
        {
            is_touching_submarine_special_zone_2 = false;
        }
    }

    //shinobi sanctuary wait systems
    private IEnumerator shinobi_saw_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        if (GameObject.FindGameObjectWithTag("boost_controller").GetComponent<random_boost_controller>().random_boost == 2)
        {
            unlocked_points.Add(11);
        }
        else
        {
            unlocked_points.Add(10);
        }
        while (moving_saw_two.localPosition.x > 82)
        {
            yield return null;
        }
        while (moving_saw_two.localPosition.x < 82)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0.05f, 0.1f));
        current_command = finish_command;
    }
    private IEnumerator moving_platform_three_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_platform_one.localPosition.x < 283)
        {
            yield return null;
        }
        while (moving_platform_one.localPosition.x > 283)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        current_command = finish_command;
    }
    private IEnumerator moving_platform_four_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_platform_one.localPosition.x > 293)
        {
            yield return null;
        }
        while (moving_platform_one.localPosition.x < 293)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        current_command = finish_command;
    }

    private void special_shinobi_sanctuary_logic()
    {
        if (game_controller.map == 3 && UI_controller.checkpoint_number > 4)
        {
            //fix for checkpoint 3
            if (respawn_point == new Vector3(807.81f, 205.02f, 0))
            {
                transform.position = new Vector3(878.27f, 182.1f, 0);
            }
        }
    }
    private IEnumerator moving_platform_five_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_platform_one.localPosition.x > -32.5f)
        {
            yield return null;
        }
        while (moving_platform_one.localPosition.x < -32.5f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.01f));
        current_command = finish_command;
    }
    private IEnumerator autumn_saw_one(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_saw_one.localPosition.y > 72f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        while (moving_saw_one.localPosition.y < 72f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.01f));
        current_command = finish_command;
    }

    private void flying_machine_logic()
    {
        //update material
        rb.sharedMaterial = friction;
        rb.gravityScale = 3.5f;
        //get animations
        anim.SetLayerWeight(2, 1);
        anim.SetLayerWeight(3, 0);
        anim.SetBool("flying_machine", true);
        anim.SetBool("submarine", false);
        //collider
        poly.enabled = true;
        box.enabled = false;
        circle.enabled = false;
        //scale
        facing_right = true;
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);

        //up down left right
        if (fm_command_left)
        {
            //move left
            rb.velocity = new Vector2(-flying_machine_speed, rb.velocity.y);
            anim.SetBool("flying_moving_right", false);
            anim.SetBool("flying_moving_left", true);
        }
        else if (fm_command_right)
        {
            //move right
            rb.velocity = new Vector2(flying_machine_speed, rb.velocity.y);
            anim.SetBool("flying_moving_right", true);
            anim.SetBool("flying_moving_left", false);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("flying_moving_right", false);
            anim.SetBool("flying_moving_left", false);
        }
        if (fm_command_up)
        {
            //move up
            rb.velocity = new Vector2(rb.velocity.x, upward_speed);
        }
        else if (fm_command_down)
        {
            //move down
            rb.velocity = new Vector2(rb.velocity.x, -upward_speed);
        }
        else
        {
            //slow fall
            rb.velocity = new Vector2(rb.velocity.x, -slow_fall_speed);
        }
        //qualified check
        //check for start coroutine
        if (current_command == "qualified")
        {
            //qualified
            game_controller.players_qualified++;
            current_command = "move_left";
            is_fm_qualified = true;
            StartCoroutine(flying_machine_command("left", Random.Range(0.2f, 1.5f)));
        }
    }

    private IEnumerator flying_machine_command(string command, float time)
    {
        if (command == "left")
        {
            fm_command_left = true;
        }
        else if (command == "right")
        {
            fm_command_right = true;
        }
        else if (command == "down")
        {
            fm_command_down = true;
        }
        else
        {
            fm_command_up = true;
        }
        yield return new WaitForSeconds(time);
        if (command == "left")
        {
            fm_command_left = false;
        }
        else if (command == "right")
        {
            fm_command_right = false;
        }
        else if (command == "down")
        {
            fm_command_down = false;
        }
        else
        {
            fm_command_up = false;
        }
    }
    private IEnumerator autumn_saw_two(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (moving_saw_two.localPosition.y > 127f)
        {
            yield return null;
        }
        while (moving_saw_two.localPosition.y < 127f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(0, 0.1f));
        coroutine_list.Add(
        StartCoroutine(flying_machine_command("left", 1f))
        );
    }
    private IEnumerator pipe_blast_one_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (pipe_blast_one.enabled == true)
        {
            yield return null;
        }
        while (pipe_blast_one.enabled == false)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(1.2f, 1.3f));
        current_command = finish_command;
    }
    private IEnumerator pipe_blast_two_wait(string finish_command)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        while (pipe_blast_one.enabled == true)
        {
            yield return null;
        }
        while (pipe_blast_one.enabled == false)
        {
            yield return null;
        }
        yield return new WaitForSeconds(Random.Range(1f, 1.2f));
        current_command = finish_command;
    }
    private void submarine_logic()
    {
        //update material
        rb.sharedMaterial = friction;
        //get animations
        anim.SetLayerWeight(3, 1);
        anim.SetLayerWeight(2,0);
        anim.SetBool("submarine", true);
        anim.SetBool("flying_machine", true);
        anim.SetBool("submarine charged", submarine_blast_charged);
        //collider
        poly.enabled = false;
        box.enabled = false;
        circle.enabled = true;
        //scale
        facing_right = true;
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        if (is_touching_water)
        {
            rb.gravityScale = 3f;
        }
        else
        {
            rb.gravityScale = 5f;
        }

        //submarine movement
        if (submarine_blast_charged && is_touching_water)
        {
            if (up_blast_trigger)
            {
                up_blast_trigger = false;
                submarine_blast_charged = false;
                if (is_touching_subnarine_special_zone)
                { rb.velocity = new Vector2(rb.velocity.x, vertical_blast_strength * 1.5f); }
                else { rb.velocity = new Vector2(rb.velocity.x, vertical_blast_strength); }
                GameObject particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x + 1.51f, transform.position.y + 0.14f, 0f), Quaternion.identity, transform);
                particles.transform.eulerAngles = new Vector3(0, 0, -120.524f);
                particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x - 1.51f, transform.position.y + 0.14f, 0f), Quaternion.identity, transform);
                particles.transform.eulerAngles = new Vector3(0, 0, -120.524f);
            }
            else if (left_blast_trigger)
            {
                left_blast_trigger = false;
                submarine_blast_charged = false;
                rb.velocity = new Vector2(-horizontal_blast_strength, rb.velocity.y);
                GameObject particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x + 1.31f, transform.position.y + 0.15f, 0f), Quaternion.identity, transform);
                particles.transform.eulerAngles = new Vector3(0, 0, -30f);
            }
            else if (right_blast_trigger)
            {
                right_blast_trigger = false;
                submarine_blast_charged = false;
                rb.velocity = new Vector2(horizontal_blast_strength, rb.velocity.y);
                GameObject particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x - 1.31f, transform.position.y + 0.15f, 0f), Quaternion.identity, transform);
                particles.transform.eulerAngles = new Vector3(0, 0, 145f);
            }
        }
        else if (is_touching_subnarine_special_zone && left_blast_trigger && submarine_blast_charged && !is_touching_water)
        {
            left_blast_trigger = false;
            submarine_blast_charged = false;
            rb.velocity = new Vector2(-horizontal_blast_strength * 2.5f, 0);
            GameObject particles = Instantiate(submarine_blast_particles_B, new Vector3(transform.position.x + 1.31f, transform.position.y + 0.15f, 0f), Quaternion.identity, transform);
            particles.transform.eulerAngles = new Vector3(0, 0, -30f);
        }


        //charge
        if (!submarine_blast_charged)
        {
            if (charge_time_set)
            {
                if (recharged_time < Time.time)
                {
                    charge_time_set = false;
                    submarine_blast_charged = true;
                }
            }
            else
            {
                if (is_touching_submarine_special_zone_2) { recharged_time = Time.time + submarine_blast_recharge_time / 2; }
                else { recharged_time = Time.time + submarine_blast_recharge_time; }
                charge_time_set = true;
            }
        }

        //speed decay
        if (rb.velocity.x < 0)
        {
            rb.velocity = new Vector2(rb.velocity.x + submarine_speed_decay_time * Time.deltaTime, rb.velocity.y);
            if (rb.velocity.x > 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else if (rb.velocity.x > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x - submarine_speed_decay_time * Time.deltaTime, rb.velocity.y);
            if (rb.velocity.x < 0)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if (is_touching_water)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -submarine_fall_max_speed, float.MaxValue));
        }

    }
    private IEnumerator submarine_startup()
    {
        float target_time = Time.time + 1;
        while( Time.time < target_time)
        {
            rb.velocity = new Vector2(-10f, rb.velocity.y);
            yield return null;
        }
    }

    private void putrid_pipeways_laser()
    {
        if (game_controller.map == 5)
        {
            laser_cycle += Time.deltaTime;
            if (former_laser_cycle == true && laser_on_off_tick.enabled == false)
            {
                laser_cycle = 0;
            }
            former_laser_cycle = laser_on_off_tick.enabled;

            if (former_saw_pos < moving_saw_one.position.x)
            {
                saw_dir_right = true;
            }
            else
            {
                saw_dir_right = false;
            }

            former_saw_pos = moving_saw_one.position.x;
        }
    }

    private bool on_off_laser_pipeway()
    {
        return !(laser_cycle < 0.5f);
    }
    private bool moving_saw_pipeway()
    {
        return !(moving_saw_one.position.x > -114.7f && moving_saw_one.position.x < -107.9f && !saw_dir_right);
    }
    private void stuck_check()
    {
        if((transform.position == former_position && (current_command == "move_left" || current_command == "move_right") && player_mode == "normal") ||(transform.position == former_position && player_mode == "submarine") || (transform.position == former_position && player_mode == "flying_machine" && !is_fm_qualified))
        {
            time_stuck += Time.deltaTime;
            if(time_stuck > time_stuck_to_delete)
            {
                death();
                time_stuck = 0;
            }
        }else
        {
            time_stuck = 0;
        }
        if((player_mode == "submarine" || player_mode == "flying_machine") && transform.position.x == former_position.x && (!(player_mode == "flying_machine") || (player_mode == "flying_machine" && !is_fm_qualified)))
        {
            time_vehicle_stuck += Time.deltaTime;
            if (time_vehicle_stuck > 60)
            {
                death();
                time_vehicle_stuck = 0;
            }
        }
        else
        {
            time_vehicle_stuck = 0;
        }
        former_position = transform.position;
    }
}