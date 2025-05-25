using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Agnes_Behavior : MonoBehaviour
{
    const int IDLE_STATE = 0;
    const int WALK_STATE = 1;

    public int life;
    public Slider life_slider;
    [SerializeField] GameObject player;
    Animator agnes_animator;
    Rigidbody agnes_rigidbody;
    Agnes_AI agnes_agent;

    bool is_dead;

    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        life = 100;
        life_slider.value = life;
        agnes_animator = GetComponent<Animator>();
        agnes_rigidbody = GetComponent<Rigidbody>();
        agnes_agent = GetComponent<Agnes_AI>();

        is_dead = false;
    }

    public void walk_towards_player() // needs to be called each frame when walking_to_player = true
    {
        if (Vector3.Distance(transform.position, player.transform.position) > 2.0f)
        {
            agnes_animator.SetInteger("state", WALK_STATE);
            // Move Agnes on player direction
            Vector3 move_direction = (player.transform.position - transform.position).normalized;
            move_direction.y = 0f;
            agnes_rigidbody.MovePosition(transform.position + 100.0f * Time.deltaTime * move_direction);
            //rotate it so its looking straight ahead at the player
            transform.LookAt(player.transform, Vector3.up);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);
        } else
        {
            agnes_animator.SetInteger("state", IDLE_STATE);
            agnes_agent.walking_to_player = false;
        }

        //for training
        //if (life <= 0 || player.GetComponentInParent<Player_Behavior>().life <= 0)
        //{
        //    agnes_agent.HandleEpisodeEnd();
        //}
    }

    public IEnumerator teleporting(bool closer_to_player)
    {
        agnes_agent.busy_teleporting = true;

        agnes_animator.SetTrigger("teleport");
        yield return new WaitForSeconds(2.3f);

        Vector3 move_direction;
        float distance;

        // direction of the teleportation
        move_direction = (player.transform.position - transform.position).normalized;
        move_direction.y = 0f;

        // distance for teleporting (90% of the distance between the player and Agnes for teleporting closer
        // or 10f units for teleporting further away)
        distance = 8f / 10f * Vector3.Distance(player.transform.position, transform.position);
        if (!closer_to_player)
        {
            move_direction = -move_direction;
            distance = 10f;
        }
        agnes_rigidbody.MovePosition(transform.position + distance * move_direction);

        agnes_agent.busy_teleporting = false;

        //for training
        //if (life <= 0 || player.GetComponentInParent<Player_Behavior>().life <= 0)
        //{
        //    agnes_agent.HandleEpisodeEnd();
        //}
    }

    public IEnumerator close_range_attack()
    {
        agnes_agent.busy_attacking = true;

        //rotate it so its looking straight ahead at the player
        transform.LookAt(player.transform, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        agnes_animator.SetTrigger("attacking");

        float distance = Vector3.Distance(player.transform.position, transform.position); //distance between player and agnes
        float angle = Vector3.Angle(transform.forward, player.transform.position - transform.position); //offset between the player and agnes angle-wise

        // attack if close enough and facing the player
        if (distance <= 2f && angle <= 60f) // we hit player
        {
            player.GetComponentInParent<Player_Behavior>().life -= 20;
            player.GetComponentInParent<Player_Behavior>().life_slider.value = player.GetComponentInParent<Player_Behavior>().life;
        }

        yield return new WaitForSeconds(1f); // wait a second after attacking before continuing
        agnes_agent.busy_attacking = false;

        //for training
        //if (life <= 0 || player.GetComponentInParent<Player_Behavior>().life <= 0)
        //{
        //    agnes_agent.HandleEpisodeEnd();
        //}
    }

    public IEnumerator long_range_attack()
    {
        agnes_agent.busy_attacking = true;

        //rotate it so its looking straight ahead at the player
        transform.LookAt(player.transform, Vector3.up);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, transform.eulerAngles.z);

        agnes_animator.SetTrigger("spelling");

        yield return new WaitForSeconds(1.755f);

        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.forward, out hit, 100f))
        {
            if(hit.transform.name == "Player") //player hit by the projectile
            {
                player.GetComponentInParent<Player_Behavior>().life -= 20;
                player.GetComponentInParent<Player_Behavior>().life_slider.value = player.GetComponentInParent<Player_Behavior>().life;
            }
        }

        yield return new WaitForSeconds(1f); // wait a second after attacking before continuing
        agnes_agent.busy_attacking = false;

        //for training
        //if (life <= 0 || player.GetComponentInParent<Player_Behavior>().life <= 0)
        //{
        //    agnes_agent.HandleEpisodeEnd();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if(life <= 0 && !is_dead)
        {
            is_dead = true;
            agnes_animator.SetTrigger("death");
            //agnes_agent.HandleEpisodeEnd();
        }


        //for ingame
        if (agnes_agent.walking_to_player)
        {
            walk_towards_player();
        }
    }
}
