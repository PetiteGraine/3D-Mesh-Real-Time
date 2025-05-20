using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimplePlayerController : MonoBehaviour
{
    public float Speed = 6.0f;
    public float Gravity = -9.81f;
    public float JumpHeight = 1.5f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;

    public Transform GroundCheck;
    public float GroundDistance = 0.4f;
    public LayerMask GroundMask;

    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        _isGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);

        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * Speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && _isGrounded)
            _velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);

        _velocity.y += Gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
