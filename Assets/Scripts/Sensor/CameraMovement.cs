using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public Slider moveSpeedSlider;
    public Slider rotationSpeedSlider;

    float moveSpeed = 5.0f;
    float rotationSpeed = 200.0f;

    // Update is called once per frame
    void Update()
    {        
        Vector3 movement = new Vector3();
        float scrollValue = Input.GetAxis("Mouse ScrollWheel");

        // Horizontal movement (A/D or LeftArrow/RightArrow)
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            movement.x -= moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            movement.x += moveSpeed * Time.deltaTime;

        // Forward/Backward movement 
        if (scrollValue > 0)
            movement.z += moveSpeed * Time.deltaTime * 3;

        if (scrollValue < 0)
            movement.z -= moveSpeed * Time.deltaTime * 3;


        // Vertical movement
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            movement.y += moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            movement.y -= moveSpeed * Time.deltaTime;

        // Use TransformDirection to get a direction relative to the camera's orientation.
        movement = transform.TransformDirection(movement);

        transform.Translate(movement, Space.World);

        // Rotate camera using the mouse while holding the right mouse button.
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseX, Space.World);
            transform.Rotate(Vector3.left * mouseY, Space.Self);
        }
    }

    public void SetMoveSpeed()
    {
        moveSpeed = moveSpeedSlider.value;
    }

    public void SetRotationSpeed()
    {
        rotationSpeed = rotationSpeedSlider.value * 40f;
    }
}
