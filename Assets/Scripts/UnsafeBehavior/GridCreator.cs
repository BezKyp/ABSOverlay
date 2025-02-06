using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridCreator : Graphic
{
    public Vector2Int gridSize = new Vector2Int(1, 1);  // Grid size
    public float thickness = 10f;  // Line thickness

    public int redLineIndex = 3;  // Row index for the red horizontal line (can be set in the Inspector)
    public Color redLineColor = Color.red;  // Customizable color for the red line

    float width;
    float height;

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        // Get the dimensions of the RectTransform in local space of the Canvas
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        // Calculate the spacing between the grid lines
        float cellWidth = width / (float)gridSize.x;
        float cellHeight = height / (float)gridSize.y;

        // Draw vertical lines
        for (int x = 0; x <= gridSize.x; x++) // +1 to draw line at the far right edge
        {
            DrawLine(x * cellWidth, 0, x * cellWidth, height, vh, color); // Use the Graphic color for vertical lines
        }

        // Draw horizontal lines
        for (int y = 0; y <= gridSize.y; y++) // +1 to draw line at the far bottom edge
        {
            // Check if the current row is the one specified to be red
            Color lineColor = (y == redLineIndex) ? redLineColor : color;  // Use custom red color or default color
            DrawLine(0, y * cellHeight, width, y * cellHeight, vh, lineColor);
        }
    }

    private void DrawLine(float x1, float y1, float x2, float y2, VertexHelper vh, Color lineColor)
    {
        // Create the 4 corners for a rectangle (thick line)
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = lineColor;

        Vector2 direction = new Vector2(x2 - x1, y2 - y1).normalized;
        Vector2 perpendicular = new Vector2(-direction.y, direction.x) * (thickness / 2);  // Perpendicular vector for thickness

        // Calculate the 4 corners of the rectangle (line with thickness)
        Vector3 p0 = new Vector3(x1, y1) - (Vector3)perpendicular;
        Vector3 p1 = new Vector3(x1, y1) + (Vector3)perpendicular;
        Vector3 p2 = new Vector3(x2, y2) + (Vector3)perpendicular;
        Vector3 p3 = new Vector3(x2, y2) - (Vector3)perpendicular;

        // Add the vertices for the line rectangle
        vertex.position = p0;
        vh.AddVert(vertex);
        vertex.position = p1;
        vh.AddVert(vertex);
        vertex.position = p2;
        vh.AddVert(vertex);
        vertex.position = p3;
        vh.AddVert(vertex);

        // Add the triangles (two triangles to form a rectangle)
        int vertCount = vh.currentVertCount;
        vh.AddTriangle(vertCount - 4, vertCount - 3, vertCount - 2);
        vh.AddTriangle(vertCount - 4, vertCount - 2, vertCount - 1);
    }
}
