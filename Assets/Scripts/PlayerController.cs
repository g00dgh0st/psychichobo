using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
  // dependencies
  private CharacterController controller;
  private Animator anim;

  // config vars
  private float groundCheckDistance = 0.5f;
  public LayerMask groundLayerMask;
  private float fallMultiplier = 2.5f;
  private float lowJumpMultiplier = 2f;
  private float jumpPower = 10f;
  private float jumpBufferTime = 0.2f;
  private float coyoteJumpTime = 0.15f;
  private float moveSpeed = 5f;

  // local state vars
  private bool isGrounded = false;
  private bool hasJumped = false;
  private float lastGroundedTime;
  private float lastJumpButtonPress;
  private Vector3 moveVector;
  private bool useAnimatorMotion = false;

  void Awake() {
    controller = GetComponent<CharacterController>();
    anim = GetComponent<Animator>();
  }

  void Update() {
    ApplyGravity();

    // check for buffered jump
    if (isGrounded && Time.time - lastJumpButtonPress < jumpBufferTime) {
      Jump();
    }

    // float v = Input.GetAxis("Vertical");
    float h = Input.GetAxisRaw("Horizontal");
    ApplyMove(h);

    if (Input.GetButtonDown("Jump")) {
      if (isGrounded) {
        // standard jump
        Jump();
      } else if (!hasJumped && Time.time - lastGroundedTime < coyoteJumpTime) {
        // coyote jump
        Jump();
      }

      // buffer jump
      lastJumpButtonPress = Time.time;
    }

    MakeMove();
  }

  void OnAnimatorMove() {
    if (useAnimatorMotion) {
      controller.Move(anim.deltaPosition);
      transform.forward = anim.deltaRotation * transform.forward;
    }
  }

  private void MakeMove() {
    controller.Move(moveVector * Time.deltaTime);
  }

  private void Jump() {
    hasJumped = true;
    isGrounded = false;
    moveVector.y = jumpPower;
  }

  private void ApplyMove(float horizontal) {
    if (horizontal == 0) {
      anim.SetFloat("moveSpeed", Mathf.Lerp(anim.GetFloat("moveSpeed"), horizontal, 0.2f));
    } else {
      anim.SetFloat("moveSpeed", Mathf.Abs(horizontal));

      Vector3 moveDir = Vector3.right.normalized * moveSpeed * horizontal;
      transform.forward = horizontal > 0 ? Vector3.right : Vector3.left;
      moveVector.x = moveDir.x;
    }
  }

  private bool ApplyGravity() {
    if (controller.isGrounded) {
      Land();
      moveVector = Physics.gravity * Time.deltaTime;
      return true;

      // keep player on ground when walking down slopes
    } else if (!hasJumped && moveVector.y <= 0 && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayerMask)) {
      Land();

      moveVector = (hit.point - transform.position) / Time.deltaTime;
      return true;
    }

    Airborne();

    if (moveVector.y <= 0) {
      moveVector += Physics.gravity * Time.deltaTime * (fallMultiplier);
    } else if (hasJumped && !Input.GetButton("Jump")) {
      moveVector += Physics.gravity * Time.deltaTime * (lowJumpMultiplier);
    } else {
      moveVector += Physics.gravity * Time.deltaTime;
    }

    return false;
  }

  void Land() {
    isGrounded = true;
    anim.SetBool("grounded", true);
    hasJumped = false;
  }

  void Airborne() {
    if (isGrounded == true) {
      lastGroundedTime = Time.time;
    }
    isGrounded = false;
    anim.SetBool("grounded", false);
  }

  void TurnEvent(string type) {
    if (type == "start") {
      useAnimatorMotion = true;
    } else {
      useAnimatorMotion = false;

    }
  }
}