using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the name scamper is left behind from a prototype, but I'll keep it as the name for the movement system I guess
namespace Scamper.Movement {
    public class ScamperController : MonoBehaviour
    {
        // public mainly because some other scripts might need to reset playerpos
        public MoveData moveData = new MoveData();
        public MoveConfig moveConfig;
        private MovePhysics physics = new MovePhysics();

        Timer shootTimer = new Timer(1);

        Rigidbody rbody;

        // MoveData
        private GameObject currentGroundObject;
        public Transform orientation, noClipOrientation;// the only difference is noClipOrientation should have a y component
        private Vector3 velocity = Vector3.zero;

        public Transform spawnPoint;

        public bool paused = false, wasPaused;
        public Vector3 friction;// change this later probably, really random to put it here

        private bool updateHasRun;

        int updates = 0;// DEBUGGING

        void Start()
        {
            rbody = GetComponent<Rigidbody>();
            rbody.isKinematic = false;
            rbody.detectCollisions = true;
        }

        // I promise I have reasons for putting all this in fixed update.
        // There should be details in a dev log, it's too much to put here, ask me if the info is not written somewhere
        void FixedUpdate()
        {
            updates ++;
            // this is to prevent a scenario where physics updates twice in a row
            if (!updateHasRun) {
                UpdateMoveData();
            }
            moveData.acceleration = Vector3.zero;

            HandleMoveWishes();// to make this method more clean

            //moveData.acceleration.y -= moveConfig.gravity * Time.deltaTime;
            if (moveData.blasted) moveData.acceleration += moveData.blastVector;// no delta time since single event
            rbody.velocity += moveData.acceleration;
            rbody.velocity += friction;// probably not the best way to do friction

            ClampSpeeds();

            if (moveData.blastVector != Vector3.zero && moveData.blasted) moveData.blastVector = Vector3.zero;

            WipeMoveData();
            updateHasRun = false;
        }

        void HandleMoveWishes() {
            friction = Vector3.zero;
            // too many ifs, maybe refactor with switch statements of enums in the future

            // for moving the player back to the start of a jump
            if (moveData.resetPosition) {
                ResetPosition();
                return;
            }
            if (moveData.noClip) {
                moveData.acceleration = physics.AirAccelerate(rbody.velocity, moveData.wishdir, moveConfig.noClipSpeed, moveConfig.airAccel);
                friction = physics.CalcFriction(rbody.velocity, moveConfig.frictionCoefficient);
                return;
            }

            if (moveData.playerWishesJump) {
                rbody.velocity = rbody.velocity + new Vector3(0, moveConfig.jumpForce, 0);
            }
            if (moveData.playerWishesMove) {
                if (moveData.onGround) {
                    moveData.wishSpeed += moveConfig.moveSpeed;
                    moveData.acceleration = physics.Accelerate(rbody.velocity, moveData.wishdir, moveData.wishSpeed, moveConfig.accel);
                }
                if (!moveData.onGround) {
                    moveData.wishSpeed = moveConfig.maxAirSpeed;
                    moveData.acceleration += physics.AirAccelerate(rbody.velocity, moveData.wishdir, moveConfig.airMoveSpeed, moveConfig.airAccel);
                }
            }

            if (moveData.onGround) {
                friction = physics.CalcFriction(rbody.velocity, moveConfig.frictionCoefficient);
            }
        }

        void ResetPosition() {
            transform.position = spawnPoint.position;
            rbody.velocity = Vector3.zero;
            moveData.resetPosition = false;
        }

        void Update() {
            updateHasRun = true;
            if (paused) {
                wasPaused = true;
                Time.timeScale = 0;
                return;
            }
            if (wasPaused) {
                wasPaused = false;
                Time.timeScale = 1;
            }

            UpdateMoveData();// I have to update MoveData in Update because then I'll run into the same bug with framerate vs physics update rate

            if (Input.GetButtonDown("Noclip")) {
                moveData.noClip = !moveData.noClip;
                rbody.useGravity = !rbody.useGravity;// better to do this here to save doing it in an unoptimized way later

                if (!moveData.noClip) {
                    moveData.resetPosition = true;
                }
            }
        }

