using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform _camera;
    LayerMask _groundLayer;
    Vector3 _playerGravity;
    [SerializeField]float gravity = 9.81f;
    [SerializeField]CharacterController _controller;
    [SerializeField]float _playerSpeed = 5;
    [SerializeField]float _jumpHeight = 5;
    [SerializeField]float _sensorRadius = 0.2f;
    [SerializeField]float _turnSmoothTime = 0.1f;
    float _turnSmoothVelocity;
    Transform _sensorPosition;
    Animator _animator;
    float _horizontal;
    float _vertical;
    bool _isGrounded;
    Transform target;

    // Start is called before the first frame update
    void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _camera = Camera.main.transform;
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");

        if(Input.GetButtonDown("Fire2"))
        {
            AimMovement();
        }
        Movement();
        Jump();
    }

    void Movement()
    {
        Vector3 moveDirection = new Vector3(_horizontal, 0, _vertical);
        _animator.SetFloat("VelX", 0);
        _animator.SetFloat("VelZ", moveDirection.magnitude);

        if(moveDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg * _camera.eulerAngles.y;
            float _smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref _turnSmoothVelocity, _turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, _smoothAngle, 0);
            Vector3 move = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            _controller.Move(move * _playerSpeed * Time.deltaTime);
        }

    }

    void AimMovement()
    {
        Vector3 moveDirection = new Vector3(_horizontal, 0, _vertical);
        _animator.SetFloat("VelX", _horizontal);
        _animator.SetFloat("VelZ", _vertical);

        
        float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg * _camera.eulerAngles.y;
        float _smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, target.eulerAngles.y, ref _turnSmoothVelocity, _turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, _smoothAngle, 0);
        
        if(moveDirection != Vector3.zero)
        {
            Vector3 move = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            _controller.Move(moveDirection * _playerSpeed * Time.deltaTime);
        }

    }

    void Jump()
    {
        _isGrounded = Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);

        _animator.SetBool("IsJumping", !_isGrounded);

        if(_isGrounded && _playerGravity.y < 0)
        {
            _playerGravity.y = -2;
        }

        if(_isGrounded && Input.GetButtonDown("Jump"))
        {
            _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * gravity);
        }

        _playerGravity.y += gravity * Time.deltaTime;
        _controller.Move(_playerGravity * _playerSpeed * Time.deltaTime);
    }
}
