using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HelpTotem : MonoBehaviour
{
    public string text;
    public TMP_FontAsset font;
    public float fontSize, radius;

    public GameObject textDisplay, HUD, player;
    TextMeshProUGUI tmp;

    bool hitPlayer;
    
    void Start() {
        HUD = GameObject.Find("HUD");
        player = GameObject.Find("Player");

        SetupText();
    }

    void SetupText() {
        textDisplay = Instantiate(textDisplay, HUD.transform);
        tmp = textDisplay.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.font = font;
        tmp.horizontalAlignment = HorizontalAlignmentOptions.Center;
        tmp.margin = new Vector4(-360, 0, -360, 0);// just going to standardize the position of the help text's for now
        textDisplay.SetActive(false);
    }

    void FixedUpdate() {
        // I'm doing it like this instead of using hitboxes because hitboxes in Unity are extremely annoying
        // and I don't really need them for this
        float dist = Vector3.Distance(transform.position, player.transform.position);
        if (dist < radius && !hitPlayer) {
            hitPlayer = true;
            Activate();
        }

        if (dist > radius && hitPlayer) {
            hitPlayer = false;
            Deactivate();
        }
    }

    void Activate() {
        textDisplay.SetActive(true);
    }

    void Deactivate() {
        textDisplay.SetActive(false);
        
    }
}
