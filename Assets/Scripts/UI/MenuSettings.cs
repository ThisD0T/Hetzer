using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using UnityEngine;

[CreateAssetMenu(fileName = "MenuData", menuName = "ScriptableObjects/MenuData", order = 1)]
public class MenuSettings : ScriptableObject
{
    public Color crosshairColour;
    public float sensitivity;

    private const string FILENAME = "MenuSettings.json";

    void Awake() {
        Save();
    }

    public void Save() {
        var filePath = Path.Combine(Application.persistentDataPath, FILENAME);

        if (!File.Exists(filePath)) {
            File.Create(filePath);
        }

        var json = JsonUtility.ToJson(this);
        Debug.Log("MenuData save: " + json);
        File.WriteAllText(filePath, json);
    }

    public void Load() {
        var filePath = Path.Combine(Application.persistentDataPath, FILENAME);

        if (!File.Exists(filePath)) {
            Debug.LogError($"File \"{filePath}\" does not exist.", this);
        }

        var json = File.ReadAllText(filePath);
        Debug.Log("MenuData load: " + json);
        JsonUtility.FromJsonOverwrite(json, this);
    }
}
