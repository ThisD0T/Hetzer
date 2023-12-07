using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    public class MoveData
    {
        public bool noClip = false;

        public Vector3 wishdir;
        public float wishSpeed = 0;
        public Vector3 wishVec;

        public Vector3 velocity;
        public Vector3 acceleration;

        public bool playerWishesMove;
        public bool playerWishesJump;

        public float horizontalInput = 0;
        public float verticalInput = 0;

        public bool onGround;
        public bool jumpHold;// true if the player is holding jump and hasn't jumped yet (to give quake-style autobhop)
        public bool justJumped;

        public Vector3 blastVector;
        public bool blasted;
        private GameObject currentGroundObject;

        public bool resetPosition;
    }
}
