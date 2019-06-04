using Mirror;
using UnityEngine;

public class Aim : NetworkBehaviour
{
    [SerializeField] private Texture2D m_crosshairImage;
    [SerializeField] private Transform m_gunPosition;

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            // Player
            Vector3 target = ray.GetPoint(distance);
            RotateTransform(target - transform.position, transform);

            // Gun
            RotateTransform(target - m_gunPosition.position, transform);
        }
    }

    private void RotateTransform(Vector3 direction, Transform toChange)
    {

        float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        toChange.rotation = Quaternion.Euler(0, rotation, 0);
    }

    private void OnGUI()
    {
        // Only draw crosshair when cursor isn't visible
        if (Cursor.visible)
        {
            return;
        }

        // draw on current mouse position
        float xMin = Input.mousePosition.x - (m_crosshairImage.width / 2);
        float yMin = (Screen.height - Input.mousePosition.y) - (m_crosshairImage.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, m_crosshairImage.width, m_crosshairImage.height), m_crosshairImage);
    }

    private void OnDestroy()
    {
        if (isLocalPlayer)
        {
            Cursor.visible = true;
        }
    }
}
