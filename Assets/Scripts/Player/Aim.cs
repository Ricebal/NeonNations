using UnityEngine;
using UnityEngine.Networking;

public class Aim : NetworkBehaviour
{
    public Texture2D CrosshairImage;

    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
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
            Vector3 target = ray.GetPoint(distance);
            Vector3 direction = target - transform.position;
            float rotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
    }

    private void OnGUI()
    {
        // Only draw crosshair when cursor isn't visible
        if (Cursor.visible)
        {
            return;
        }

        // draw on current mouse position
        float xMin = Input.mousePosition.x - (CrosshairImage.width / 2);
        float yMin = (Screen.height - Input.mousePosition.y) - (CrosshairImage.height / 2);
        GUI.DrawTexture(new Rect(xMin, yMin, CrosshairImage.width, CrosshairImage.height), CrosshairImage);
    }
}
