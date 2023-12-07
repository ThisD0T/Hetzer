using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StutterFix : MonoBehaviour
{
    void Awake() {
        if (Application.isEditor) {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 0;
        }
    }
}
