using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    Dictionary<string, GameObject> inventory_elements = new Dictionary<string, GameObject>();
    public Dictionary<string, int> inventory_qt = new Dictionary<string, int>();

    [SerializeField] GameObject inventory_window, inventory_content;
    [SerializeField] GameObject inv_vial, inv_bullet;

    [SerializeField] GameObject player;
    Player_Movement player_movement;

    bool update_qts, add_vial_elem, add_bullet_elem, vial_first_time, bullet_first_time;

    // Start is called before the first frame update
    void Start()
    {
        inventory_qt.Add("vials", 0);
        inventory_qt.Add("bullets", 0);

        update_qts = false;
        add_vial_elem = false;
        add_bullet_elem = false;
        vial_first_time = true;
        bullet_first_time = true;

        player_movement = player.GetComponent<Player_Movement>();
    }

    void UpdateInventory()
    {
        GameObject element;

        if (add_vial_elem)
        {
            GameObject aux = Instantiate(inv_vial);
            inventory_elements.Add("vials", aux);
            aux.transform.SetParent(inventory_content.transform, false);

            add_vial_elem= false;
        }

        if (add_bullet_elem)
        {
            GameObject aux = Instantiate(inv_bullet);
            inventory_elements.Add("bullets", aux);
            aux.transform.SetParent(inventory_content.transform, false);
            aux.transform.position = new Vector3(aux.transform.position.x, aux.transform.position.y - 270, aux.transform.position.z);

            add_bullet_elem = false;
        }

        if (inventory_elements.ContainsKey("vials"))
        {
            element = GameObject.Find("UI_Elements/Canvas/Inventory/Viewport/Content/Inv_vial(Clone)/Quantity");
            element.GetComponent<TextMeshProUGUI>().text = "Qt : " + inventory_qt["vials"];
        }

        if (inventory_elements.ContainsKey("bullets"))
        {
            element = GameObject.Find("UI_Elements/Canvas/Inventory/Viewport/Content/Inv_bullet(Clone)/Quantity");
            element.GetComponent<TextMeshProUGUI>().text = "Qt : " + inventory_qt["bullets"];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory_window.activeSelf)
            {
                inventory_window.SetActive(false);
                player_movement.inventory_input_freeze = false;
            } else
            {
                inventory_window.SetActive(true);
                player_movement.inventory_input_freeze = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventory_window.activeSelf)
            {
                inventory_window.SetActive(false);
                player_movement.inventory_input_freeze = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            inventory_qt["vials"]++;
            update_qts = true;

            if (vial_first_time)
            {
                add_vial_elem = true;
                vial_first_time = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory_qt["bullets"]++;
            update_qts = true;

            if (bullet_first_time)
            {
                add_bullet_elem = true;
                bullet_first_time = false;
            }
        }

        if (update_qts)
        {
            update_qts = false;
            UpdateInventory();
        }
    }
}
