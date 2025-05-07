using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Agnes_AI : Agent
{
    public GameObject player;
    Agnes_Behavior boss_behaviors;

    private Vector3 startPosition;
    private Vector3 player_startPosition;

    public bool busy_teleporting;
    public bool busy_attacking;
    public bool walking_to_player;

    //int last_player_life;
    //int last_boss_life;
    //int current_player_life;
    //int current_boss_life;

    bool start_agent;

    public override void Initialize() // save the start position so we can initialize the position from the beginning for each episode
    {
        player_startPosition = player.transform.position;
        startPosition = transform.position;
        boss_behaviors = GetComponent<Agnes_Behavior>();

        busy_teleporting = false;
        busy_attacking = false;
        walking_to_player = false;

        start_agent = false;
    }

    public override void OnEpisodeBegin() // Agnes back in place and the life is full for both Agnes and the player
    {
        player.transform.position = player_startPosition;
        transform.position = startPosition;
        boss_behaviors.life = 100;
        player.GetComponentInParent<Player_Behavior>().life = 100;
        boss_behaviors.life_slider.value = 100;
        player.GetComponentInParent<Player_Behavior>().life_slider.value = 100;

        //last_player_life = 100;
        //last_boss_life = 100;

        busy_teleporting = false;
        busy_attacking = false;
        walking_to_player = false;

        //for ingame
        start_agent = false;
        StartCoroutine(Delay_start());

        //for training
        //start_agent = true;
    }

    IEnumerator Delay_start()
    {
        yield return new WaitForSeconds(6);
        start_agent = true;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //sensor.AddObservation(Vector3.Distance(transform.position, player.transform.position)); // Distance to player
        sensor.AddObservation(boss_behaviors.life / 100f); // Boss HP (normalized)
        sensor.AddObservation(player.GetComponentInParent<Player_Behavior>().life / 100f); // Player HP (normalized)
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if(!start_agent || boss_behaviors.life <= 0)
        {
            return;
        }

        // One discrete action with 5 branches:
        // 0:walk, 1:teleport_closer, 2:teleport_further, 3:close_attack, 4:long_ranged_attack

        if(walking_to_player)
        {
            //for training
            //boss_behaviors.walk_towards_player();
            //current_boss_life = boss_behaviors.life;

            //if (current_boss_life < last_boss_life)
            //{
            //    AddReward(-0.1f);
            //}

            //if (boss_behaviors.life <= 0)
            //{
            //    AddReward(-1.0f);
            //    EndEpisode(); // Boss "dies"
            //}
            //if (player.GetComponentInParent<Player_Behavior>().life <= 0)
            //{
            //    AddReward(1.0f); // Boss wins
            //    EndEpisode();
            //}

            ////update the lives for the next action
            //last_boss_life = current_boss_life;

            return;
        }

        if(busy_teleporting) // the boss is doing a coroutine method; check the life of boss and player
        {
            //for training
            //current_boss_life = boss_behaviors.life;

            //if (current_boss_life < last_boss_life)
            //{
            //    AddReward(-0.1f);
            //}

            //if (boss_behaviors.life <= 0)
            //{
            //    AddReward(-1.0f);
            //    EndEpisode(); // Boss "dies"
            //}
            //if (player.GetComponentInParent<Player_Behavior>().life <= 0)
            //{
            //    AddReward(1.0f); // Boss wins
            //    EndEpisode();
            //}

            ////update the lives for the next action
            //last_boss_life = current_boss_life;

            return;
        }

        if(busy_attacking) //the boss is attacking, check for life
        {
            //for training
            //current_boss_life = boss_behaviors.life;
            //current_player_life = player.GetComponentInParent<Player_Behavior>().life;

            //if (current_boss_life < last_boss_life)
            //{
            //    AddReward(-0.2f);
            //}

            //if (current_player_life < last_player_life)
            //{
            //    AddReward(0.5f);
            //}

            //if (boss_behaviors.life <= 0)
            //{
            //    AddReward(-1.0f);
            //    EndEpisode(); // Boss "dies"
            //}
            //if (player.GetComponentInParent<Player_Behavior>().life <= 0)
            //{
            //    AddReward(1.0f); // Boss wins
            //    EndEpisode();
            //}

            ////update the lives for the next action
            //last_player_life = current_player_life;
            //last_boss_life = current_boss_life;

            return;
        }

        int action = actionBuffers.DiscreteActions[0];

        switch (action)
        {
            case 0: // WALK
                walking_to_player = true;
                break;
            case 1: // TELEPORT_CLOSER
                StartCoroutine(boss_behaviors.teleporting(true));
                break;
            case 2: // TELEPORT_FURTHER
                StartCoroutine(boss_behaviors.teleporting(false));
                break;
            case 3: // CLOSE_ATTACK
                StartCoroutine(boss_behaviors.close_range_attack());
                break;
            case 4: // LONGRANGEDATTACK
                StartCoroutine(boss_behaviors.long_range_attack());
                break;
        }

        //for training
        //if (boss_behaviors.life <= 0)
        //{
        //    AddReward(-1.0f);
        //    EndEpisode(); // Boss "dies"
        //}
        //if (player.GetComponentInParent<Player_Behavior>().life <= 0)
        //{
        //    AddReward(1.0f); // Boss wins
        //    EndEpisode();
        //}

        ////update the lives for the next action
        //last_player_life = player.GetComponentInParent<Player_Behavior>().life;
        //last_boss_life = boss_behaviors.life;
    }
}
