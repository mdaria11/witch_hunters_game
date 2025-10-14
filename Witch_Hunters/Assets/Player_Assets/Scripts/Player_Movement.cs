using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    const int IDLE_STATE = 0;
    const int WALK_STATE = 1;
    const int RUN_STATE = 2;

    Vector3 moveDirection;
    float input_mouseX;
    float input_mouseY;
    float camera_speed = 600f;
    Vector3 camera_player_vector;

    Vector3 player_centerpoint;

    float walk_speed;

    bool is_running;
    bool is_dead;

    [SerializeField] public GameObject player_mesh;
    public Animator player_animator;
    public Rigidbody player_rigidbody;

    void Awake()
    {
        player_animator = player_mesh.GetComponent<Animator>();
        player_rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        walk_speed = 4.0f;
        moveDirection = new Vector3(0, 0, 0.1f);
        player_centerpoint = new Vector3(player_mesh.transform.position.x, player_mesh.transform.position.y + 1.13f, player_mesh.transform.position.z);
        camera_player_vector = Vector3.Normalize(player_centerpoint - Camera.main.transform.position);

        is_running = false;
        is_dead = false;
    }

    private void FixedUpdate()
    {
        //WASD MOVEMENT

        if (!is_dead)
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z);
                if (is_running)
                {
                    walk_speed = 8.0f;
                }
                else
                {
                    walk_speed = 4.0f;
                }
            }
            else if (Input.GetKey(KeyCode.S))
            {
                moveDirection = new Vector3(-1 * Camera.main.transform.forward.x, 0, -1 * Camera.main.transform.forward.z);
                if (is_running)
                {
                    walk_speed = 8.0f;
                }
                else
                {
                    walk_speed = 4.0f;
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                moveDirection = new Vector3(-1 * Camera.main.transform.right.x, 0, -1 * Camera.main.transform.right.z);
                if (is_running)
                {
                    walk_speed = 8.0f;
                }
                else
                {
                    walk_speed = 4.0f;
                }
            }
            else if (Input.GetKey(KeyCode.D))
            {
                moveDirection = new Vector3(Camera.main.transform.right.x, 0, Camera.main.transform.right.z);
                if (is_running)
                {
                    walk_speed = 8.0f;
                }
                else
                {
                    walk_speed = 4.0f;
                }
            }
            else
            {
                walk_speed = 0.0f;
            }
        }
        else
        {
            walk_speed = 0.0f;
        }

        Quaternion finalRotation = Quaternion.LookRotation(moveDirection);
        player_mesh.transform.rotation = Quaternion.RotateTowards(player_mesh.transform.rotation, finalRotation, 1000f * Time.fixedDeltaTime);

        player_rigidbody.MovePosition(transform.position + walk_speed * Time.fixedDeltaTime * moveDirection);

        if (walk_speed == 0.0f)
        {
            player_animator.SetInteger("state", IDLE_STATE);
        }
        else if (is_running)
        {
            player_animator.SetInteger("state", RUN_STATE);
        }
        else
        {
            player_animator.SetInteger("state", WALK_STATE);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // get mouse input
        input_mouseX = Input.GetAxis("Mouse X");
        input_mouseY = Input.GetAxis("Mouse Y");

        ////////// CAMERA ROTATION AND ZOOM /////////////////////////
        player_centerpoint = new Vector3(player_mesh.transform.position.x, player_mesh.transform.position.y + 1.13f, player_mesh.transform.position.z);
        if (walk_speed == 0.0f)
        {
            Camera.main.transform.RotateAround(player_centerpoint, player_mesh.transform.up, input_mouseX * camera_speed * Time.deltaTime);
        }
        else
        {
            Quaternion bodyRotation = Quaternion.Euler(0f, input_mouseX * camera_speed * Time.deltaTime, 0f);
            player_rigidbody.MoveRotation(player_rigidbody.rotation * bodyRotation);
        }
        //rotate just the camera on Y axis
        float camera_x_rotation = (-1) * input_mouseY * camera_speed * Time.deltaTime;
        Camera.main.transform.RotateAround(player_centerpoint, Camera.main.transform.right, camera_x_rotation);

        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            float camera_player_distance = Vector3.Distance(player_centerpoint, Camera.main.transform.position);
            if ((camera_player_distance >= 1.5f && Input.GetAxis("Mouse ScrollWheel") > 0f) || (camera_player_distance <= 5f && Input.GetAxis("Mouse ScrollWheel") < 0f))
            {
                camera_player_vector = Vector3.Normalize(player_centerpoint - Camera.main.transform.position);
                Camera.main.transform.position += camera_player_vector * Input.GetAxis("Mouse ScrollWheel") * (camera_speed * 2.0f) * Time.deltaTime;
            }
        }

        ////////// ACTIONS //////////////////////////////////////////

        //Running
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            is_running = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            is_running = false;
        }
    }
}
