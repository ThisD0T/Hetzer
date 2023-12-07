using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTimer : MonoBehaviour
{
    // I don't think I need to explain this
    public float duration;
    Timer timer;
    void Start()
    {
        timer = new Timer(duration);
    }

    void Update()
    {
        timer.Tick();
        if (timer.finished) {
            Destroy(gameObject);
        }
    }
}
