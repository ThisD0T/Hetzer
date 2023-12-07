using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

struct ShootData {
    public bool wishesShoot;
    
}

public class CameraControl : MonoBehaviour
{
    public float xSensitivity;
    public float ySensitivity;

    //public Transform orientation;

    float xRotation, yRotation;

    float mouseX, mouseY;

    public float shotInterval;

    public GameObject orientation;

    public bool paused;

    public GameObject rocket;
    ShootData data;

    Timer shootTimer;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        shootTimer = new Timer(shotInterval);
    }

    void Update()
    {
        shootTimer.Tick();
        if (paused) {
            return;
        }

        // looking
        mouseX = Input.GetAxisRaw("Mouse X") * xSensitivity;
        mouseY = Input.GetAxisRaw("Mouse Y") * ySensitivity;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // shooting
        // I'm doing it like this mainly to conform to the standard set in ScamperController
        UpdateData();

        if (data.wishesShoot) {
            RocketParent rocketParent = rocket.GetComponent<RocketParent>();
            rocketParent.player = gameObject;
            Instantiate(rocket, transform.position, transform.rotation);
        }

        ResetData();
    }

    void UpdateData() {
        if (Input.GetButton("Fire1") && shootTimer.finished) {
            data.wishesShoot = true;
            shootTimer.Reset();
        }
    }

    void ResetData() {
        data.wishesShoot = false;
    }
}
