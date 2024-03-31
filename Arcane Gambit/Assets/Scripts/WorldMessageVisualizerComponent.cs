using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMessageVisualizerComponent : MonoBehaviour
{
    [Header("UI References")]
    public GameObject anchor_point;
    public GameObject messages_parent;
    public Text message_text;
    [Header("Visuals")]
    public float message_display_time = 3f;

    // References
    private Camera main_camera;
    private float display_timer;

    void Start()
    {
        display_timer = 0;
        main_camera = Camera.main;
    }

    // Displays message for a period of time.
    public void DisplayMessage(string message)
    {
        if (message_text != null)
        {
            message_text.text = message;
        }
        display_timer = message_display_time;
    }

    void Update()
    {
        // Billboarding effect
        if (main_camera != null && anchor_point != null)
        {
            Vector3 cam_pos = main_camera.transform.position;
            Vector3 look_rot = (anchor_point.transform.position - cam_pos);
            anchor_point.transform.rotation = Quaternion.LookRotation(look_rot);
        }
        // Message display
        if (message_text != null)
        {
            messages_parent.SetActive(display_timer > 0);
            if (display_timer > 0)
            {
                display_timer -= Time.deltaTime;
            }
        }
    }
}
