using UnityEngine;

namespace Character
{
    public static class PlayerInput
    {
        public static bool IsInputRangedAttack()
        {
            return Input.GetKeyDown(KeyCode.R);
        }
        public static bool IsInputMeleeAttack(out int attackNum)
        {
            attackNum = 0;
            if (Input.GetKeyDown(KeyCode.E)) attackNum = 1;
            else if (Input.GetKeyDown(KeyCode.R)) attackNum = 2;

            return attackNum > 0;
        }
        public static bool IsInputSlide()
        {
            return IsInputRun(out var dir) && Input.GetKey(KeyCode.C);
        }
        public static bool IsInputEscape()
        {
            return Input.GetKeyDown(KeyCode.Escape);
        }
        public static bool IsInputRun(out Vector3 dir)
        {
            return IsInputWalk(out dir) && Input.GetKey(KeyCode.LeftShift);
        }
        public static bool IsInputSelfHit()
        {
            return Input.GetKeyDown(KeyCode.Alpha1);
        }
        public static bool IsInputWalk(out Vector3 dir)
        {
            dir = Vector3.zero;
            dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");
            return dir != Vector3.zero;
        }
        public static bool IsInputJump()
        {
            return Input.GetAxisRaw("Jump") != 0;
        }
        public static bool IsInputRetry()
        {
            return Input.GetAxisRaw("Cancel") != 0;
        }
        public static bool IsInputInteraction(out InteractState state)
        {
            state = InteractState.None;

            if (Input.GetKeyDown(KeyCode.F)) state = InteractState.Begin;
            else if (Input.GetKey(KeyCode.F)) state = InteractState.Tick;
            else if (Input.GetKeyUp(KeyCode.F)) state = InteractState.Cancel;

       
            return state != InteractState.None;
        }

        public static bool IsInputConsumption(out int slotIndex)
        {
            slotIndex = -1;

            if (Input.GetKeyDown(KeyCode.Alpha1)) slotIndex = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha2)) slotIndex = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha3)) slotIndex = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha4)) slotIndex = 4;
            return slotIndex >= 0;
        }
    }
}