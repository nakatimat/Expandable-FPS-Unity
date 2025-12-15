using UnityEngine;
using UnityEngine.InputSystem;

namespace nakatimat.InputSystem
{
    public class CameraController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerRoot;
        [SerializeField] private Transform cameraRoot;
        [SerializeField] private CharacterController characterController;

        [Header("Mouse Sensitivity")]
        [Range(0, 1)] public float mouseSensitivityX = 0.25f;
        [Range(0, 1)] public float mouseSensitivityY = 0.25f;

        [Header("Gamepad Sensitivity")]
        [Range(0, 1)] public float gamepadSensitivityX = 0.25f;
        [Range(0, 1)] public float gamepadSensitivityY = 0.25f;

        [Header("Look Clamp")]
        [SerializeField] private float minPitch = -80f;
        [SerializeField] private float maxPitch = 80f;

        [Header("Crouch Camera")]
        [SerializeField] private float heightSmooth = 12f;

        private float _pitch;
        private float _currentCameraY;
        private float _baseCameraY;
        private float _standingCenterY;

        private float _sensX;
        private float _sensY;

        private InputManager _input;

        private void Start()
        {
            _input = InputManager.Input;
            ReferenceInitialize();

            _baseCameraY = cameraRoot.localPosition.y;
            _currentCameraY = _baseCameraY;
            _standingCenterY = characterController.center.y;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            if (_input == null) return;

            UpdateSensitivity();
            HandleLook();
            HandleCrouchCamera();
        }

        void ReferenceInitialize()
        {
            characterController = GetComponentInParent<CharacterController>();
        }

        private void UpdateSensitivity()
        {
            // 🎮 Controle ativo
            if (Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame)
            {
                _sensX = gamepadSensitivityX;
                _sensY = gamepadSensitivityY;
            }
            // 🖱 Mouse / Keyboard
            else
            {
                _sensX = mouseSensitivityX;
                _sensY = mouseSensitivityY;
            }
        }

        private void HandleLook()
        {
            Vector2 lookDelta = _input.GetLookDelta();

            float yaw = lookDelta.x * _sensX * Time.deltaTime;
            playerRoot.Rotate(Vector3.up * yaw);

            _pitch -= lookDelta.y * _sensY * Time.deltaTime;
            _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

            cameraRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void HandleCrouchCamera()
        {
            float crouchDelta = characterController.center.y - _standingCenterY;
            float targetY = _baseCameraY + crouchDelta;

            _currentCameraY = Mathf.Lerp(
                _currentCameraY,
                targetY,
                heightSmooth * Time.deltaTime
            );

            Vector3 pos = cameraRoot.localPosition;
            pos.y = _currentCameraY;
            cameraRoot.localPosition = pos;
        }
    }
}
