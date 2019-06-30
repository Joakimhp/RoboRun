using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavour : MonoBehaviour {

    [Header("General variables")]
    public GameObject player;

    [Header("Sentry variables")]
    public Vector3 rotation;

    [Header("Chaser variables")]
    public CharacterController cc;
    public float chaseSpeed;
    public bool canChase;

    [Header("Back Wall Variables")]
    public Rigidbody rb;
    public Vector3 newPosition;
    public float runSpeed;
    public float maxAllowedDistanceToPlayer;
    //public float minAllowedDistanceToPlayer;
    public float minAllowedSpeedScale;
    public float MaxAllowedSpeedScale;

    private void Awake() {
        if (gameObject.tag == "Chaser") {
            cc = GetComponent<CharacterController>();
            canChase = false;
        }
        else if (gameObject.tag == "EnemyWall") {
            rb = GetComponent<Rigidbody>();
            newPosition = new Vector3(0, 0, runSpeed);
            if (maxAllowedDistanceToPlayer == 0)
                maxAllowedDistanceToPlayer = 1;
        }
        
            player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update() {
        if (GameManager.instance.charactersCanMove) {
            if (gameObject.tag == "Sentry") {
                Vector3 newRotation = new Vector3(rotation.x * Time.deltaTime, rotation.y * Time.deltaTime, rotation.z * Time.deltaTime) * 40;
                transform.Rotate(newRotation);
            }
            else if (gameObject.tag == "Chaser") {
                if (player != null && canChase) {
                    transform.LookAt(player.transform.position);

                    Vector3 playerpositionDirection = player.transform.position - transform.position;
                    playerpositionDirection.Normalize();
                    cc.Move(playerpositionDirection * chaseSpeed * Time.deltaTime);
                }
            }
            else if (gameObject.tag == "EnemyWall") {
                float playerSprintSpeed = player.GetComponent<PlayerMove>().sprintSpeed;
                
                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                float distanceToPlayerNorm = distanceToPlayer / maxAllowedDistanceToPlayer;

                newPosition.z = Mathf.Clamp(
                    playerSprintSpeed * distanceToPlayerNorm, 
                    playerSprintSpeed * minAllowedSpeedScale, 
                    playerSprintSpeed * MaxAllowedSpeedScale);
                
                rb.velocity = newPosition;
            }
        }
        else if (gameObject.tag == "EnemyWall") {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            GameManager.instance.KillPlayer(other.gameObject);
        }
    }
}
