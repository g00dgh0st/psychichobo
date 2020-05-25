using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEditor;
using UnityEngine;

public class InteractiveObject : MonoBehaviour {
  // NOTE: place holder
  private MeshRenderer renderer;
  private Rigidbody rigidbody;
  private Camera mainCam;

  [SerializeField]
  private Material defaultMat;
  [SerializeField]
  private Material telekinesisMat;

  private bool interactable = true;

  private void Awake() {
    mainCam = Camera.main;
    renderer = GetComponent<MeshRenderer>();
    rigidbody = GetComponent<Rigidbody>();
  }

  private void OnMouseEnter() {
    if (interactable)
      renderer.material = telekinesisMat;
  }

  void OnMouseExit() {
    renderer.material = defaultMat;
  }

  private void OnMouseDown() {
    if (interactable)
      Interact();
  }

  private void OnMouseUp() {
    EndInteract();
  }

  void Interact() {
    interactable = false;
    rigidbody.isKinematic = false;
  }

  void EndInteract() {}
}