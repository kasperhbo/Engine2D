using System;
using System.IO;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;


namespace KDBEngine.Shaders
{
    // A simple class meant to help create shaders.
    public class Shader
    {
        //public readonly int Handle;

        //private readonly Dictionary<string, int> _uniformLocations;

        //// This is how you create a simple shader.
        //// Shaders are written in GLSL, which is a language very similar to C in its semantics.
        //// The GLSL source is compiled *at runtime*, so it can optimize itself for the graphics card it's currently being used on.
        //// A commented example of GLSL can be found in shader.vert.
        //public Shader(string vertPath, string fragPath)
        //{

        //    // There are several different types of shaders, but the only two you need for basic rendering are the vertex and fragment shaders.
        //    // The vertex shader is responsible for moving around vertices, and uploading that data to the fragment shader.
        //    //   The vertex shader won't be too important here, but they'll be more important later.
        //    // The fragment shader is responsible for then converting the vertices to "fragments", which represent all the data OpenGL needs to draw a pixel.
        //    //   The fragment shader is what we'll be using the most here.

        //    // Load vertex shader and compile
        //    var shaderSource = File.ReadAllText(vertPath);

        //    // GL.CreateShader will create an empty shader (obviously). The ShaderType enum denotes which type of shader will be created.
        //    var vertexShader = GL.CreateShader(ShaderType.VertexShader);

        //    // Now, bind the GLSL source code
        //    GL.ShaderSource(vertexShader, shaderSource);

        //    // And then compile
        //    CompileShader(vertexShader, vertPath);

        //    // We do the same for the fragment shader.
        //    shaderSource = File.ReadAllText(fragPath);
        //    var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        //    GL.ShaderSource(fragmentShader, shaderSource);
        //    CompileShader(fragmentShader, fragPath);

        //    // These two shaders must then be merged into a shader program, which can then be used by OpenGL.
        //    // To do this, create a program...
        //    Handle = GL.CreateProgram();

        //    // Attach both shaders...
        //    GL.AttachShader(Handle, vertexShader);
        //    GL.AttachShader(Handle, fragmentShader);

        //    // And then link them together.
        //    LinkProgram(Handle);

        //    // When the shader program is linked, it no longer needs the individual shaders attached to it; the compiled code is copied into the shader program.
        //    // Detach them, and then delete them.
        //    GL.DetachShader(Handle, vertexShader);
        //    GL.DetachShader(Handle, fragmentShader);
        //    GL.DeleteShader(fragmentShader);
        //    GL.DeleteShader(vertexShader);

        //    // The shader is now ready to go, but first, we're going to cache all the shader uniform locations.
        //    // Querying this from the shader is very slow, so we do it once on initialization and reuse those values
        //    // later.

        //    // First, we have to get the number of active uniforms in the shader.
        //    GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        //    // Next, allocate the dictionary to hold the locations.
        //    _uniformLocations = new Dictionary<string, int>();

        //    // Loop over all the uniforms,
        //    for (var i = 0; i < numberOfUniforms; i++)
        //    {
        //        // get the name of this uniform,
        //        var key = GL.GetActiveUniform(Handle, i, out _, out _);

        //        // get the location,
        //        var location = GL.GetUniformLocation(Handle, key);

        //        // and then add it to the dictionary.
        //        _uniformLocations.Add(key, location);
        //    }
        //}

        //private static void CompileShader(int shader, string path)
        //{
        //    // Try to compile the shader
        //    GL.CompileShader(shader);

        //    // Check for compilation errors
        //    GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
        //    if (code != (int)All.True)
        //    {
        //        // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
        //        var infoLog = GL.GetShaderInfoLog(shader);

        //        throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog} at path: {path} ");
        //    }
        //}

        //private static void LinkProgram(int program)
        //{
        //    // We link the program
        //    GL.LinkProgram(program);

        //    // Check for linking errors
        //    GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
        //    if (code != (int)All.True)
        //    {
        //        // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
        //        throw new Exception($"Error occurred whilst linking Program({program})");
        //    }
        //}

        //// A wrapper function that enables the shader program.
        //public void Use()
        //{
        //    GL.UseProgram(Handle);
        //}

        //// The shader sources provided with this project use hardcoded layout(location)-s. If you want to do it dynamically,
        //// you can omit the layout(location=X) lines in the vertex shader, and use this in VertexAttribPointer instead of the hardcoded values.
        //public int GetAttribLocation(string attribName)
        //{
        //    return GL.GetAttribLocation(Handle, attribName);
        //}

