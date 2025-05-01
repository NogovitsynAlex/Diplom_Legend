using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 2f, -3.5f);
    public float sensitivityX = 200f;
    public float sensitivityY = 150f;
    public float smoothSpeed = 0.05f;
    public float minYAngle = -40f;
    public float maxYAngle = 80f;

    private float rotX = 0f;
    private float rotY = 0f;

    private Vector3 currentVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Vector3 angles = transform.eulerAngles;
        rotX = angles.y;
        rotY = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        float mouseX = Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        rotX += mouseX;
        rotY -= mouseY;
        rotY = Mathf.Clamp(rotY, minYAngle, maxYAngle);

        Quaternion rotation = Quaternion.Euler(rotY, rotX, 0);
        Vector3 desiredPosition = target.position + rotation * offset;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothSpeed);
        transform.LookAt(target.position + Vector3.up * 1.5f); // Чтобы не смотреть в ноги
    }
}
