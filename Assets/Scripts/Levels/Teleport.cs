using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.transform.position = transform.GetChild(0).position;
        }
    }
}
