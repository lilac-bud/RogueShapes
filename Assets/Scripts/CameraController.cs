using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CinemachineConfiner2D m_Confiner;
    private PolygonCollider2D m_PolygonCollider;

    private void Awake()
    {
        m_Confiner = GetComponent<CinemachineConfiner2D>();
        m_PolygonCollider = (PolygonCollider2D)m_Confiner.BoundingShape2D;
    }

    public void UpdateCollider(int Width, int Height)
    {
        m_PolygonCollider.SetPath(0, new Vector2[] { new(0, 0), new(0, Width), new(Height, Width), new(Height, 0) });
        m_Confiner.InvalidateBoundingShapeCache();
    }
}