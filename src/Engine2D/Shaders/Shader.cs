#region

using System.Numerics;
using Engine2D.UI.Debug;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

#endregion

namespace KDBEngine.Shaders;

// A simple class meant to help create shaders.
internal class Shader
{
    public readonly string FragmentSource;
    public readonly string VertexSource;
    
    public int ShaderProgramId { get; private set; }

    internal Shader(string vertexFilePath, string fragmentFilePath)
    {
        DebugStats.LoadedShaders++;
        VertexSource = File.ReadAllText(vertexFilePath);
        FragmentSource = File.ReadAllText(fragmentFilePath);
        Compile(VertexSource, FragmentSource);
    }

    internal void Compile(string vertexSource, string fragmentSource)
    {
        int vertexID, fragmentID;

        vertexID = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexID, vertexSource);
        GL.CompileShader(vertexID);

        GL.GetShader(vertexID, ShaderParameter.CompileStatus, out var succes);
        if (succes != (int)All.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = GL.GetShaderInfoLog(vertexID);
            throw new Exception($"Error occurred whilst compiling Shader({vertexID}).\n\n{infoLog}");
        }

        fragmentID = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentID, fragmentSource);
        GL.CompileShader(fragmentID);

        GL.GetShader(fragmentID, ShaderParameter.CompileStatus, out succes);
        if (succes != (int)All.True)
        {
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            var infoLog = GL.GetShaderInfoLog(fragmentID);
            throw new Exception($"Error occurred whilst compiling Shader({fragmentID}).\n\n{infoLog}");
        }

        ShaderProgramId = GL.CreateProgram();
        GL.AttachShader(ShaderProgramId, vertexID);
        GL.AttachShader(ShaderProgramId, fragmentID);
        GL.LinkProgram(ShaderProgramId);

        GL.GetProgram(ShaderProgramId, ProgramParameter.LinkStatus, out succes);
        if (succes != (int)All.True)
            // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
            // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
            throw new Exception($"Error occurred whilst linking Program({ShaderProgramId})");
    }

    internal void use()
    {
        GL.UseProgram(ShaderProgramId);
    }

    internal void detach()
    {
        GL.UseProgram(0);
    }

    internal void uploadMat4f(string varName, Matrix4 mat4)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);
        GL.UniformMatrix4(varLocation, false, ref mat4);
    }

    internal void uploadMat3f(string varName, Matrix3 mat3)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);

        GL.UniformMatrix3(varLocation, false, ref mat3);
    }

    internal void uploadVec4f(string varName, Vector4 vec)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);

        GL.Uniform4(varLocation, vec);
    }

    internal void uploadVec3f(string varName, Vector3 vec)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);

        GL.Uniform3(varLocation, vec);
    }

    internal void uploadVec2f(string varName, Vector2 vec)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);
        GL.Uniform2(varLocation, vec);
    }

    /// <summary>
    ///     Overload for System.numerics
    /// </summary>
    /// <param name="varName"></param>
    /// <param name="vec"></param>
    internal void uploadVec2f(string varName, System.Numerics.Vector2 vec)
    {
        uploadVec2f(varName, new Vector2(vec.X, vec.Y));
    }

    internal void uploadFloat(string varName, float val)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);

        GL.Uniform1(varLocation, val);
    }

    internal void uploadInt(string varName, int val)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);

        GL.Uniform1(varLocation, val);
    }

    internal void uploadTexture(string varName, int slot)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);
        GL.Uniform1(varLocation, slot);
    }

    internal void UploadIntArray(string v, int[] values)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, v);
        GL.Uniform1(varLocation, values.Length, values);
    }

    internal void uploadVec2fArray(string varName, Vector2[] vec)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);
        var vals = new float[vec.Length * 2];
        for (var i = 0; i < vec.Length; i++)
        {
            vals[i * 2] = vec[i].X;
            vals[i * 2 + 1] = vec[i].Y;
        }

        GL.Uniform2(varLocation, vec.Length, vals);
    }

    internal void uploadVec2fArray(string varName, System.Numerics.Vector2[] vec)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);
        var vals = new float[vec.Length * 2];
        for (var i = 0; i < vec.Length; i++)
        {
            vals[i * 2] = vec[i].X;
            vals[i * 2 + 1] = vec[i].Y;
        }

        GL.Uniform2(varLocation, vec.Length, vals);
    }


    internal void uploadVec3fArray(string varName, Vector3[] vec)
    {
        var varLocation = GL.GetUniformLocation(ShaderProgramId, varName);
        use();
        var vals = new float[vec.Length * 3];
        for (var i = 0; i < vec.Length; i++)
        {
            vals[i * 3] = vec[i].X;
            vals[i * 3 + 1] = vec[i].Y;
            vals[i * 3 + 2] = vec[i].Z;
        }

        GL.Uniform3(varLocation, vec.Length, vals);
    }

    internal int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(ShaderProgramId, attribName);
    }

    internal void uploadFloatArray(string v, float[] values)
    {
        use();
        var varLocation = GL.GetUniformLocation(ShaderProgramId, v);
        GL.Uniform1(varLocation, values.Length, values);
    }

    internal void uploadMat4f(string varName, Matrix4x4 v)
    {
        var val = new Matrix4(
            new Vector4(v.M11, v.M12, v.M13, v.M14),
            new Vector4(v.M21, v.M22, v.M23, v.M24),
            new Vector4(v.M31, v.M32, v.M33, v.M34),
            new Vector4(v.M41, v.M42, v.M43, v.M44)
        );

        uploadMat4f(varName, val);
    }
}