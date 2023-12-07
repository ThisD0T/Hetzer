using System.Collections;
using System.Collections.Generic;
using Scamper.Movement;
using UnityEngine;

public class StageResetTrigger : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponent<ScamperController>().moveData.resetPosition = true;
        }
    }
}
