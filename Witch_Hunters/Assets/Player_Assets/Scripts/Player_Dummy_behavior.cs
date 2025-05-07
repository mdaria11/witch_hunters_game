using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Dummy_behavior : MonoBehaviour
{
    Player_Behavior player_behavior;
    bool close_attacking, long_attacking;
    bool walking;
    Vector3 initial_position;
    bool first_walking_save;

    // Start is called before the first frame update
    void Start()
    {
        player_behavior = GetComponent<Player_Behavior>();
        close_attacking = false;
        long_attacking = false;
        walking = false;
        first_walking_save = true;
    }

    void walk_to_agnes()
    {
        if (Vector3.Distance(player_behavior.player_mesh.transform.position, player_behavior.agnes.transform.position) > 1.6f)
        {
            // Move Agnes on player direction
            Vector3 move_direction = (player_behavior.agnes.transform.position - player_behavior.player_mesh.transform.position).normalized;
            move_direction.y = 0f;
            player_behavior.player_mesh.transform.position = player_behavior.player_mesh.transform.position + 10f * Time.deltaTime * move_direction;
            //rotate it so its looking straight ahead at the player
            player_behavior.player_mesh.transform.LookAt(player_behavior.agnes.transform, Vector3.up);
            player_behavior.player_mesh.transform.eulerAngles = new Vector3(0, player_behavior.player_mesh.transform.eulerAngles.y, player_behavior.player_mesh.transform.eulerAngles.z);
        }
        else
        {
            walking = false;
        }
    }

    void walk_away()
    {
        if (Vector3.Distance(player_behavior.player_mesh.transform.position, initial_position) <= 12f)
        {
            // Move on random direction
            Vector3 move_direction = new Vector3(Random.Range(0f, 1f), 0, Random.Range(0f, 1f));
            player_behavior.player_mesh.transform.position = player_behavior.player_mesh.transform.position + 10f * Time.deltaTime * move_direction;
        }
        else
        {
            walking = false;
            first_walking_save = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!close_attacking && !long_attacking)
        {
            float type_of_attack = Random.Range(0f, 1f);

            if(Vector3.Distance(player_behavior.player_mesh.transform.position, player_behavior.agnes.transform.position) >= 6f)
            {
                if(type_of_attack <= 0.5f)
                {
                    //long range attack
                    walking = true;
                    long_attacking = true;
                } else
                {
                    //close range attack
                    walking = true;
                    close_attacking = true;
                }
            } else
            {
                if (type_of_attack <= 0.2f)
                {
                    //long range attack
                    walking = true;
                    long_attacking = true;
                }
                else
                {
                    //close range attack
                    walking = true;
                    close_attacking = true;
                }
            }
        }

        if(close_attacking && !walking)
        {
            player_behavior.close_range_attack();
            close_attacking=false;
        }

        if (long_attacking && !walking)
        {
            player_behavior.player_mesh.transform.LookAt(player_behavior.agnes.transform, Vector3.up);
            player_behavior.player_mesh.transform.eulerAngles = new Vector3(0, player_behavior.player_mesh.transform.eulerAngles.y, player_behavior.player_mesh.transform.eulerAngles.z);
            player_behavior.long_range_attack();
            long_attacking = false;
        }

        if (walking && close_attacking)
        {
            walk_to_agnes();
        }

        if(walking && long_attacking)
        {
            if(first_walking_save)
            {
                initial_position = player_behavior.player_mesh.transform.position;
                first_walking_save = false;
            }
            walk_away();
        }
    }
}
