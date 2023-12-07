using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTrail : MonoBehaviour
{
    public GameObject rocketParent;

    void Start()
    {
        transform.rotation = rocketParent.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (rocketParent == null) { 
            GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(gameObject, 2.5f);
            return;
        }
        transform.position = rocketParent.transform.position;
    }

    IEnumerator KillTimer() {
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
