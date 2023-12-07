using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scamper.Movement {
    [System.Serializable]
    public class MoveConfig
    {
        [Header ("Jumping and Gravity")]
        public bool autoBhop;
        public float jumpForce;
        public float gravity;

        [Header ("Ground Movement")]
        public float moveSpeed;
        public float maxGroundSpeed;
        public float accel;// pretty much the same as airAccel but for the ground, having this value very low will make the movement kind of "slippery"

        [Header ("Air Movement")]
        public bool clampAirSpeed;
        public float maxAirSpeed;// how fast you're allowed to move in the air
        public float airMoveSpeed;// how much force you move with in the air, different from airAccel in that it doesn't affect how tight you are able to air strafe as much
        public float airAccel;// how tight you are able to make air strafes

        [Header ("Random Crap (Misc.)")]
        public float maxGroundAngle;
        public float frictionCoefficient;
        public float noClipSpeed;
    }
}
