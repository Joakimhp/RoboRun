using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempBehaviour : MonoBehaviour {

    Vector3 newRot;
    Animator animator;

    private void Start() {
        newRot = new Vector3();
        animator = gameObject.GetComponent<Animator>();
    }
    
    void Update () {
        transform.position += transform.forward * Time.deltaTime * 2;
        newRot.y = 50 * Time.deltaTime;
        transform.Rotate(newRot);
	}

    /*private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            animator.Play("Death");
            print("AAAAARGH!");
        }
    }*/

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            Destroy(gameObject);
        }
    }


}
