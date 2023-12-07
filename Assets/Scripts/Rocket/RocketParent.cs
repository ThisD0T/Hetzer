using System.Collections;
using System.Collections.Generic;
using Scamper.Movement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class RocketParent : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float blastRadius;
    public float magnitude;
    public float destroyDist = 4;
    private Vector3 moveVec;

    bool exploded = false;
    public GameObject blast;
    public GameObject trailParticles;
    private GameObject trail, soundObject;

    private Rigidbody rbody;

    public AudioClip explosionAudio, blastAudio;

    void Start() {
        rbody = GetComponent<Rigidbody>();
        GameObject trail = Instantiate(trailParticles, transform.position, transform.rotation);
        trail.GetComponent<RocketTrail>().rocketParent = gameObject;
        moveVec = transform.forward;

        soundObject = CreateSoundObject(explosionAudio, 0.8f, 0.08f);
        Assert.IsNotNull(soundObject);
    }

    private void FixedUpdate() {
        MoveForwards();
    }

    private void Update() {
        DestroyCheck();
        if (soundObject != null)
            soundObject.transform.position = transform.position;
    }

    void MoveForwards() {
        moveVec = SetMag(moveVec, speed);
        Vector3 newPos = transform.position + moveVec;
        rbody.MovePosition(newPos);
    }

    Vector3 SetMag(Vector3 vec, float mag) {
        return vec.normalized * mag;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "blastable" && !exploded) {
            exploded = true;
            BlastCheck();
            // need to have these two lines here to prevent physics from running twice
            var newBlast = Instantiate(blast, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // checks if player is within blast radius
    // if yes, activate the blast function in the controller
    void BlastCheck() {
        GameObject player = GameObject.Find("Player");
        ScamperController playerController = player.GetComponent<ScamperController>();
        if (Vector3.Distance(transform.position, player.transform.position) < blastRadius) {
            playerController.Blast(transform.position, magnitude, blastRadius);
            var blastSound = CreateSoundObject(blastAudio, 1f, 0.2f);
        }
    }

    void DestroyCheck() {
        if (Vector3.Distance(transform.position, player.transform.position) > destroyDist) {
            Destroy(gameObject);
        }
    }

    GameObject CreateSoundObject(AudioClip sound, float killTime, float volume) {
        GameObject soundObject = new GameObject();
        KillTimer killTimer = soundObject.AddComponent(typeof(KillTimer)) as KillTimer;
        killTimer.duration = killTime;

        var soundComponent = soundObject.AddComponent<AudioSource>();
        soundComponent.clip = sound;
        soundComponent.volume = volume;
        soundComponent.GetComponent<AudioSource>().Play();

        return soundObject;
    }
}