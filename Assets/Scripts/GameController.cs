using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject _girlPlayer;
    [SerializeField] private GameObject _boyPlayer;
    [SerializeField] private GameObject _girlCamera;
    [SerializeField] private GameObject _boyCamera;

    [SerializeField] private PlayerController _girlController;
    [SerializeField] private PlayerController _boyController;
    [SerializeField] private InputActionAsset _playerActions;
    [SerializeField] private PickUpController _girlPickUpController;
    [SerializeField] private PickUpController _boyPickUpController;

    private void Start()
    {
        _girlController.OnSwitchCharacter += () =>
        {
            Debug.Log("switch to boy");

            _girlController.enabled = false;
            _girlPickUpController.enabled = false;
            _boyController.enabled = true;
            _boyPickUpController.enabled = true;

            Destroy(_girlPlayer.GetComponent<PlayerInput>());
            PlayerInput _newInput = _boyPlayer.AddComponent<PlayerInput>();
            _newInput.actions = _playerActions;
            _newInput.actions.Enable();

            _girlCamera.SetActive(false);
            _boyCamera.SetActive(true);           
        };

        _boyController.OnSwitchCharacter += () =>
        {
            Debug.Log("switch to girl");

            _boyController.enabled = false;
            _boyPickUpController.enabled = false;
            _girlController.enabled = true;
            _girlPickUpController.enabled = true;

            Destroy(_boyPlayer.GetComponent<PlayerInput>());
            PlayerInput _newInput = _girlPlayer.AddComponent<PlayerInput>();
            _newInput.actions = _playerActions;
            _newInput.actions.Enable();

            _boyCamera.SetActive(false);
            _girlCamera.SetActive(true);
        };
    }
}
