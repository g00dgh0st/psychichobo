using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightOscillator : MonoBehaviour {

  [SerializeField]
  private float startDelay = 1f;
  [SerializeField]
  private float oscillateTime = 1f;

  private Light light;
  private float intensity;

  void Awake() {
    light = GetComponent<Light>();
  }

  void Start() {
    intensity = light.intensity;
  }

  void Update() {

  }

}