using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player_Behavior : MonoBehaviour
{
    [SerializeField] public GameObject agnes;

    public GameObject player_mesh;
    public Slider life_slider;

    Animator player_animator;
    Player_Movement player_movement;

    public int life;
    RaycastHit hit;

    Agnes_AI agnes_ai;

    // Start is called before the first frame update
    void Start()
    {
        life = 100;
        life_slider.value = life;
        player_movement = GetComponent<Player_Movement>();
        player_animator = player_movement.player_animator;
        player_mesh = player_movement.player_mesh;
        agnes_ai = agnes.GetComponent<Agnes_AI>();
    }

    public void close_range_attack()
    {
        player_animator.SetTrigger("attacking");

        float distance = Vector3.Distance(player_mesh.transform.position, agnes.transform.position); //distance between player and agnes
        float angle = Vector3.Angle(player_mesh.transform.forward, agnes.transform.position - player_mesh.transform.position); //offset between the player and agnes angle-wise

        // attack if close enough and facing Agnes
        if (distance <= 1.6f && angle <= 40f) // we hit Agnes
        {
            agnes.GetComponent<Agnes_Behavior>().life -= 5;
            agnes.GetComponent<Agnes_Behavior>().life_slider.value = agnes.GetComponent<Agnes_Behavior>().life;
        }

        //update Agnes AI count variable
        agnes_ai.count_close_attacks++;
    }

    public void long_range_attack()
    {
        player_animator.SetTrigger("shooting");

        if (Physics.Raycast(new Vector3(player_mesh.transform.position.x, player_mesh.transform.position.y + 1.5f, player_mesh.transform.position.z), player_mesh.transform.forward, out hit, 100f))
        {
            if(hit.transform.name == "Agnes")
            {
                agnes.GetComponent<Agnes_Behavior>().life -= 5;
                agnes.GetComponent<Agnes_Behavior>().life_slider.value = agnes.GetComponent<Agnes_Behavior>().life;
            }
        }

        //update Agnes AI count variable
        agnes_ai.count_long_attacks++;
    }

    // Update is called once per frame
    void Update()
    {
        if (player_movement.inventory_input_freeze)
            return;

        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            close_range_attack();
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            long_range_attack();
        }
    }
}
