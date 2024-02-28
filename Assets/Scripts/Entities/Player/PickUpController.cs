using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [SerializeField] private LayerMask _pickableLayer;
    [SerializeField] private Transform _pickUpContainer;

    private Rigidbody _pickerRigidbody;
    private Rigidbody _pickedRigidBody;
    private PlayerController _playerController;
    private Collider _collider;

    private bool _picked = false;

    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _pickerRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            if (!_picked)
            {
                PickUp();
            }
            else
            {
                Drop();
            }
                 
        }
    }

    private void PickUp()
    {
        var facingDir = _playerController.FacingDirection;
        var interactPos = _pickerRigidbody.transform.position + facingDir;

        //Debug.DrawLine(_pickerRigidbody.transform.position, interactPos, Color.red, 1f);

        Collider[] collider = Physics.OverlapSphere(interactPos, .1f, _pickableLayer);

        if (collider.Length != 0)
        {
            //Debug.Log("collision");
            _collider = collider[0];
            _pickedRigidBody = _collider.GetComponent<Rigidbody>();
            _pickedRigidBody.transform.SetParent(_pickUpContainer);
            _pickedRigidBody.transform.localPosition = Vector3.zero;
            _pickedRigidBody.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            _pickedRigidBody.transform.localScale = Vector3.one;

            _pickedRigidBody.isKinematic = true;
            _collider.isTrigger = true;

            //_collider.GetComponent<Animator>().SetBool("isPicked", true);
            _picked = true;
        }
    }

    private void Drop()
    {
        _pickedRigidBody.transform.SetParent(null);

        _pickedRigidBody.isKinematic = false;
        _collider.isTrigger = false;

       //_collider.GetComponent<Animator>().SetBool("isPicked", false);

        _picked = false;
    }
}
