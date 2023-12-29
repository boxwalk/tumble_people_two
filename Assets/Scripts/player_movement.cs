using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    //components 
    private Rigidbody2D rb;
    private Animator anim;
    private BoxCollider2D box;
    private PolygonCollider2D poly;
    private CircleCollider2D circle;
    [HideInInspector] public Animator cam_animator;
    private player_respawn player_respawn;

    //serialized movement values
    public float speed;
    public float jump_height;
    public float wall_slide_speed;
    public float wall_jump_time;
    public Vector2 wall_jump_power;
    [SerializeField] private float ground_check_radius;
    [SerializeField] private float wall_check_radius;
    private bool is_touching_wind;
    private bool is_touching_down_wind;
    private float time_in_wind;
    public float max_wind_speed;
    private bool touching_boost = false;
    private bool boost_active = false;
    private float boost_speed;
    public float max_boost_speed;
    public float boost_decay_time;
    public Vector2 boosted_wall_jump_power;
    public float boosted_wall_jump_time;
    private bool is_touching_ice;
    public float ice_speed;
    private bool is_touching_water = false;
    public ParticleSystem water_splash;
    [HideInInspector] public bool submarine_Activated = false;
    public float submarine_speed_decay_time;
    public float submarine_fall_max_speed;
    public float submarine_blast_recharge_time;
    public float vertical_blast_strength;
    public float horizontal_blast_strength;
    private bool submarine_blast_charged = false;
    private bool charge_time_set = false;
    private bool is_touching_subnarine_special_zone = false;
    private bool is_touching_submarine_special_zone_2 = false;
    [SerializeField] private LayerMask no_friction_mask;

    //movement variables
    private bool facing_right = true;
    private bool is_jumping = false;
    private float jump_reset_timer;
    private bool is_wall_sliding;
    [HideInInspector] public bool is_wall_jumping;
    private bool is_touching_wall_boost = false;
    [HideInInspector] public string player_mode = "normal";
    private float recharged_time;

    //checks
    [SerializeField] private Transform ground_check;
    [SerializeField] private Transform wall_check;

    //layermasks
    [SerializeField] private LayerMask ground_mask;
    [SerializeField] private LayerMask wall_mask;

    //materials
    [SerializeField] private PhysicsMaterial2D friction;
    [SerializeField] private PhysicsMaterial2D no_friction;

    //particles
    [SerializeField] private ParticleSystem death_particles;
    public GameObject submarine_blast_particles_A;
    public GameObject submarine_blast_particles_B;

    //flying machine values
    public float flying_machine_speed;
    public float upward_speed;
    public float slow_fall_speed;

    void Start()
    {
        //Get components
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        box = GetComponent<BoxCollider2D>();
        poly = GetComponent<PolygonCollider2D>();
        circle = GetComponent<CircleCollider2D>();
        cam_animator = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Animator>();
        player_respawn = GetComponent<player_respawn>();
    }

    void Update()
    {
        if(player_mode == "normal")
        {
            Player_movement();
            get_animations();
            update_material();
        }else if(player_mode == "flying_machine")
        {
            flying_machine_movement();
        }else if (player_mode == "submarine")
        {
            submarine_movement();
        }
    }

    private void Player_movement()
    {
        poly.enabled = false;
        box.enabled = true;
        circle.enabled = false;
        rb.gravityScale = 3.5f;
        //Reset or get animations
        anim.SetBool("is_walking", false);
        //Horizontal movement
        if (Input.GetKey(KeyCode.A) && !is_wall_jumping)
        {
            //move left
            if (is_touching_ice && is_grounded()){
                rb.velocity = new Vector2(-ice_speed + (boost_speed*-1), rb.velocity.y);
            }else{
                rb.velocity = new Vector2(-speed + (boost_speed * -1), rb.velocity.y);}

            anim.SetBool("is_walking", true);
        }
        if (Input.GetKey(KeyCode.D) && !is_wall_jumping)
        {
            //move right
            if (is_touching_ice && is_grounded()){
                rb.velocity = new Vector2(ice_speed + boost_speed, rb.velocity.y);
            }else{
                rb.velocity = new Vector2(speed + boost_speed, rb.velocity.y);}

                anim.SetBool("is_walking", true);
        }
        //Jumping logic
        if(Input.GetKeyDown(KeyCode.W) && is_grounded())
        {
            //jump
            rb.velocity = new Vector2(rb.velocity.x, jump_height);
            is_jumping = true;
            jump_reset_timer = Time.time + 0.3f;
        }
        //wallslide logic
        if (is_walled() && !is_grounded() && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
        {
            is_wall_sliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wall_slide_speed, float.MaxValue)); //wall sliding formula from unity docs
            is_wall_jumping = false;
            CancelInvoke(nameof(stop_wall_jump));
        }
        else
        {
            is_wall_sliding = false;
        }
        //walljump logic
        if(is_wall_sliding && Input.GetKeyDown(KeyCode.W))
        {
            is_wall_jumping = true;
            if (is_touching_wall_boost)
            {
                Invoke(nameof(stop_wall_jump), boosted_wall_jump_time);
            }else
            {
                Invoke(nameof(stop_wall_jump), wall_jump_time);
            }
            if (Input.GetKey(KeyCode.A))
            {
                if (is_touching_wall_boost)
                {
                    rb.velocity = new Vector2(boosted_wall_jump_power.x, boosted_wall_jump_power.y);
                    anim.SetTrigger("wall_boost");
                }else
                {
                    rb.velocity = new Vector2(wall_jump_power.x, wall_jump_power.y);
                }
            }else
            {
                if (is_touching_wall_boost)
                {
                    rb.velocity = new Vector2(boosted_wall_jump_power.x * -1, boosted_wall_jump_power.y);
                    anim.SetTrigger("wall_boost");
                }else
                {
                    rb.velocity = new Vector2(wall_jump_power.x * -1, wall_jump_power.y);
                }
            }
        }
        //flip
        flip();
        experimental_snappy_controls();
        wind_logic();
        boost_logic();
        water_backup_logic();
    }

    public bool is_grounded()
    {
        return Physics2D.OverlapCircle(ground_check.position, ground_check_radius, ground_mask);
    }

    private void get_animations()
    {
        ///weights
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 0);
        anim.SetBool("flying_machine", false);
        anim.SetBool("submarine", false);
        //grounded values
        if (anim.GetBool("is_grounded") && !is_grounded())
        {
            anim.SetTrigger("leave_ground");
        }
        anim.SetBool("is_grounded", is_grounded());

        //takeoff check
        if(jump_reset_timer < Time.time && is_grounded())
        {
            is_jumping = false;
        }
        anim.SetBool("is_jumping", is_jumping);

        //wallslide values 
        if (!anim.GetBool("wall_sliding") && is_wall_sliding)
        {
            anim.SetTrigger("start_wall_slide");
        }
        anim.SetBool("wall_sliding", is_wall_sliding);

    }

    private void flip()
    {
        //check if facing right matches the velocity
        if (facing_right && rb.velocity.x < 0f || !facing_right && rb.velocity.x > 0f)
        {
            //flip the player
            facing_right = !facing_right;
            Vector3 localscale = transform.localScale;
            localscale.x *= -1f;
            transform.localScale = localscale;
        }
    }

    private void update_material()
    {
        if (is_grounded() && !box.IsTouchingLayers(no_friction_mask))
        {
            //friction needed
            rb.sharedMaterial = friction;
        }else
        {
            //no friction needed
            rb.sharedMaterial = no_friction;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ground_check.position, ground_check_radius);
        Gizmos.DrawWireSphere(wall_check.position, wall_check_radius);
    }

    private bool is_walled()
    {
        return Physics2D.OverlapCircle(wall_check.position, wall_check_radius, wall_mask);
    }

    private void stop_wall_jump()
    {
        is_wall_jumping = false;
    }

    private void experimental_snappy_controls()
    {
        //experimental snappy test - add to "player_movement" method to make controls more snappy
        if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A) && !is_wall_jumping && is_grounded() && !box.IsTouchingLayers(no_friction_mask) && !is_touching_ice)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private void water_backup_logic()
    {
        if (is_touching_water)
        {
            rb.gravityScale = 1.5f;
        }
        else
        {
            rb.gravityScale = 3.5f;
        }
    }

    private void wind_logic()
    {
        if (is_touching_wind)
        {
            time_in_wind += Time.deltaTime;
            float wind_velocity = Mathf.Clamp(rb.velocity.y + time_in_wind * 2 , float.MinValue, max_wind_speed);
            rb.velocity = new Vector2(rb.velocity.x, wind_velocity);
        }
        if (is_touching_down_wind)
        {
            time_in_wind += Time.deltaTime;
            float wind_velocity = Mathf.Clamp(rb.velocity.y + (time_in_wind * -2), -max_wind_speed*1.5f, float.MaxValue);
            rb.velocity = new Vector2(rb.velocity.x, wind_velocity);
        }
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
        for(int i =0; i<100; i++)
        {
            yield return new WaitForSeconds(boost_decay_time / 100);
            boost_speed -= max_boost_speed / 100;
        }
        boost_active = false;
    }

    public void cancel_boost()
    {
        StopCoroutine(boost_run());
        boost_active = false;
        boost_speed = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //bouncy 
        if(collision.gameObject.tag == "bouncy")
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
        if(collision.gameObject.tag == "flying_machine" && !(player_mode == "flying_machine"))
        {
            player_mode = "flying_machine";
            transform.position = new Vector3(transform.position.x, transform.position.y + 1.8f, transform.position.z);
            Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
            cam_animator.SetTrigger("zoom_out");
        }
        if (collision.gameObject.tag == "submarine" && !(player_mode == "submarine"))
        {
            rb.velocity = new Vector2(-10f, rb.velocity.y);
            submarine_Activated = false;
            StartCoroutine(activate_submarine());
            player_mode = "submarine";
            Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
            cam_animator.SetTrigger("zoom_out");
            submarine_blast_charged = false;
            recharged_time = Time.time + submarine_blast_recharge_time;
            charge_time_set = true;
        }
        if (collision.gameObject.tag == "ice")
        {
            is_touching_ice = true;
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
        if (collision.gameObject.tag == "remove_submarine" && player_mode == "submarine")
        {
            player_respawn.remove_submarine();
            Instantiate(death_particles, transform.position, Quaternion.identity, GameObject.FindGameObjectWithTag("prefab").transform);
        }
        if (collision.gameObject.tag == "submarine_special_two")
        {
            is_touching_submarine_special_zone_2 = true;
        }
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


    // flying machine logic
    void flying_machine_movement()
    {
        //update material
        rb.sharedMaterial = friction;
        rb.gravityScale = 3.5f;
        //get animations
        anim.SetLayerWeight(2, 1);
        anim.SetLayerWeight(3,0);
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
        if (Input.GetKey(KeyCode.A))
        {
            //move left
            rb.velocity = new Vector2(-flying_machine_speed , rb.velocity.y);
            anim.SetBool("flying_moving_right", false);
            anim.SetBool("flying_moving_left", true);
        }else if (Input.GetKey(KeyCode.D))
        {
            //move right
            rb.velocity = new Vector2(flying_machine_speed, rb.velocity.y);
            anim.SetBool("flying_moving_right", true);
            anim.SetBool("flying_moving_left", false);
        }else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("flying_moving_right", false);
            anim.SetBool("flying_moving_left", false);
        }
        if (Input.GetKey(KeyCode.W))
        {
            //move up
            rb.velocity = new Vector2(rb.velocity.x, upward_speed);
        }else if (Input.GetKey(KeyCode.S))
        {
            //move down
            rb.velocity = new Vector2(rb.velocity.x, -upward_speed);
        } else
        {
            //slow fall
            rb.velocity = new Vector2(rb.velocity.x, -slow_fall_speed);
        }
    }
    private IEnumerator activate_submarine()
    {
        yield return new WaitForSeconds(0.5f);
        submarine_Activated = true;

    }
    void submarine_movement()
    {
        //update material
        rb.sharedMaterial = no_friction;
        //get animations
        anim.SetLayerWeight(2, 0);
        anim.SetLayerWeight(3, 1);
        anim.SetBool("submarine", true);
        anim.SetBool("flying_machine", false);
        anim.SetBool("submarine_charged", submarine_blast_charged);
        //collider
        poly.enabled = false;
        box.enabled = false;
        circle.enabled = true;
        //scale
        facing_right = true;
        transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        //rigidbody
        if (is_touching_water)
        {
            rb.gravityScale = 3f;
        }else
        {
            rb.gravityScale = 5f;
        }
        if (submarine_Activated)
        {
            //submarine movement
            if (submarine_blast_charged && is_touching_water)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    submarine_blast_charged = false;
                    if (is_touching_subnarine_special_zone)
                    {rb.velocity = new Vector2(rb.velocity.x, vertical_blast_strength*1.5f);}
                    else{rb.velocity = new Vector2(rb.velocity.x, vertical_blast_strength);}
                    GameObject particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x +1.51f, transform.position.y + 0.14f, 0f), Quaternion.identity, transform);
                    particles.transform.eulerAngles = new Vector3(0, 0, -120.524f);
                    particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x - 1.51f, transform.position.y + 0.14f, 0f), Quaternion.identity, transform);
                    particles.transform.eulerAngles = new Vector3(0, 0, -120.524f);
                }
                else if (Input.GetKeyDown(KeyCode.A))
                {
                    submarine_blast_charged = false;
                    rb.velocity = new Vector2(-horizontal_blast_strength, rb.velocity.y);
                    GameObject particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x + 1.31f, transform.position.y + 0.15f, 0f), Quaternion.identity, transform);
                    particles.transform.eulerAngles = new Vector3(0, 0, -30f);
                }
                else if (Input.GetKeyDown(KeyCode.D))
                {
                    submarine_blast_charged = false;
                    rb.velocity = new Vector2(horizontal_blast_strength, rb.velocity.y);
                    GameObject particles = Instantiate(submarine_blast_particles_A, new Vector3(transform.position.x - 1.31f, transform.position.y + 0.15f, 0f), Quaternion.identity, transform);
                    particles.transform.eulerAngles = new Vector3(0, 0, 145f);
                }
            } else if (is_touching_subnarine_special_zone && Input.GetKeyDown(KeyCode.A) && submarine_blast_charged && !is_touching_water)
            {
                submarine_blast_charged = false;
                rb.velocity = new Vector2(-horizontal_blast_strength*2.5f, 0);
                GameObject particles = Instantiate(submarine_blast_particles_B, new Vector3(transform.position.x + 1.31f, transform.position.y + 0.15f, 0f), Quaternion.identity, transform);
                particles.transform.eulerAngles = new Vector3(0, 0, -30f);
            }
            //charge
            if (!submarine_blast_charged)
            {
                if (charge_time_set)
                {
                    if(recharged_time < Time.time)
                    {
                        charge_time_set = false;
                        submarine_blast_charged = true;
                    }
                }
                else{
                    if (is_touching_submarine_special_zone_2) { recharged_time = Time.time + submarine_blast_recharge_time/2; }
                    else { recharged_time = Time.time + submarine_blast_recharge_time; }
                    charge_time_set = true;
                }
            }
            //speed decay
            if(rb.velocity.x < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x + submarine_speed_decay_time*Time.deltaTime, rb.velocity.y);
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
    }
}
