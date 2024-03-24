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
    //public Vector3 inputDir;

    // Start is called before the first frame update
    public override void OnNetworkSpawn () {
        
        Cursor.lockState =  CursorLockMode.Locked;
        Cursor.visible = false;
        
    }

    // Update is called once per frame
    private void FixedUpdate() { 
        
        if (!IsOwner) return; 
        Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
        orientationServerRpc(viewDir);
        float horizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");    
        //inputDir = 
        updateServerRpc(horizontalInput, VerticalInput);
        /*if (inputDir != Vector3.zero) {
           
        } */  

        
    } 
    [ServerRpc]
    private void updateServerRpc(float horizontalInput, float VerticalInput) {   
        
        Vector3 inputDir = orientation.forward * VerticalInput + orientation.right * horizontalInput;
        playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
    }

    [ServerRpc]
    private void orientationServerRpc(Vector3 viewDir) {
        orientation.forward = viewDir.normalized;
    }
}
