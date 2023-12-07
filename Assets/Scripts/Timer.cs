using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Timer
{
    public float duration;
    public bool finished = false;
    public float timePassed;

    public Timer(float time) {
        duration = time;
    }

    public void Tick() {
        if (timePassed > duration) {
            finished = true;
            return;
        }
        timePassed += Time.deltaTime;
    }

    public void Reset() {
        timePassed = 0;
        finished = false;
    }
}
