using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting.Antlr3.Runtime;

public class Agnes_AI : Agent
{
    public GameObject player;
    Agnes_Behavior boss_behaviors;

    private Vector3 startPosition;
    private Vector3 player_startPosition;

    public bool busy_teleporting;
    public bool busy_attacking;
    public bool walking_to_player;

    public int count_close_attacks;
    public int count_long_attacks;

    bool close_strategy_chosen;
    bool long_strategy_chosen;

    int last_player_life;
    int last_boss_life;
    int current_player_life;
    int current_boss_life;

    bool start_agent;

    int how_many_times_teleporting;
    int idle_actions_taken;

    public override void Initialize() // save the start position so we can initialize the position from the beginning for each episode
    {
        player_startPosition = player.transform.position;
        startPosition = transform.position;
        boss_behaviors = GetComponent<Agnes_Behavior>();

        busy_teleporting = false;
        busy_attacking = false;
        walking_to_player = false;

        start_agent = false;

        count_close_attacks = 0;
        count_long_attacks = 0;
        close_strategy_chosen = false;
        long_strategy_chosen = false;

        how_many_times_teleporting = 0;
        idle_actions_taken = 0;
    }

    public override void OnEpisodeBegin() // Agnes back in place and the life is full for both Agnes and the player
    {
        player.transform.position = player_startPosition;
        transform.position = startPosition;
        GetComponent<Animator>().SetInteger("state", 0);
        boss_behaviors.life = 100;
        player.GetComponentInParent<Player_Behavior>().life = 100;
        boss_behaviors.life_slider.value = 100;
        player.GetComponentInParent<Player_Behavior>().life_slider.value = 100;

        last_player_life = 100;
        last_boss_life = 100;

        busy_teleporting = false;
        busy_attacking = false;
        walking_to_player = false;

        count_close_attacks = 0;
        count_long_attacks = 0;
        close_strategy_chosen = false;
        long_strategy_chosen = false;

        //for ingame
        start_agent = false;
        StartCoroutine(Delay_start());

        //for training
        //start_agent = true;

        how_many_times_teleporting = 0;
        idle_actions_taken = 0;
    }

    IEnumerator Delay_start()
    {
        yield return new WaitForSeconds(6);
        start_agent = true;
    }