        //// Uniform setters
        //// Uniforms are variables that can be set by user code, instead of reading them from the VBO.
        //// You use VBOs for vertex-related data, and uniforms for almost everything else.

        //// Setting a uniform is almost always the exact same, so I'll explain it here once, instead of in every method:
        ////     1. Bind the program you want to set the uniform on
        ////     2. Get a handle to the location of the uniform with GL.GetUniformLocation.
        ////     3. Use the appropriate GL.Uniform* function to set the uniform.

        ///// <summary>
        ///// Set a uniform int on this shader.
        ///// </summary>
        ///// <param name="name">The name of the uniform</param>
        ///// <param name="data">The data to set</param>
        //public void SetInt(string name, int data)
        //{
        //    GL.UseProgram(Handle);
        //    GL.Uniform1(_uniformLocations[name], data);
        //}

        ///// <summary>
        ///// Set a uniform float on this shader.
        ///// </summary>
        ///// <param name="name">The name of the uniform</param>
        ///// <param name="data">The data to set</param>
        //public void SetFloat(string name, float data)
        //{
        //    GL.UseProgram(Handle);
        //    GL.Uniform1(_uniformLocations[name], data);
        //}

        ///// <summary>
        ///// Set a uniform Matrix4 on this shader
        ///// </summary>
        ///// <param name="name">The name of the uniform</param>
        ///// <param name="data">The data to set</param>
        ///// <remarks>
        /////   <para>
        /////   The matrix is transposed before being sent to the shader.
        /////   </para>
        ///// </remarks>
        //public void SetMatrix4(string name, Matrix4 data)
        //{
        //    GL.UseProgram(Handle);
        //    GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        //}

        ///// <summary>
        ///// Set a uniform Vector3 on this shader.
        ///// </summary>
        ///// <param name="name">The name of the uniform</param>
        ///// <param name="data">The data to set</param>
        //public void SetVector3(string name, Vector3 data)
        //{
        //    GL.UseProgram(Handle);
        //    GL.Uniform3(_uniformLocations[name], data);
        //}

        //internal void Detach()
        //{
        //    GL.UseProgram(0);
        //}
                
        private readonly string fragmentSource;
        private int shaderProgramID;

        private readonly string vertexSource;

        public Shader(string vertexFilePath, string fragmentFilePath)
        {
            vertexSource = File.ReadAllText(vertexFilePath);
            fragmentSource = File.ReadAllText(fragmentFilePath);
            Compile(vertexSource, fragmentSource);
        }

        public void Compile(string vertexSource, string fragmentSource)
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

            shaderProgramID = GL.CreateProgram();
            GL.AttachShader(shaderProgramID, vertexID);
            GL.AttachShader(shaderProgramID, fragmentID);
            GL.LinkProgram(shaderProgramID);

            GL.GetProgram(shaderProgramID, ProgramParameter.LinkStatus, out succes);
            if (succes != (int)All.True)
                // We can use `GL.GetShaderInfoLog(shader)` to get information about the error.
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred whilst linking Program({shaderProgramID})");
        }

        public void use()
        {
            GL.UseProgram(shaderProgramID);
        }

        public void detach()
        {
            GL.UseProgram(0);
        }

        public void uploadMat4f(string varName, Matrix4 mat4)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);
            GL.UniformMatrix4(varLocation, false, ref mat4);
        }

        public void uploadMat3f(string varName, Matrix3 mat3)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.UniformMatrix3(varLocation, false, ref mat3);
        }

        public void uploadVec4f(string varName, Vector4 vec)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.Uniform4(varLocation, vec);
        }

        public void uploadVec3f(string varName, Vector3 vec)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.Uniform3(varLocation, vec);
        }

        public void uploadVec2f(string varName, Vector2 vec)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.Uniform2(varLocation, vec);
        }

        public void uploadFloat(string varName, float val)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.Uniform1(varLocation, val);
        }

        public void uploadInt(string varName, int val)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.Uniform1(varLocation, val);
        }

        public void uploadTexture(string varName, int slot)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, varName);

            GL.Uniform1(varLocation, slot);
        }

        internal void UploadIntArray(string v, int[] values)
        {
            use();
            var varLocation = GL.GetUniformLocation(shaderProgramID, v);
            GL.Uniform1(varLocation, values.Length, values);            
        }
    }
}