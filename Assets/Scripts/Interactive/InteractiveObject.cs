using System.Collections;
using System.Collections.Generic;
using OFR.Psycho.Utils;
using UnityEditor;
using UnityEngine;

namespace OFR.Psycho.Interactive {

  public class InteractiveObject : MonoBehaviour {
    protected MeshRenderer renderer = null;
    protected Camera mainCam;

    [SerializeField]
    protected Material defaultMat;
    [SerializeField]
    protected Material hoverMat;

    protected bool interactable = true;
    protected bool isInteracting = false;

    protected void Awake() {
      mainCam = Camera.main;
      renderer = GetComponent<MeshRenderer>();
    }

    void OnMouseEnter() {
      if (interactable)
        SetInteractiveMat();

    }

    void OnMouseExit() {
      if (!isInteracting)
        ResetMat();
    }

    void OnMouseDown() {
      if (interactable)
        Interact();
    }

    void OnMouseUp() {
      if (isInteracting) {
        EndInteract();
      }
    }

    protected virtual void Interact() {
      isInteracting = true;
    }

    protected virtual void EndInteract() {
      isInteracting = false;
      ResetMat();
    }

    private void SetInteractiveMat() {
      renderer.material = hoverMat;
    }

    private void ResetMat() {
      renderer.material = defaultMat;
    }
  }
}