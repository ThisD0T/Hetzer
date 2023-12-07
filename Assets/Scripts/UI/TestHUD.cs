using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestHUD : MonoBehaviour
{
    public GameObject player;
    public TMP_Text velocityText, FPS_Text;

    private float currentVel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateData();
        velocityText.SetText("Velocity: " + currentVel.ToString("0.00"));
        FPS_Text.SetText("FPS: " + 1/Time.deltaTime);
    }

    void UpdateData() {
        currentVel = player.GetComponent<Rigidbody>().velocity.magnitude;
    }  
}