        private void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.tag == "Stage" && !moveData.noClip) {
                GameObject.Find("HUD").GetComponent<LevelTracker>().StageChange(collider.gameObject);
            }
        }

        private void OnCollisionStay(Collision collision) {
            DetectGround(collision, moveConfig.maxGroundAngle);
        }

        private void OnCollisionExit(Collision other) {
            if (other.gameObject == currentGroundObject) {
                moveData.onGround = false;
                currentGroundObject = null;
            }
        }

        public void DetectGround(Collision collisionInfo, float maxGroundAngle) {

            int contactPoints = collisionInfo.contactCount;

            // the whole point of this part is to grab the normal of the collision to see if it's within a certain value, in which case it is floor
            for(int i = 0; i < contactPoints; i++) {
                ContactPoint currentContact = collisionInfo.GetContact(i);

                // to stop the dumb double-jump thing. I'm pretty sure this time that it's needed
                if (moveData.justJumped) {
                    moveData.justJumped = false;
                    return;
                }

                // taking into account playerWishesJump to avoid this overriding the bhop bug D:
                if (IsFloorAngle(currentContact.normal, maxGroundAngle) && !moveData.playerWishesJump) {
                    moveData.onGround = true;
                    currentGroundObject = currentContact.otherCollider.gameObject;
                }
                else if(currentGroundObject == currentContact.otherCollider.gameObject) {
                    moveData.onGround = false;
                    currentGroundObject = null;
                }
            }
        }

        // maybe add an exception for vertical speed (to get ramp sliding)
        bool IsFloorAngle(Vector3 vec, float maxGroundAngle) {
            // grabs angle of a vector and returns true if that angle is less than the maximum floor angle
            float angle = Vector3.Angle(Vector3.up, vec);

            if (angle < maxGroundAngle) return true;
            else return false;
        }

        // I have to put these methods into this class because of some dumb reasons to do with the fact I could not
        // pass a Transform to MoveData.
        // I could probably refactor this if I wanted to, but this actually works fine and I don't think it's worth the effort at all.
        public void UpdateMoveData() {
            GetInput();
            CalcWish();

            if (moveData.wishSpeed != 0) 
                moveData.playerWishesMove = true;
            else moveData.playerWishesMove = false;

        }

        // get input from the player
        void GetInput() {
            moveData.horizontalInput = Input.GetAxisRaw("Horizontal");
            moveData.verticalInput = Input.GetAxisRaw("Vertical");

            // quake like bhop behaviour
            if (Input.GetButtonDown("Jump")) {
                moveData.jumpHold = true;
            }
            if (Input.GetButtonUp("Jump")) {
                moveData.jumpHold = false;
            }

            // two ifs here to account for the fact you may not have auto bhop enabled
            if (moveData.jumpHold && moveData.onGround && moveConfig.autoBhop) {
                moveData.playerWishesJump = true;
                moveData.onGround = false;// resetting onGround to make sure it doesn't get capped later
                moveData.jumpHold = false;
                moveData.justJumped = true;
                return;
            }
            if (Input.GetButtonDown("Jump") && moveData.onGround && !moveConfig.autoBhop ||
                Input.mouseScrollDelta.y < 0f && moveData.onGround && !moveConfig.autoBhop)  {
                moveData.playerWishesJump = true;
                moveData.onGround = false;
                moveData.justJumped = true;
                return;
            }
            if (Input.GetButtonDown("Reset Position")) moveData.resetPosition = true;
        }

        // calculate wishdir, wishSpeed, wishVec based on user input grabbed in GetInput()
        void CalcWish() {
            if (moveData.noClip)
                moveData.wishVec = noClipOrientation.forward * moveData.verticalInput + noClipOrientation.right * moveData.horizontalInput;
            else
                moveData.wishVec = orientation.forward * moveData.verticalInput + orientation.right * moveData.horizontalInput;

            moveData.wishdir = moveData.wishVec.normalized;
            moveData.wishSpeed = moveData.wishdir.magnitude * moveConfig.moveSpeed;
        }

        void WipeMoveData() {
            moveData.playerWishesJump = false;
        }

        void ClampSpeeds() {
            // ground cap 
            if (moveData.onGround && Vector3.Magnitude(rbody.velocity) > moveConfig.maxGroundSpeed) {
                rbody.velocity = Vector3.ClampMagnitude(rbody.velocity, moveConfig.maxGroundSpeed);
                return;
            }

            // air cap
            if (moveConfig.clampAirSpeed && rbody.velocity.magnitude > moveConfig.maxAirSpeed) {
                rbody.velocity = Vector3.ClampMagnitude(rbody.velocity, moveConfig.maxAirSpeed);
                return;
            }
        }

        // this is called by other scripts to apply some force to the player
        public void Blast(Vector3 blastPos, float magnitude, float blastRadius) {
            moveData.blastVector = physics.BlastVector(transform.position, blastPos, magnitude, blastRadius, moveData.onGround);
            moveData.blasted = true;
            print("blasted: " + moveData.blastVector);
        }
    }
}