    public void HandleEpisodeEnd()
    {
        if (boss_behaviors.life <= 0)
        {
            AddReward(-1f);
            EndEpisode();
        }
        else if (player.GetComponentInParent<Player_Behavior>().life <= 0)
        {
            AddReward(1f);

            if(boss_behaviors.life >= 50) //boss defeated player with more than his life left
            {
                AddReward(0.5f);
            }

            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 4 floats for observations

        sensor.AddObservation(close_strategy_chosen);
        sensor.AddObservation(long_strategy_chosen);
        sensor.AddObservation(boss_behaviors.life / 100f); // Boss HP (normalized)
        sensor.AddObservation(player.GetComponentInParent<Player_Behavior>().life / 100f); // Player HP (normalized)
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (!start_agent || boss_behaviors.life <= 0)
        {
            return;
        }

        // One discrete action with 5 branches:
        // 0:walk, 1:teleport_closer, 2:teleport_further, 3:close_attack, 4:long_ranged_attack

        if (count_close_attacks >= count_long_attacks * 3)
        {
            close_strategy_chosen = true;
            long_strategy_chosen = false;
        }

        if (count_long_attacks >= count_close_attacks * 3)
        {
            long_strategy_chosen = true;
            close_strategy_chosen = false;
        }

        if (walking_to_player)
        {
            //for training
            //boss_behaviors.walk_towards_player();
            //current_boss_life = boss_behaviors.life;

            //if (current_boss_life < last_boss_life)
            //{
            //    AddReward(-0.1f);
            //}

            //HandleEpisodeEnd();

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
            //    AddReward(-0.05f);
            //}

            //HandleEpisodeEnd();

            ////update the lives for the next action
            //last_boss_life = current_boss_life;

            return;
        }

        if(busy_attacking) //the boss is attacking, check for life
        {
            //for training
            //current_boss_life = boss_behaviors.life;
            //current_player_life = player.GetComponentInParent<Player_Behavior>().life;

            //if (current_boss_life < last_boss_life) // losing life while attacking
            //{
            //    AddReward(-0.05f);
            //}

            //if (current_player_life < last_player_life) // the hit was successful 
            //{
            //    AddReward(0.5f);
            //}

            //HandleEpisodeEnd();

            ////update the lives for the next action
            //last_player_life = current_player_life;
            //last_boss_life = current_boss_life;

            return;
        }

        int action = actionBuffers.DiscreteActions[0];

        //Debug.Log("Action: "+ action);

        switch (action)
        {
            case 0: // WALK
                //idle_actions_taken++;
                //if (idle_actions_taken > 3)
                //{
                //    AddReward(-0.05f);
                //}

                //if (Vector3.Distance(transform.position, player.transform.position) <= 2f) //penalize if walking when already close to the player
                //{
                //    AddReward(-0.3f);
                //}

                //if (long_strategy_chosen)
                //{
                //    AddReward(0.1f);
                //}

                //if (close_strategy_chosen)
                //{
                //    AddReward(-0.2f);
                //}

                //how_many_times_teleporting = 0;

                walking_to_player = true;

                break;
            case 1: // TELEPORT_CLOSER
                //idle_actions_taken++;
                //if (idle_actions_taken > 3)
                //{
                //    AddReward(-0.05f);
                //}

                //how_many_times_teleporting++;
                //if (how_many_times_teleporting > 3)
                //{
                //    AddReward(-0.2f);
                //}
                StartCoroutine(boss_behaviors.teleporting(true));

                //if (long_strategy_chosen)
                //{
                //    AddReward(0.1f);
                //}

                //if (close_strategy_chosen)
                //{
                //    AddReward(-0.2f);
                //}

                break;
            case 2: // TELEPORT_FURTHER
                //idle_actions_taken++;
                //if (idle_actions_taken > 3)
                //{
                //    AddReward(-0.05f);
                //}

                //how_many_times_teleporting++;
                //if (how_many_times_teleporting > 3)
                //{
                //    AddReward(-0.2f);
                //}
                StartCoroutine(boss_behaviors.teleporting(false));

                //if (long_strategy_chosen)
                //{
                //    AddReward(-0.2f);
                //}

                //if (close_strategy_chosen)
                //{
                //    AddReward(0.1f);
                //}

                break;
            case 3: // CLOSE_ATTACK
                //idle_actions_taken = 0;
                //how_many_times_teleporting = 0;

                //if (long_strategy_chosen)
                //{
                //    AddReward(0.1f);
                //}

                //if (close_strategy_chosen)
                //{
                //    AddReward(-0.2f);
                //}
                //if (Vector3.Distance(transform.position, player.transform.position) > 2f) //penalize if attacking when not close to the player
                //{
                //    AddReward(-0.2f);
                //} else
                //{
                //    AddReward(0.3f);
                //}

                StartCoroutine(boss_behaviors.close_range_attack());
                //AddReward(0.05f); //encourage attacking
                break;
            case 4: // LONGRANGEDATTACK
                //idle_actions_taken = 0;
                //how_many_times_teleporting = 0;

                //if (long_strategy_chosen)
                //{
                //    AddReward(-0.2f);
                //}

                //if (close_strategy_chosen)
                //{
                //    AddReward(0.1f);
                //}
                //if (Vector3.Distance(transform.position, player.transform.position) < 7f) //penalize if attacking when not at a distance to the player
                //{
                //    AddReward(-0.2f);
                //}
                //else
                //{
                //    AddReward(0.3f);
                //}

                StartCoroutine(boss_behaviors.long_range_attack());
                //AddReward(0.05f); //encourage attacking
                break;
        }

        //for training
        //HandleEpisodeEnd();

        ////update the lives for the next action
        //last_player_life = player.GetComponentInParent<Player_Behavior>().life;
        //last_boss_life = boss_behaviors.life;
    }
}
