using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Netcode;
using UnityEngine;


public class thirdPersonCam : NetworkBehaviour
{
    
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    GameObject playerObject;
    public Rigidbody rb;
    public float rotationSpeed;

    // Start is called before the first frame update
    public override void OnNetworkSpawn () {
        
        Cursor.lockState =  CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    private void FixedUpdate() { 
        
        if (!IsOwner) return; 
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        float horizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");  
        float move = horizontalInput + VerticalInput;
        if (move != 0) {
            updateServerRpc(horizontalInput, VerticalInput, viewDir);
        } 

        
    } 
    [ServerRpc]
    private void updateServerRpc(float horizontalInput, float VerticalInput, Vector3 viewDir) {   
        
        orientation.forward = viewDir.normalized;
        Vector3 inputDir = orientation.forward * VerticalInput + orientation.right * horizontalInput;
        playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

    [ServerRpc]
    private void orientationServerRpc(Vector3 viewDir) {
    }
}
