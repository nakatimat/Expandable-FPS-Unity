using UnityEngine;
using nakatimat.InputSystem;

namespace nakatimat.Player
{
    public class PlayerController : MonoBehaviour
    {
        private InputManager input;
        private CharacterController _charController;
        private Transform _camera;

        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 3f;
        [SerializeField] private float _crouchSpeed = 2f;
        [SerializeField] private float _sprintSpeed = 6f;
        [SerializeField] private float _speedChangeDamping = 10f;

        [Header("Jump & Gravity")]
        [SerializeField] private float _jumpForce = 7f;
        [SerializeField] private float _gravityMultiplayer = 2f;
        [SerializeField] private float _groundedOffset = -0.1f;
        [SerializeField] private float _groundCheckRadius = 0.1f;
        [SerializeField] private LayerMask _groundLayerMask;

        [Header("Capsule Values")]
        [SerializeField] private float _capsuleStandingHeight = 1.8f;
        [SerializeField] private float _capsuleStandingCenter = 1f;
        [SerializeField] private float _capsuleCrouchingHeight = 1.2f;
        [SerializeField] private float _capsuleCrouchingCenter = 0.6f;

        [SerializeField] private Transform _standBottom;
        [SerializeField] private Transform _standTop;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private float _radius = 0.3f;

        private Vector3 _moveDirection;
        private float _currentSpeed;
        private float _verticalVelocity;

        private bool _isSprint;
        private bool _isGrounded;
        private bool _isCrouching;

        void Start()
        {
            input = InputManager.Input;
            _charController = GetComponent<CharacterController>();
            _camera = Camera.main.transform;

            SubscribeInput(true);
        }

        private void OnDisable()
        {
            SubscribeInput(false);
        }

        void Update()
        {
            HandleInput();
            UpdateSpeedState();
            GroundCheck();
            ApplyGravity();
            HandleMovement();
        }

        private void SubscribeInput(bool subscribe)
        {
            if (input == null) return;

            if (subscribe)
            {
                input.onJumpPerformed += Jump;
                input.onSprintPerformed += SprintPerformed;
                input.onSprintCanceled += SprintCanceled;
                input.onCrouchToggle += OnCrouchToggle;
            }
            else
            {
                input.onJumpPerformed -= Jump;
                input.onSprintPerformed -= SprintPerformed;
                input.onSprintCanceled -= SprintCanceled;
                input.onCrouchToggle -= OnCrouchToggle;
            }
        }

        void HandleInput()
        {
            _moveDirection = GetCameraRelativeDirection(input.GetAxis());
        }

        private Vector3 GetCameraRelativeDirection(Vector2 axis)
        {
            Vector3 forward = _camera.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = _camera.right;
            right.y = 0f;
            right.Normalize();

            Vector3 desired = (forward * axis.y) + (right * axis.x);
            return desired.sqrMagnitude > 1f ? desired.normalized : desired;
        }

        private void HandleMovement()
        {
            Vector3 velocity = (_moveDirection * _currentSpeed) + Vector3.up * _verticalVelocity;
            _charController.Move(velocity * Time.deltaTime);
        }

        private void UpdateSpeedState()
        {
            float inputMagnitude = _moveDirection.magnitude;

            float targetSpeed = 0f;

            if (_isCrouching) targetSpeed = _crouchSpeed;
            else if (_isSprint) targetSpeed = _sprintSpeed;
            else targetSpeed = _walkSpeed;

            if (inputMagnitude < 0.01f) targetSpeed = 0f;

            _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _speedChangeDamping * Time.deltaTime);
        }

        private void GroundCheck()
        {
            Vector3 spherePos = transform.position + Vector3.up * _groundedOffset;
            _isGrounded = Physics.CheckSphere(
                spherePos,
                _groundCheckRadius,
                _groundLayerMask,
                QueryTriggerInteraction.Ignore
            );
        }

        private void ApplyGravity()
        {
            if (_isGrounded && _verticalVelocity < 0f)
                _verticalVelocity = -2f;

            _verticalVelocity += Physics.gravity.y * _gravityMultiplayer * Time.deltaTime;
        }

        private void Jump()
        {
            if (_isGrounded)
                _verticalVelocity = _jumpForce;
        }

        private void SprintPerformed()
        {
            if (CanStandUp() == false) return;

            _isSprint = true;
            _isCrouching = false;
            CapsuleCrouchingSize(false);
        }

        private void SprintCanceled()
        {
            _isSprint = false;
        }

        private void OnCrouchToggle()
        {
            if (CanStandUp() == false) return;

            _isCrouching = !_isCrouching;
            CapsuleCrouchingSize(_isCrouching);
        }

        private void CapsuleCrouchingSize(bool crouching)
        {
            _charController.height = crouching ? _capsuleCrouchingHeight : _capsuleStandingHeight;
            _charController.center = new Vector3(
                0f,
                crouching ? _capsuleCrouchingCenter : _capsuleStandingCenter,
                0f
            );
        }

        private bool CanStandUp()
        {
            return !Physics.CheckCapsule(
                _standBottom.position,
                _standTop.position,
                _radius,
                _obstacleMask,
                QueryTriggerInteraction.Ignore
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 spherePos = transform.position + Vector3.up * _groundedOffset;
            Gizmos.DrawWireSphere(spherePos, _groundCheckRadius);

            Gizmos.color = Color.cyan;
            Vector3 p1 = _standBottom.position;
            Vector3 p2 = _standTop.position;

            Gizmos.DrawWireSphere(p1, _radius);
            Gizmos.DrawWireSphere(p2, _radius);
        }
    }
}
