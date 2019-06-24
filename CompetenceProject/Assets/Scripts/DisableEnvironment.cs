using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableEnvironment : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Environment")
            other.gameObject.SetActive(false);
    }
}
