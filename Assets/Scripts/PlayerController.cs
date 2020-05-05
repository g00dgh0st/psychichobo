using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
  private CharacterController controller;
  private Animator anim;

  private float groundCheckDistance = 0.1f;
  public LayerMask groundLayerMask;
  private float fallMultiplier = 2.5f;
  private float lowJumpMultiplier = 2f;
  private float jumpPower = 10f;
  private float moveSpeed = 5f;

  private bool isGrounded;
  private bool hasJumped;
  private Vector3 moveVector;
  private bool useAnimatorMotion = false;

  void Awake() {
    controller = GetComponent<CharacterController>();
    anim = GetComponent<Animator>();
  }

  void Update() {
    ApplyGravity();

    // float v = Input.GetAxis("Vertical");
    float h = Input.GetAxis("Horizontal");
    ApplyMove(h);

    if (Input.GetButtonDown("Jump")) {
      Jump();
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
    moveVector.y += jumpPower;
  }

  private void ApplyMove(float horizontal) {
    anim.SetFloat("moveSpeed", Mathf.Abs(horizontal));

    if (horizontal != 0) {
      Vector3 moveDir = Vector3.right.normalized * moveSpeed * horizontal;
      transform.forward = horizontal > 0 ? Vector3.right : Vector3.left;
      moveVector.x = moveDir.x;
    }
  }

  private bool ApplyGravity() {
    if (controller.isGrounded) {
      isGrounded = true;
      anim.SetBool("grounded", true);

      hasJumped = false;
      moveVector = Physics.gravity * Time.deltaTime;
      return true;

      // keep player on ground when walking down slopes
    } else if (!hasJumped && moveVector.y <= 0 && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayerMask)) {
      isGrounded = true;
      anim.SetBool("grounded", true);
      moveVector = (hit.point - transform.position) / Time.deltaTime;
      return true;
    }

    isGrounded = false;
    anim.SetBool("grounded", false);

    if (moveVector.y <= 0) {
      moveVector += Physics.gravity * Time.deltaTime * (fallMultiplier);
    } else if (hasJumped && !Input.GetButton("Jump")) {
      moveVector += Physics.gravity * Time.deltaTime * (lowJumpMultiplier);
    } else {
      moveVector += Physics.gravity * Time.deltaTime;
    }

    return false;
  }

  void TurnEvent(string type) {
    if (type == "start") {
      useAnimatorMotion = true;
    } else {
      useAnimatorMotion = false;

    }
  }
}