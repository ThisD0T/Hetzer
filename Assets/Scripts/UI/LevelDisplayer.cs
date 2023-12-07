using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// NOTE: in the future to support many levels, there could be a function to shuffle this object to the left and right to show more levels.
public class LevelDisplayer : MonoBehaviour
{
    // this scripts job is to grab all the scenes and procedurally make a button for each of them on the main menu
    // It's sort of similar to the way LevelTracker does it (Philosophically; easily expandable, but not flexible in format)
    public GameObject levelButton;
    public float YOffset, XOffset,
        buttonSeparationX, buttonSeparationY;
    public int columns;

    void Start()
    {
        int numScenes = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < numScenes; i++) {
            // I have to do it this way because GetSceneByBuildIndex doesn't work unless the scene is loaded. WHY?!
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            Scene scene = SceneManager.GetSceneByPath(scenePath);
            string sceneName = GetSceneNameFromPath(scenePath);

            // this math creates a 2D grid from a 1D array based on a few variables
            float x = (i % columns * buttonSeparationX) + XOffset;
            float y = Mathf.FloorToInt(i / columns) * -buttonSeparationY;

            levelButton.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(x, y, 0);
            levelButton.transform.GetChild(0).GetComponent<TMP_Text>().text = sceneName;// get the TMP_Text child
            
            GameObject newButton = Instantiate(levelButton, transform);

            // I have no idea why but I have to put this here. Unity's API is making me go insane.
            newButton.GetComponent<Button>().onClick.AddListener(() => SceneManager.LoadScene(sceneName));// I'm not actually sure why the syntax must be this way
        }
        Toggle(0);
    }

    // toggles setting all the level buttons active, for animation (I didn't want to do this with an animation)
    public void Toggle(float time) {
        if (time != 0)
            StartCoroutine(ToggleDelay(time));
        else
            foreach (Transform child in transform) {
                child.gameObject.SetActive(!child.gameObject.activeSelf);
            }
    }

    IEnumerator ToggleDelay(float time) {
        yield return new WaitForSeconds(time);
        foreach (Transform child in transform) 
            child.gameObject.SetActive(!child.gameObject.activeSelf);
    }

    // I have to do it this way because Unity's Scene API is literal garbage
    string GetSceneNameFromPath(string scenePath) {
        string sceneName = "";

        // - 7 to account for the .unity at the end of the file name
        for (int i = scenePath.Length - 7; i > 1; i--) {
            if (scenePath[i] == '/')
                break;
            sceneName = scenePath[i] + sceneName;
        };

        return sceneName;
    }
}
