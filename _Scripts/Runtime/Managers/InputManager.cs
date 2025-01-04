using FurtleGame.Singleton;
using UnityEngine;

namespace Runtime.Managers
{
    public class InputManager : SingletonMonoBehaviour<InputManager>
    {
        #region Serialized Variables

        [SerializeField] private Joystick joystick;
        [SerializeField] private PlayerMovementController playerManager;

        #endregion

        #region Private Variables

        public float horizontal;
        public float vertical;

        #endregion

        private void Update()
        {
            GetInputs();
        }

        public void GetInputs()
        {
            horizontal = joystick.Horizontal;
            vertical = joystick.Vertical;
        }

        public Vector3 GetMovementInput()
        {
            return new Vector3(horizontal, playerManager.playerRb.velocity.y, vertical);
        }

        public bool GetAnyInput()
        {
            return GetMovementInput() != Vector3.zero;
        }
    }
}