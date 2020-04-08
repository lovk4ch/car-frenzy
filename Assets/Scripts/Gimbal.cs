using UnityEngine;

public class Gimbal : Manager<Gimbal>
{
    private static Vector3 smoothScale;
    private float mouseX, mouseY;

    private GameObject target = null;

    [SerializeField]
    private float m_rotationSpeed = 100f;
    [SerializeField]
    private float m_speed = 3f;
    [SerializeField]
    private Transform cameraHolder = null;

    private void Awake()
    {
        target = LevelManager.Instance.Player.gameObject;
        smoothScale = cameraHolder.localPosition;
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        float delta = Time.deltaTime;

        if (target)
        {
            mouseX += InputManager.Instance.GetAxis(InputManager.MOUSE_X) * m_rotationSpeed * delta;
            mouseY -= InputManager.Instance.GetAxis(InputManager.MOUSE_Y) * m_rotationSpeed * delta;

            transform.position = target.transform.position;
            /*if (mouseY < 0)
                mouseY = 0;
            else
            if (mouseY > 30)
                mouseY = 30;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(mouseY, mouseX, 0),
                m_speed * delta);*/

            float change = InputManager.Instance.GetWheel();
            if (change != 0 && smoothScale.z + change <= -10 && smoothScale.z + change >= -40)
            {
                smoothScale += new Vector3(0, 0, change);
            }
            cameraHolder.localPosition = Vector3.Lerp(cameraHolder.localPosition, smoothScale, m_speed * delta);
        }
    }
}