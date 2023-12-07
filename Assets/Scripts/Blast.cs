using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : MonoBehaviour
{
    public float killTime;
    void Start()
    {
        StartCoroutine(KillTimer());
    }

    IEnumerator KillTimer() {
        yield return new WaitForSeconds(killTime);
        Destroy(gameObject);
    }
}
