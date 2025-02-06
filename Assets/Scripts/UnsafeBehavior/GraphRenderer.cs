using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphRenderer : Graphic
{
    public GridCreator gridCreator;  // Reference to the GridCreator component
    public float barWidth = 50f;  // Width of the bar (adjust as needed)
    public Color barColor = Color.green;  // Default bar color

    private float latestGaitSpeed = 0f; // Stores the most recent gait speed value

    float width;
    float height;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        if (gridCreator == null)
        {
            Debug.LogError("GridCreator reference is missing!");
            return;
        }

        // Calculate bar height based on the latest gait speed
        float barHeight = Mathf.Clamp((latestGaitSpeed / gridCreator.gridSize.y) * height, 0, height);

        // Change bar color if above a threshold
        Color currentBarColor = (barHeight > height / gridCreator.gridSize.y) ? Color.red : barColor;

        // Draw a single bar
        DrawBar(vh, barHeight, currentBarColor);
    }

    private void DrawBar(VertexHelper vh, float barHeight, Color color)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        float halfBarWidth = barWidth / 2;
        float barX = width / 2; // Center the bar horizontally

        // Define four corners of the bar
        Vector3 p0 = new Vector3(barX - halfBarWidth, 0); // Bottom-left
        Vector3 p1 = new Vector3(barX + halfBarWidth, 0); // Bottom-right
        Vector3 p2 = new Vector3(barX + halfBarWidth, barHeight); // Top-right
        Vector3 p3 = new Vector3(barX - halfBarWidth, barHeight); // Top-left

        // Add vertices
        vertex.position = p0; vh.AddVert(vertex);
        vertex.position = p1; vh.AddVert(vertex);
        vertex.position = p2; vh.AddVert(vertex);
        vertex.position = p3; vh.AddVert(vertex);

        int vertCount = vh.currentVertCount;

        // Create two triangles for the rectangle (bar)
        vh.AddTriangle(vertCount - 4, vertCount - 3, vertCount - 2);
        vh.AddTriangle(vertCount - 4, vertCount - 2, vertCount - 1);
    }

    public void AddGaitSpeed(float speed)
    {
        latestGaitSpeed = speed;  // Store the latest value
        SetVerticesDirty(); // Forces UI to update
    }
}
