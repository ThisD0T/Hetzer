using System.Collections;
using System.Collections.Generic;
using Scamper.Movement;
using UnityEngine;
using UnityEngine.Assertions;

// this script is responsible for tracking a players time and current stage through a level. 
// other scripts will grab certain things from it like whether the level is complete or not
public class LevelTracker : MonoBehaviour
{
    public float clearTime, levelTimer, stageTimer;

    public GameObject[] stages;
    public int currentStage;
    public string currentStageName;
    public GameObject player;

    public bool levelFinished = false;

    void Update() {
        if (!levelFinished) levelTimer += Time.deltaTime;
        stageTimer += Time.deltaTime;
    }

    public void StageChange(GameObject newStage) {
        for(int i = 0; i < stages.Length; i++) {
            print(i);
            if (stages[i] == newStage) {
                currentStageName = stages[i].GetComponent<Stage>().stageName;
                player.GetComponent<ScamperController>().spawnPoint = stages[i].transform.GetChild(0).transform;
                currentStage = i;
                // making a function  for this because I don't want more nesting
                levelWinCheck();
            }
        }
    }

    public void levelWinCheck() {
        if (currentStage + 1 == stages.Length && !levelFinished) {
            levelFinished = true;
            GameObject.Find("HUD").GetComponent<PlayHUD>().LevelFinishedRoutine();
        }
    }

    public void ResetLevel() {
        StageChange(stages[0]);
        player.GetComponent<ScamperController>().moveData.resetPosition = true;
        levelTimer = 0;
        levelFinished = false;
    }
}