using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    Vector3 mousePosition;
    Rigidbody rb;
    Camera mainCamera;
    float rotationSpeed = 200f;

    private void Start()
    {
        rb = transform.GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    private Vector3 GetMousePos()
    {
        return mainCamera.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePos();
    }

    private void OnMouseDrag()
    {
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        rb.MovePosition(newPosition);
        rb.velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.Z))
        {
            rb.transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        else if (Input.GetKey(KeyCode.X))
        {
            rb.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
