using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    public class MoveData
    {
        public Vector3 wishdir;
        public float wishSpeed = 0;
        public Vector3 wishVec;

        public bool playerWishesMove;
        public bool playerWishesJump;

        public float horizontalInput = 0;
        public float verticalInput = 0;

        public bool onGround;
        private GameObject currentGroundObject;

    }
}
