using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private int _moveSpeed, _zoomSpeed, _dragSpeed, _maxZoom;

    [SerializeField]
    private bool _invertX, _invertY;

    private Camera _camera;

    private Vector3 _lastFramePos;
    private EventManager _eventManager;

    private void Start() {
        _camera = Camera.main;

        _eventManager = new EventManager();
        _eventManager.RegisterEvent(CameraEvent.CameraMoved, transform.position);
    }

    private void Update() {
        Zoom();
        KeyboardMovement();
        MouseMovement();
    }

    private void Zoom() {
        var orthogSize = _camera.orthographicSize;

        orthogSize -= _camera.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed;

        _camera.orthographicSize = Mathf.Clamp(orthogSize, 1, _maxZoom);
    }

    private void MouseMovement() {
        if (Input.GetMouseButton(2)) {
            Vector3 currPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            MoveCamera(_lastFramePos - currPosition);
        }

        _lastFramePos = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void KeyboardMovement() {
        Vector2 movementVector = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
            movementVector += new Vector2(0, 1);
        if (Input.GetKey(KeyCode.S))
            movementVector += new Vector2(0, -1);
        if (Input.GetKey(KeyCode.A))
            movementVector += new Vector2(-1, 0);
        if (Input.GetKey(KeyCode.D))
            movementVector += new Vector2(1, 0);

        MoveCamera(movementVector.normalized * Time.deltaTime * _moveSpeed);
    }

    private void MoveCamera(Vector3 newPosition) {
        _camera.transform.Translate(newPosition);

        if (newPosition != Vector3.zero)
            _eventManager.RegisterEvent(CameraEvent.CameraMoved, transform.position);
    }
}