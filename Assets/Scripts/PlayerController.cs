using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEngine;

[System.Serializable]
public struct LedgeCatchConfig {
  public float forwardOffset;
  public float botY;
  public float midY;
  public float topY;
  public float castDistance;
  public float yLowCatchOffset;
  public float yHangOffset;
  public float xLowCatchOffset;
  public float xHangOffset;
}

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
  private LedgeCatchConfig ledgeConfig = new LedgeCatchConfig {
    forwardOffset = 0.2f,
    botY = 0.2f,
    midY = 1.1f,
    topY = 2f,
    castDistance = 0.4f,
    yLowCatchOffset = .75f,
    xLowCatchOffset = 0.35f,
    yHangOffset = 2f,
    xHangOffset = 0.1f
  };
  private float ledgeCatchLerpSpeed = 0.1f;

  // local state vars
  private bool isLedgeHanging = false;

  /// NOTE: some state control might help
  private bool isLedgeCatching = false;
  private Vector3 ledgeCatchPosition;
  private Vector3 ledgeCatchStartPosition;
  private float ledgeCatchLerp;
  private bool isOnHighLedge = false;

  private bool isLedgeClimbing = false;
  private bool isGrounded = false;
  private bool hasJumped = false;
  private float lastGroundedTime;
  private float lastJumpButtonPress;
  private Vector3 moveVector;
  private bool useAnimatorMotion = false;

  private Collider currentLadder;
  private bool isLadderClimbing;

  public void AttachLadder(Collider ladder) {
    currentLadder = ladder;
    // if (!isGrounded) {
    //   ClimbLadder();
    // }
  }

  public void DetachLadder() {
    currentLadder = null;
    isLadderClimbing = false;
  }

  void Awake() {
    controller = GetComponent<CharacterController>();
    anim = GetComponent<Animator>();
  }

  private void OnControllerColliderHit(ControllerColliderHit hit) {
    if (hit.collider.tag == Tags.Ledge && !isGrounded) {
      Vector3 forwardVector = transform.position + (transform.forward * ledgeConfig.forwardOffset);
      bool topHit = false, midHit = false, botHit = false;

      // check top
      Vector3 topStart = forwardVector + new Vector3(0, ledgeConfig.topY, 0);
      topHit = Physics.Raycast(topStart, transform.forward, ledgeConfig.castDistance, groundLayerMask);
      Vector3 midStart = forwardVector + new Vector3(0, ledgeConfig.midY, 0);
      midHit = Physics.Raycast(midStart, transform.forward, ledgeConfig.castDistance, groundLayerMask);
      Vector3 botStart = forwardVector + new Vector3(0, ledgeConfig.botY, 0);
      botHit = Physics.Raycast(botStart, transform.forward, ledgeConfig.castDistance, groundLayerMask);

      if (topHit) {
        // too high
        return;
      } else if (midHit) {
        // high catch
        CatchLedge(true, hit.collider);
      } else if (botHit) {
        // low catch
        CatchLedge(false, hit.collider);
      }

      // no catch
    }
  }

  void Update() {
    // TODO: state management here for different control states
    if (!isLedgeHanging && !isLadderClimbing) {
      ApplyGravity();

      // check for buffered jump
      if (isGrounded && Time.time - lastJumpButtonPress < jumpBufferTime) {
        Jump();
      }

      if (currentLadder != null && Input.GetKeyDown(KeyCode.W)) {
        // NOTE: TEMP CRAP
        ClimbLadder();
        return;
      }

      // float v = Input.GetAxis("Vertical");
      float h = Input.GetAxisRaw("Horizontal");
      ApplyMovement(h);

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
    } else if (isLedgeHanging) {
      if (isLedgeCatching) {
        ledgeCatchLerp += Time.deltaTime / ledgeCatchLerpSpeed;
        transform.position = Vector3.Lerp(ledgeCatchStartPosition, ledgeCatchPosition, ledgeCatchLerp);

        if (transform.position == ledgeCatchPosition) isLedgeCatching = false;
      } else {
        if (!isOnHighLedge) {
          ClimbLedge();
        } else {
          if (Input.GetKeyDown(KeyCode.W)) {
            ClimbLedge();

          } else if (Input.GetKeyDown(KeyCode.S)) {
            ReleaseLedge();
          }

        }
      }
    } else if (isLadderClimbing) {
      float v = Input.GetAxis("Vertical");
      anim.SetFloat("moveSpeed", v);

      if (Input.GetButtonDown("Jump")) {
        ReleaseLadder();
      }
    }
  }

  void OnAnimatorMove() {
    if (isLedgeClimbing || isLadderClimbing) {
      transform.position += anim.deltaPosition;
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

  private void ApplyMovement(float horizontal) {
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
    } else if (!hasJumped
      && moveVector.y <= 0
      && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayerMask, QueryTriggerInteraction.Ignore)
    ) {
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

  void CatchLedge(bool isHigh, Collider ledgeCollider) {
    isLedgeHanging = true;
    moveVector = Vector3.zero;
    controller.enabled = false;
    Bounds bds = ledgeCollider.bounds;

    // TODO: reuse ledge catching lerp for ladder and rope, etc
    isLedgeCatching = true;
    ledgeCatchStartPosition = transform.position;
    ledgeCatchLerp = 0f;

    anim.ResetTrigger("ledgeClimb");
    anim.SetBool("ledgeHanging", true);
    if (isHigh) {
      anim.SetBool("isHighLedge", true);
      isOnHighLedge = true;
      float hangingXPos;
      if (transform.forward.x > 0) {
        // facing right
        hangingXPos = bds.min.x - ledgeConfig.xHangOffset;
      } else {
        // facing left
        hangingXPos = bds.max.x + ledgeConfig.xHangOffset;
      }

      ledgeCatchPosition = new Vector3(hangingXPos, bds.max.y - ledgeConfig.yHangOffset, 0);
    } else {
      anim.SetBool("isHighLedge", false);
      isOnHighLedge = false;
      float hangingXPos;
      if (transform.forward.x > 0) {
        // facing right
        hangingXPos = bds.min.x - ledgeConfig.xLowCatchOffset;
      } else {
        // facing left
        hangingXPos = bds.max.x + ledgeConfig.xLowCatchOffset;
      }

      ledgeCatchPosition = new Vector3(hangingXPos, bds.max.y - ledgeConfig.yLowCatchOffset, 0);
    }

  }

  void ClimbLadder() {
    anim.SetBool("ladderClimb", true);
    isLadderClimbing = true;
    moveVector = Vector3.zero;
    controller.enabled = false;
    Bounds bds = currentLadder.bounds;
    transform.position = new Vector3(bds.center.x, transform.position.y, 0f);
    transform.forward = Vector3.forward;
  }

  void ReleaseLadder() {
    anim.SetBool("ladderClimb", false);
    DetachLadder();
    controller.enabled = true;
  }

  void ClimbLedge() {
    hasJumped = false;
    isLedgeClimbing = true;
    anim.SetTrigger("ledgeClimb");
  }

  void ReleaseLedge() {
    isLedgeHanging = false;
    LedgeClimbEvent("end");
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

  // Animation Events
  void LedgeClimbEvent(string type) {
    if (type == "end") {
      anim.SetBool("ledgeHanging", false);
      isLedgeHanging = false;
      isLedgeClimbing = false;
      controller.enabled = true;
    }
  }
}