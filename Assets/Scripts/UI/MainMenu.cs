using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Animator MainMenuAnim;

    void Update() {
        if (Input.GetButtonUp("Cancel")) { 
            GetComponent<AudioSource>().Play();
            MainMenuAnim.Play("StartMenuInvert");
            GameObject.Find("LevelsButtonParent").GetComponent<LevelDisplayer>().Toggle(0);
            print("cancelled");
        }

    }

    void Start() {
        Time.timeScale = 1;
        Intro();
    }

    void Intro() {
        MainMenuAnim.Play("MainMenuIntro");
        print("played intro animation?");
    }

    public void StartMenu() {
        MainMenuAnim.Play("StartMenu");
    }

    public void ExitStart() {
        MainMenuAnim.Play("StartMenuInverted");
    }

    public void Quit() {
        Application.Quit();
    }
}
