using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : NetworkBehaviour
{
    [SerializeField] private float _moveRate = 4f;
    private CharacterController _characterController;

    public override void OnStartLocalPlayer()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // take input from focused window only
        if (!Application.isFocused) return;

        // movement for local player
        if (isLocalPlayer)
        {
            float delta = Time.deltaTime;
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 forces = new Vector3(horizontal, Physics.gravity.y, vertical) * _moveRate;
            _characterController.Move(forces * delta);
        }
    }
}
