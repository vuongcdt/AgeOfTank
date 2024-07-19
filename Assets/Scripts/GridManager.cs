using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float cellSize;

    private void Start()
    {
        RenderGrid();
    }

    private void RenderGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(GetPosition(x, y), GetPosition(x, y + 1), Color.white, 5000);
                Debug.DrawLine(GetPosition(x, y), GetPosition(x + 1, y), Color.white, 5000);
            }
        }

        Debug.DrawLine(GetPosition(0, height), GetPosition(width, height), Color.white, 5000);
        Debug.DrawLine(GetPosition(width, 0), GetPosition(width, height), Color.white, 5000);
    }

    private Vector3 GetPosition(float x, float y)
    {
        return new Vector3(x - width * 0.5f, y - height * 0.5f) * cellSize;
    }
}