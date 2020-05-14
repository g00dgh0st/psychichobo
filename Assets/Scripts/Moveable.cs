using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour {
  private MeshRenderer renderer;
  private Rigidbody rigidbody;
  private Camera mainCam;

  [SerializeField]
  private Material defaultMat;
  [SerializeField]
  private Material telekinesisMat;

  private bool dragging = false;

  public Transform test;

  private void Awake() {
    mainCam = Camera.main;
    renderer = GetComponent<MeshRenderer>();
    rigidbody = GetComponent<Rigidbody>();
  }

  private void OnMouseEnter() {
    renderer.material = telekinesisMat;
  }

  void OnMouseExit() {
    renderer.material = defaultMat;
  }

  private void OnMouseDown() {
    rigidbody.useGravity = false;
    dragging = true;
  }
  private void OnMouseUp() {

    if (dragging) {
      dragging = false;
      rigidbody.useGravity = true;
    }
  }

  void FixedUpdate() {
    if (dragging) {
      // TODO: move this into a global input manager or something class
      Ray mouseRay = mainCam.ScreenPointToRay(Input.mousePosition);
      Plane pp = new Plane(-Vector3.forward, Vector3.zero);
      pp.Raycast(mouseRay, out float rayDist);
      Vector3 velocityVector = mouseRay.GetPoint(rayDist) - transform.position;

      rigidbody.velocity = velocityVector.normalized * Mathf.Clamp(velocityVector.sqrMagnitude, 0f, 10f);
      print(rigidbody.velocity);
    }
  }
}