using Engine2D.Cameras;
using Engine2D.Components.TransformComponents;
using Engine2D.Core;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
using Engine2D.Managers;
using OpenTK.Mathematics;

namespace Engine2D.Rendering;

internal static class DebugDraw
{
    private static int MAX_LINES = 30000;

    private static List<Line2D> _lines = new();
    // 6 floats per vertex, 2 vertices per line
    private static readonly float[] VertexArray = new float[MAX_LINES * 6 * 2];
    private static readonly Shader Shader;

    private static readonly int VaoId;
    private static readonly int VboId;

    private static float _gridSize;

    
    static DebugDraw()
    {
        var dat = new ShaderData();
        
        dat.VertexPath = Utils.GetBaseEngineDir() + "\\Shaders\\ShaderFiles\\Line.vert";
        dat.FragPath = Utils.GetBaseEngineDir() + "\\Shaders\\ShaderFiles\\Line.frag";

        Shader = ResourceManager.GetShader(dat);
        
        VaoId = GL.GenVertexArray();
        GL.BindVertexArray(VaoId);

        // Create the vbo and buffer some memory
        VboId = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
        GL.BufferData(BufferTarget.ArrayBuffer, VertexArray.Length * sizeof(float), VertexArray, BufferUsageHint.DynamicDraw);

        // Enable the vertex array attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
         0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
         3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.LineWidth(2);
    }

    internal static void Render(Camera cam)
    {
        AddGridLines(cam);
        
        if (_lines.Count <= 0) return;
        
        RemoveDeadLines();

        
        int index = 0;
        foreach (Line2D line in _lines) {
            for (int i=0; i < 2; i++) {
                
                Vector2 position = i == 0 ? new(line.From.X, line.From.Y) : new(line.To.X, line.To.Y);
                
                Vector4 color = line.Color;
                
                // Load position
                VertexArray[index] = position.X;
                VertexArray[index + 1] = position.Y;
                VertexArray[index + 2] = -10.0f;

                // Load the color
                VertexArray[index + 3] = color.X;
                VertexArray[index + 4] = color.Y;
                VertexArray[index + 5] = color.Z;
                index += 6;
            }
        }
        
        GL.Enable(EnableCap.LineSmooth);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, VboId);
        
        GL.BufferData(BufferTarget.ArrayBuffer, VertexArray.Length * sizeof(float), VertexArray, BufferUsageHint.DynamicDraw);
        
        Shader.use();
        Shader.uploadMat4f("uProjection", cam.GetProjectionMatrix());
        Shader.uploadMat4f("uView", cam.GetViewMatrix());
        
        Shader.uploadFloat("baseGridSize", _gridSize);
        Shader.uploadFloat("lineThickness", 20);
     
        // Bind the vao
        GL.BindVertexArray(VaoId);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);

        // Draw the batch
        GL.DrawArrays(PrimitiveType.Lines, 0, _lines.Count);

        // Disable Location
        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindVertexArray(0);
    }

    private static void RemoveDeadLines()
    {
        for (int i=0; i < _lines.Count(); i++) {
            if (_lines[i].OnRender() < 0) {
                _lines.RemoveAt(i);
                i--;
            }
        }
    }

    private static void AddGridLines(Camera camera)
    {
        System.Numerics.Vector2 cameraPos = camera.Parent.GetComponent<Transform>().Position;
        System.Numerics.Vector2 projectionSize = camera.ProjectionSize;

        float gridSize = Settings.GRID_HEIGHT;

        // Calculate the zoom level for nested grids
        float zoomLevel = MathF.Floor(camera.Size);
        if(zoomLevel<1) zoomLevel = 1;
        
        float zoomedGridSize = gridSize * zoomLevel;
        
        DebugDraw._gridSize = zoomedGridSize;

        float firstX = MathF.Floor(cameraPos.X / zoomedGridSize) * zoomedGridSize;
        float firstY = MathF.Floor(cameraPos.Y / zoomedGridSize) * zoomedGridSize;

        firstX -= zoomedGridSize / 2;
        firstY -= zoomedGridSize / 2;

        int numVtLines = (int)MathF.Ceiling(projectionSize.X * camera.Size / zoomedGridSize) + 2;
        int numHzLines = (int)MathF.Ceiling(projectionSize.Y * camera.Size / zoomedGridSize) + 2;

        float width = MathF.Floor(projectionSize.X * camera.Size) + (5 * zoomedGridSize);
        float height = MathF.Floor(projectionSize.Y * camera.Size) + (5 * zoomedGridSize);

        int maxLines = Math.Max(numVtLines, numHzLines);
        Vector4 color = new Vector4(0.2f, 0.2f, 0.2f, 1f);

        for (int i = 0; i < maxLines; i++)
        {
            float x = firstX + (zoomedGridSize * i);
            float y = firstY + (zoomedGridSize * i);

            if (i < numVtLines)
            {
                AddLine2D(new Vector2(x, firstY), new Vector2(x, firstY + height), color, camera);
            }

            if (i < numHzLines)
            {
                AddLine2D(new Vector2(firstX, y), new Vector2(firstX + width, y), color, camera);
            }
        }
    }


    internal static void AddLine2D(Vector2 from, Vector2 to, Vector4 color, Camera camera) {
        AddLine2D(from, to, color, 1, camera);
    }

    internal static void AddLine2D(Vector2 from, Vector2 to, Vector4 color, int lifetime, Camera camera)
    {
        if (_lines.Count > MAX_LINES) return;
        DebugDraw._lines.Add(new Line2D(new(from.X, from.Y), new(to.X, to.Y), color, 3));
    }

}