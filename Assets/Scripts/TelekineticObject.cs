using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEditor;
using UnityEngine;

public class TelekineticObject : MonoBehaviour {
  private MeshRenderer renderer;
  private Rigidbody rigidbody;
  private Camera mainCam;

  [SerializeField]
  private Material defaultMat;
  [SerializeField]
  private Material telekinesisMat;

  private bool dragging = false;
  private bool isFrozen = false;

  private void Awake() {
    mainCam = Camera.main;
    renderer = GetComponent<MeshRenderer>();
    rigidbody = GetComponent<Rigidbody>();
  }

  private void OnCollisionEnter(Collision other) {
    // Doesnt trigger if you unfreeze while player in standing on it
    if (other.collider.tag == Tags.Player) {
      EndDrag();
    }
  }

  private void OnMouseEnter() {
    renderer.material = telekinesisMat;
  }

  void OnMouseExit() {
    if (!dragging)
      renderer.material = defaultMat;
  }

  private void OnMouseDown() {
    BeginDrag();
  }

  private void OnMouseUp() {
    if (dragging) {
      EndDrag();
    }
  }

  private void BeginDrag() {
    if (isFrozen) Unfreeze();

    renderer.material = telekinesisMat;
    rigidbody.useGravity = false;
    dragging = true;
    rigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
  }

  private void EndDrag() {
    renderer.material = defaultMat;
    dragging = false;
    rigidbody.useGravity = true;
    rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
  }

  private void Freeze() {
    isFrozen = true;
    rigidbody.useGravity = false;
    rigidbody.constraints = RigidbodyConstraints.FreezeAll;
  }

  private void Unfreeze() {
    isFrozen = false;
    rigidbody.useGravity = true;
    rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
  }

  [SerializeField]
  private AnimationCurve lowMovementCurve;
  [SerializeField]
  private AnimationCurve fullMovementCurve;

  private float maxVelocity = 15f;
  private float lowMovementMaxDistance = 1f;
  private float lowMovementMaxVelocity = 3f;
  private float fullMovementMaxDistance = 6f;
  private float fullMovementMaxVelocity = 20f;
  private float decelMinDistance = 1.3f;

  private float lastMouseDist;

  void FixedUpdate() {
    if (dragging) {
      if (Input.GetMouseButtonDown(1)) {
        EndDrag();
        Freeze();

        return;
      }

      // TODO: move this into a global input manager or something class
      Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
      Plane pp = new Plane(-Vector3.forward, Vector3.zero);
      pp.Raycast(mouseRay, out float rayDist);
      Vector3 velocityVector = mouseRay.GetPoint(rayDist) - transform.position;
      float velocityMagnitude;

      if (lastMouseDist <= velocityVector.magnitude) {
        GameObject.Find("Core").GetComponent<UIManager>().SetText("");
        // Accelerating
        if (velocityVector.magnitude < lowMovementMaxDistance) {
          // low move threshold
          float x = velocityVector.magnitude / lowMovementMaxDistance;
          velocityMagnitude = lowMovementMaxVelocity * lowMovementCurve.Evaluate(x);
        } else {
          // full move threshold
          float x = (velocityVector.magnitude - lowMovementMaxDistance) / (fullMovementMaxDistance - lowMovementMaxDistance);
          velocityMagnitude = Mathf.Lerp(lowMovementMaxVelocity, fullMovementMaxVelocity, fullMovementCurve.Evaluate(x));
        }
      } else if (velocityVector.magnitude < decelMinDistance) {
        // decelerating
        GameObject.Find("Core").GetComponent<UIManager>().SetText("decel");
        float x = velocityVector.magnitude / decelMinDistance;
        velocityMagnitude = lowMovementMaxVelocity * lowMovementCurve.Evaluate(x);
      } else {
        GameObject.Find("Core").GetComponent<UIManager>().SetText("");
        velocityMagnitude = rigidbody.velocity.magnitude;
      }

      lastMouseDist = velocityVector.magnitude;

      rigidbody.velocity = velocityVector.normalized * velocityMagnitude;

      // GameObject.Find("Core").GetComponent<UIManager>().SetText(velocityVector.magnitude < lowMovementMaxDistance ? "LOW" : velocityVector.magnitude + "     " + rigidbody.velocity.magnitude);
    }
  }
}