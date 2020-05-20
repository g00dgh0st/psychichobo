using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

  [SerializeField]
  private float moveSpeed;

  private Vector3 targetPosition;
  private Vector3 startPosition;

  private Rigidbody rBody;

  void Awake() {
    targetPosition = transform.Find("TargetPosition").position;
    startPosition = transform.position;
    rBody = GetComponent<Rigidbody>();
  }

  void Start() {
    rBody.velocity = (targetPosition - startPosition).normalized * moveSpeed;
  }

  void Update() {
    if (Vector3.Distance(transform.position, targetPosition) <= 0.2f) {
      Vector3 holder = startPosition;
      startPosition = targetPosition;
      targetPosition = holder;

      // rBody.velocity = (targetPosition - startPosition).normalized * moveSpeed;
    }
  }
}