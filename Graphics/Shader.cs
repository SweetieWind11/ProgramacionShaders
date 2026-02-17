using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.IO;

public sealed class Shader : IDisposable
{
    public int Handle { get; }

    public Shader(string vertexPath, string fragmentPath)
    {
        int vertex = Compile(ShaderType.VertexShader, File.ReadAllText(vertexPath));
        int fragment = Compile(ShaderType.FragmentShader, File.ReadAllText(fragmentPath));

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, vertex);
        GL.AttachShader(Handle, fragment);
        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int status);
        if (status == 0)
            throw new Exception(GL.GetProgramInfoLog(Handle));

        GL.DeleteShader(vertex);
        GL.DeleteShader(fragment);
    }

    private static int Compile(ShaderType type, string src)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, src);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int status);
        if (status == 0)
            throw new Exception(GL.GetShaderInfoLog(shader));

        return shader;
    }

    public void Use() => GL.UseProgram(Handle);

    public void SetMatrix4(string name, Matrix4 value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(loc, false, ref value);
    }

    public void SetInt(string name, int value)
    {
        GL.Uniform1(GL.GetUniformLocation(Handle, name), value);
    }

    public void Dispose() => GL.DeleteProgram(Handle);
}
    /*
    public void SetInt(string name, int value)
    {
        int loc = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(loc, value);
    }
    public void SetMatrix4(string name, OpenTK.Mathematics.Matrix4 value)
    {
        int loc = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(Handle, name);
        OpenTK.Graphics.OpenGL4.GL.UniformMatrix4(loc, false, ref value);
    }
    public void SetFloat(string name, float value)
    {
        int loc = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(Handle, name);
        OpenTK.Graphics.OpenGL4.GL.Uniform1(loc, value);
    }

    public void SetVector3(string name, OpenTK.Mathematics.Vector3 value)
    {
        int loc = OpenTK.Graphics.OpenGL4.GL.GetUniformLocation(Handle, name);
        OpenTK.Graphics.OpenGL4.GL.Uniform3(loc, value);
    }
    public void Use() => GL.UseProgram(Handle);

    public void Dispose() => GL.DeleteProgram(Handle);
   */ 

