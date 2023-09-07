using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    [System.Serializable]
    public class MoveConfig
    {
        [Header ("Jumping and Gravity")]
        public bool autoBhop;
        public float gravity;

        [Header ("Ground Movement")]
        public float moveSpeed;
        public float maxGroundSpeed;
        public float accel;

        [Header ("Air Movement")]
        public bool clampAirSpeed;
        public float maxAirSpeed;
        public float airAccel;

        [Header ("Random Crap (Misc.)")]
        public float maxGroundAngle;
    }
}
