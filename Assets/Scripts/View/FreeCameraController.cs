using UnityEngine;
using UnityEngine.InputSystem;

namespace MatrixMatcher.View
{
    public class FreeCameraController : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 10f;

        [SerializeField]
        private float _fastMoveSpeed = 30f;

        [SerializeField]
        private float _mouseSensitivity = 2f;

        private float _pitch;
        private float _yaw;

        private void Start()
        {
            var euler = transform.rotation.eulerAngles;
            _yaw = euler.y;
            _pitch = euler.x;
        }

        private void Update()
        {
            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            if (mouse == null || keyboard == null)
            {
                return;
            }

            if (mouse.rightButton.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            if (mouse.rightButton.wasReleasedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            if (mouse.rightButton.isPressed)
            {
                var delta = mouse.delta.ReadValue();
                _yaw += delta.x * _mouseSensitivity;
                _pitch -= delta.y * _mouseSensitivity;
                _pitch = Mathf.Clamp(_pitch, -89f, 89f);

                transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
            }

            float speed = keyboard.leftShiftKey.isPressed ? _fastMoveSpeed : _moveSpeed;
            speed *= Time.deltaTime;

            Vector3 move = Vector3.zero;

            if (keyboard.wKey.isPressed)
            {
                move += transform.forward;
            }

            if (keyboard.sKey.isPressed)
            {
                move -= transform.forward;
            }

            if (keyboard.aKey.isPressed)
            {
                move -= transform.right;
            }

            if (keyboard.dKey.isPressed)
            {
                move += transform.right;
            }

            if (keyboard.qKey.isPressed)
            {
                move -= transform.up;
            }

            if (keyboard.eKey.isPressed)
            {
                move += transform.up;
            }

            transform.position += move * speed;
        }
    }
}