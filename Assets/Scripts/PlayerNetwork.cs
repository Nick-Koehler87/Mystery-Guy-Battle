using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("Movement")]
    public Rigidbody rb;
    public Transform orientation;
    public float moveSpeed;
    Vector3 moveDir;
    public float rotationSpeed;
    public Vector3 inputDir;
    
    [Header("References")]
    public Transform player;
    public Transform playerObj;
    
    [Header("Cam/Sound")]
    [SerializeField] private CinemachineFreeLook vc;
    [SerializeField] private AudioListener audio;


    [Header("Spawning")]
    public string spawnpointTag = "SpawnPoint";

    // utility
    System.Random random = new System.Random();

    public override void OnNetworkSpawn () {
        if (IsOwner) {
            rb = GetComponent<Rigidbody>();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            audio.enabled = true;
            vc.Priority = 1;
            
            spawning();
            
        } else {
            vc.Priority = 0;
        }
        
    }

    private void FixedUpdate() {
        if (!IsOwner) return; 
        float horizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical"); 
        updateServerRpc(horizontalInput, VerticalInput);
    }

    private void myInput() {
        
    }

    [ServerRpc]
    private void updateServerRpc(float horizontalInput, float VerticalInput) {
        
        inputDir = orientation.forward * VerticalInput + orientation.right * horizontalInput;

        if (inputDir != Vector3.zero) {
            rb.AddForce(inputDir * moveSpeed * 10f, ForceMode.Force);
        }    
    }

    

    private void spawning() {
            GameObject[] spawnpoints = GameObject.FindGameObjectsWithTag(spawnpointTag);

            int randomNumber = random.Next(0, 2);

            transform.position = spawnpoints[randomNumber].transform.position;
    }

}
