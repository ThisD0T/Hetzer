using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    public class MoveData : MonoBehaviour
    {
        public Transform orientation;

        public Vector3 wishdir;
        public float wishSpeed;
        public Vector3 wishVec;

        public bool playerWishesMove;

        float horizontalInput;
        float verticalInput;

        public bool onGround;
        private GameObject currentGroundObject;

        public void UpdateMoveData() {
            GrabInput();
            CalcWish();

            if (wishSpeed != 0) 
                playerWishesMove = true;
            else playerWishesMove = false;
        }

        // get input from the player
        void GrabInput() {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");
        }

        // calculate wishdir, wishSpeed, wishVec
        void CalcWish() {
            wishVec = orientation.forward * verticalInput + orientation.right * horizontalInput;
            wishSpeed = wishVec.magnitude;
            wishdir = wishVec.normalized;
        }

        public void DetectGround(Collision collisionInfo, float maxSlopeAngle) {
            foreach (ContactPoint contact in collisionInfo.contacts) {
                if (IsFloorAngle(contact.normal, maxSlopeAngle)) {
                    onGround = true;
                    currentGroundObject = contact.otherCollider.gameObject;
                    return;
                }
                else if(currentGroundObject == contact.otherCollider.gameObject) {
                    onGround = false;
                    currentGroundObject = null;
                }
            }
        }

        bool IsFloorAngle(Vector3 vec, float maxSlopeAngle) {
            float angle = Vector3.Angle(Vector3.up, vec);
            
            if (angle < maxSlopeAngle) return true;
            if (angle > maxSlopeAngle) return false;

            return false;
        }
    }
}
