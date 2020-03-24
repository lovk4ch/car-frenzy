using UnityEngine;

public class Consts : MonoBehaviour
{
    public static Vector3 GetProjection(Vector3 position)
    {
        return new Vector3(position.x, 0, position.z);
    }
}