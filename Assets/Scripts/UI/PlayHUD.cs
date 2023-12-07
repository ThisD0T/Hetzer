using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scamper.Movement {
    public class PlayHUD : MonoBehaviour
    {
        public MenuSettings settingsData;

        public float tabButtonX, tabButtonStickOut;
        public Color activeTabColour, inactiveTabColour;

        // TODO: create a scriptable object so this works from scene to scene
        public GameObject playerCam, pauseMenu, 
            senseSlider,
            senseText;// for changing sensitivity

        public ScamperController pController;
        public CameraControl camControl;

        Slider RSlider, GSlider, BSlider, ASlider;
        RawImage currentCrosshair;

        LevelTracker levelTrack;

        public TMP_Text stageNumText, levelTimeText;

        // PARENTS
        public GameObject xHairTab, mainTab, xHairTabButton, mainTabButton;
        List<GameObject> tabs = new List<GameObject>(), tabButtons = new List<GameObject>();

        void Start() {
            settingsData.Load();
            // grabbing objects like this always seems really sketchy but I feel like it's the best option for me here
            // TODO: make a separate class for each tab in the pause menu to minimize this hell

            playerCam = GameObject.Find("Camera");
            pauseMenu = GameObject.Find("PauseMenu");

            senseSlider = GameObject.Find("SenseSlider");
            senseText = GameObject.Find("Sensitivity");

            pController = GameObject.Find("Player").GetComponent<ScamperController>();
            camControl = playerCam.GetComponent<CameraControl>();

            RSlider = GameObject.Find("RedSlider").GetComponent<Slider>();
            GSlider = GameObject.Find("GreenSlider").GetComponent<Slider>();
            BSlider = GameObject.Find("BlueSlider").GetComponent<Slider>();
            ASlider = GameObject.Find("AlphaSlider").GetComponent<Slider>();
            currentCrosshair = GameObject.Find("Crosshair").GetComponent<RawImage>();

            levelTrack = GetComponent<LevelTracker>();

            stageNumText = GameObject.Find("StageNumber").GetComponent<TMP_Text>();
            levelTimeText = GameObject.Find("LevelTime").GetComponent<TMP_Text>();

            xHairTab = GameObject.Find("XHairTab");
            mainTab = GameObject.Find("MainTab");
            tabs.Add(xHairTab);
            tabs.Add(mainTab);

            xHairTabButton = GameObject.Find("XHairTabButton");
            mainTabButton = GameObject.Find("MainTabButton");
            tabButtons.Add(xHairTabButton);
            tabButtons.Add(mainTabButton);

            pauseMenu.SetActive(false);

            //StartCoroutine(InitSliders());
            InitializeMainSliders();
            InitializeCrosshairSliders();

            SenseUpdate();
            CrosshairSliders();
            EnableMainTab();
            currentCrosshair.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
            currentCrosshair.gameObject.SetActive(true);
        }

        void Update() {

            stageNumText.text = "Stage: " + levelTrack.currentStageName;
            levelTimeText.text = "Time: " + levelTrack.levelTimer.ToString("0.00");

            // I'm using if statements in this weird way to minimize nesting which looks bad and is less readable IMO
            if (!Input.GetButtonDown("Cancel")) {
                return;
            }

            pauseMenu.SetActive(!pauseMenu.activeSelf);

            if (pauseMenu.activeSelf) {// player opened pause menu
                Cursor.visible = true;
                pController.paused = true;
                camControl.paused = true;
                Cursor.lockState = CursorLockMode.None;
                if (xHairTab.activeSelf) {
                    SetMenuCrosshair();
                    return;
                }

                currentCrosshair.gameObject.SetActive(false);
            }
            else {// player closed pause menu
                Cursor.visible = false;
                pController.paused = false;
                camControl.paused = false;
                Cursor.lockState = CursorLockMode.Locked;
                currentCrosshair.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
                currentCrosshair.gameObject.SetActive(true);
            }
        }

        IEnumerator InitSliders() {
            yield return 0;
            InitializeCrosshairSliders();
            InitializeMainSliders();
        }

        public void ExitToMainMenu() {
            settingsData.Save();
            SceneManager.LoadScene("Main Menu");
        }

        public void LevelFinishedRoutine() {
            var completionText = GameObject.Find("LevelComplete");
            completionText.GetComponent<Animation>().Play();
            completionText.GetComponent<AudioSource>().Play();
        }

        // --------------- BASE -----------------

        // used for wiping everything before you enable another tab in the menu
        // I have no idea if this is a good idea but it's the one I thought of
        void SetActiveTab(GameObject obj, GameObject button) {
            SetTabButtonActive(button);
            // I could make this better but the game won't scale that far
            foreach (GameObject tab in tabs) {
                tab.SetActive(false);
            }
            currentCrosshair.gameObject.SetActive(false);

            obj.SetActive(true);
        }

        void SetTabButtonActive(GameObject button) {
            foreach (GameObject tabButton in tabButtons) {
                Vector3 position = tabButton.GetComponent<RectTransform>().anchoredPosition3D;
                position.x = tabButtonX;
                tabButton.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = inactiveTabColour;

                if (tabButton == button) {
                    position.x = tabButtonX + tabButtonStickOut;
                    tabButton.gameObject.transform.GetChild(0).GetComponent<TMP_Text>().color = activeTabColour;
                }

                tabButton.GetComponent<RectTransform>().anchoredPosition3D = position;
            }
        }

        // ------------ Main Pause Menu ----------------
        // these initialize functions are here to set slider values to the actual values in settingsData
        // (instead of defaulting to whatever it's set to in the editor)
        public void InitializeMainSliders() {
            print("initialize main slider");
            senseSlider.GetComponent<Slider>().SetValueWithoutNotify(settingsData.sensitivity);
        }

        public void EnableMainTab() {
            SetActiveTab(mainTab, mainTabButton);
        }


        public void SenseUpdate() {
            print("sense update");
            settingsData.sensitivity = senseSlider.GetComponent<Slider>().value;
            // perfect, definitely not a cumbersome way to do this
            camControl.xSensitivity = settingsData.sensitivity;
            camControl.ySensitivity = settingsData.sensitivity;
            senseText.GetComponent<TMP_Text>().text = "Sensitivity: " + camControl.xSensitivity.ToString("0.##");
        }

        // ------------ Crosshair Customization ------------------
        public void InitializeCrosshairSliders()  {
            RSlider.value = settingsData.crosshairColour.r;
            GSlider.value = settingsData.crosshairColour.g;
            BSlider.value = settingsData.crosshairColour.b;
            ASlider.value = settingsData.crosshairColour.a;
        }

        public void EnableCrosshairTab() {
            SetActiveTab(xHairTab, xHairTabButton);
            SetMenuCrosshair();
        }
        
        public void SetMenuCrosshair() {
            currentCrosshair.gameObject.SetActive(true);
            currentCrosshair.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(170, 40);
        }

        public void CrosshairSliders() {
            settingsData.crosshairColour = new Color(RSlider.value, GSlider.value, BSlider.value, ASlider.value);
            Color newCrosshairColour = settingsData.crosshairColour;
            currentCrosshair.color = newCrosshairColour;
        }
    }
}
        // recursively deactivates all children of an object
        /* turns out I didn't need this but could be useful later
        void DeactivateAllChildren(GameObject obj) {
            if (obj == null) 
                return;

            foreach (Transform child in obj.transform) {
                if (child == null) continue;
                child.gameObject.SetActive(false);
                DeactivateAllChildren(child.gameObject);
            }
        }
        */