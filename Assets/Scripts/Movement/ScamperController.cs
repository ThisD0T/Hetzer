using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    public class ScamperController : MonoBehaviour
    {
        private MoveData moveData = new MoveData();
        public MoveConfig moveConfig;
        private MovePhysics movePhysics = new MovePhysics();

        private Vector3 acceleration;

        Rigidbody rbody;

        // MoveData
        private GameObject currentGroundObject;
        private bool onGround;
        public Transform orientation;
        private Vector3 velocity = Vector3.zero;

        void Start()
        {
            rbody = GetComponent<Rigidbody>();
            rbody.isKinematic = false;
            rbody.detectCollisions = true;
        }

        // Update is called once per frame
        void Update()
        {
            acceleration = Vector3.zero;

            UpdateMoveData();

            if (moveData.playerWishesMove) {
                if (onGround) {
                    moveData.wishSpeed = moveConfig.moveSpeed;
                    acceleration = movePhysics.Accelerate(rbody.velocity, moveData.wishdir, moveData.wishSpeed, moveConfig.accel);
                }
                if (!onGround) {
                    moveData.wishSpeed = moveConfig.maxAirSpeed;
                    acceleration = movePhysics.AirAccelerate(rbody.velocity, moveData.wishdir, moveData.wishSpeed, moveConfig.airAccel);
                }
            }

            if (moveData.playerWishesJump) {
                acceleration.y += moveConfig.jumpForce;
                print("tried to jump");
            }

            acceleration.y -= moveConfig.gravity;
        }

        void FixedUpdate() {
            rbody.velocity += acceleration;
        }

        private void OnCollisionStay(Collision collisionInfo) {
            DetectGround(collisionInfo, moveConfig.maxGroundAngle);
        }

        private void OnCollisionExit(Collision other) {
            if (other.gameObject == currentGroundObject) {
                onGround = false;
                currentGroundObject = null;
            }
        }

        public void DetectGround(Collision collisionInfo, float maxGroundAngle) {

            int contactPoints = collisionInfo.contactCount;

            for(int i = 0; i < contactPoints; i++) {
                ContactPoint currentContact = collisionInfo.GetContact(i);
                if (IsFloorAngle(currentContact.normal, maxGroundAngle)) {
                    onGround = true;
                    currentGroundObject = currentContact.otherCollider.gameObject;
                }
                else if(currentGroundObject == currentContact.otherCollider.gameObject) {
                    onGround = false;
                    currentGroundObject = null;
                }
            }
        }

        // maybe add an exception for vertical speed
        bool IsFloorAngle(Vector3 vec, float maxGroundAngle) {
            float angle = Vector3.Angle(Vector3.up, vec);

            if (angle < maxGroundAngle) return true;
            else return false;
        }

        // I have to put these methods into this class because of some dumb reasons to do with the fact I could not
        // pass a Transform to MoveData. 
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

            if (Input.GetKeyDown("space") && moveData.onGround) {
                moveData.playerWishesJump = true;
                print("wishes jump");
            }
        }

        // calculate wishdir, wishSpeed, wishVec
        void CalcWish() {
            moveData.wishVec = orientation.forward * moveData.verticalInput + orientation.right * moveData.horizontalInput;
            moveData.wishdir = moveData.wishVec.normalized;
            moveData.wishSpeed = moveData.wishdir.magnitude * moveConfig.moveSpeed;
        }
    }
}
