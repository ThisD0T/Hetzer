using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    public class ScamperController : MonoBehaviour
    {
        private MoveData moveData;
        public MoveConfig moveConfig;
        private MovePhysics movePhysics;

        private Vector3 acceleration;

        Rigidbody rbody;

        // MoveData

        void Start()
        {
            rbody = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            acceleration = Vector3.zero;

            moveData.UpdateMoveData();

            if (moveData.playerWishesMove) {
                if (moveData.onGround) {
                    acceleration = movePhysics.Accelerate(rbody.velocity, moveData.wishdir, moveData.wishSpeed);
                }
                if (!moveData.onGround) {
                    acceleration = movePhysics.AirAccelerate(rbody.velocity, moveData.wishdir, moveData.wishSpeed);
                }
            }

            acceleration.y -= moveConfig.gravity;

            rbody.velocity += acceleration;
        }

        private void ObCollisionstay(Collision collisionInfo) {
            moveData.DetectGround(collisionInfo, moveConfig.maxGroundAngle);
        }
    }
}
