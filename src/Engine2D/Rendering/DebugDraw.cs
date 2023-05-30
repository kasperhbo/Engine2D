using Engine2D.Core;
using Engine2D.Rendering.Lines;
using KDBEngine.Shaders;
using OpenTK.Graphics.OpenGL4;
using Engine2D.GameObjects;
using Engine2D.Logging;
using Engine2D.Testing;
using OpenTK.Mathematics;

namespace Engine2D.Rendering;

public class DebugDraw
{
    private static int MAX_LINES = 30000;

    private static List<Line2D> lines = new();
    // 6 floats per vertex, 2 vertices per line
    private static float[] vertexArray = new float[MAX_LINES * 6 * 2];
    private static Shader shader;

    private static int vaoID;
    private static int vboID;

    
    public void Init()
    {
        var dat = new ShaderData();
        
        dat.VertexPath = Utils.GetBaseEngineDir() + "\\Shaders\\ShaderFiles\\Line.vert";
        dat.FragPath = Utils.GetBaseEngineDir() + "\\Shaders\\ShaderFiles\\Line.frag";

        shader = ResourceManager.GetShader(dat);
        
        vaoID = GL.GenVertexArray();
        GL.BindVertexArray(vaoID);

        // Create the vbo and buffer some memory
        vboID = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
        GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float), vertexArray, BufferUsageHint.DynamicDraw);

        // Enable the vertex array attributes
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
         0);
        GL.EnableVertexAttribArray(0);

        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float),
         3 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        GL.LineWidth(2);
    }
    
    
    
    
    public void Render(TestCamera cam)
    {
        if (lines.Count <= 0) return;
        
        RemoveDeadLines();

        int index = 0;
        foreach (Line2D line in lines) {
            for (int i=0; i < 2; i++) {
                
                Vector2 position = i == 0 ? new(line.From.X, line.From.Y) : new(line.To.X, line.To.Y);
                
                KDBColor color = line.Color;
                
                // Load position
                vertexArray[index] = position.X;
                vertexArray[index + 1] = position.Y;
                vertexArray[index + 2] = -10.0f;

                // Load the color
                vertexArray[index + 3] = color.r;
                vertexArray[index + 4] = color.g;
                vertexArray[index + 5] = color.b;
                index += 6;
            }
        }
        
        GL.Enable(EnableCap.LineSmooth);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, vboID);
        
        GL.BufferData(BufferTarget.ArrayBuffer, vertexArray.Length * sizeof(float), vertexArray, BufferUsageHint.DynamicDraw);
        
        shader.use();
        shader.uploadMat4f("uProjection", cam.getProjectionMatrix());
        shader.uploadMat4f("uView", cam.getViewMatrix());

        // Bind the vao
        GL.BindVertexArray(vaoID);
        GL.EnableVertexAttribArray(0);
        GL.EnableVertexAttribArray(1);

        // Draw the batch
        GL.DrawArrays(PrimitiveType.Lines, 0, lines.Count);

        // Disable Location
        GL.DisableVertexAttribArray(0);
        GL.DisableVertexAttribArray(1);
        GL.BindVertexArray(0);
    }

    private void RemoveDeadLines()
    {
        for (int i=0; i < lines.Count(); i++) {
            if (lines[i].OnRender() < 0) {
                lines.RemoveAt(i);
                i--;
            }
        }
    }
    
    public void AddLine2D(Vector2 from, Vector2 to, KDBColor color, TestCamera camera) {
        AddLine2D(from, to, color, 1, camera);
    }

    public void AddLine2D(Vector2 from, Vector2 to, KDBColor color, int lifetime, TestCamera camera)
    {
        if (lines.Count > MAX_LINES) return;
        DebugDraw.lines.Add(new Line2D(new(from.X, from.Y), new(to.X, to.Y), color, 3));
    }

}