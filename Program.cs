using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;

class Game : GameWindow
{

    // ID used by OpenGL.
    private int _vao; // Vertex array object -> Cómo se leen los vértices.
    private int _vbo; // Vertex buffer object -> Datos crudos de los vértices.
    private int _ebo; // Element buffer object -> índices y el orden de cada vértice.
    private float _time;

    private Shader _shader;
    
    private Texture _texture;
    private Texture _texture2;
    private int _vao2; // Vertex array object -> Cómo se leen los vértices.
    private int _vbo2; // Vertex buffer object -> Datos crudos de los vértices.
    private int _ebo2; // Element buffer object -> índices y el orden de cada vértice.
    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    protected override void OnLoad()
    {
        base.OnLoad();

        GL.ClearColor(0.1f, 0.1f, 0.15f, 1f);

        // 4 flotantes por vertice (x,y) & (u,v)
        float[] vertices =
        {
            -0.5f,  0.5f,  0f, 1f,  // v0: Izq Arriba
             0.5f,  0.5f,  1f, 1f,  // v1: Der Arriba
            -0.5f, -0.5f,  0f, 0f,  // v2: Izq Abajo
             0.5f, -0.5f,  1f, 0f,  // v3: Der Abajo 
        };
        float [] vertices2 =
        {
            0.5f,  0.5f,  0f, 1f,  // v0: Izq Arriba
            1.5f,  0.5f,  1f, 1f,  // v1: Der Arriba
            0.5f, -0.5f,  0f, 0f,  // v2: Izq Abajo
            1.5f, -0.5f,  1f, 0f,  // v3: Der Abajo 
        };
        // Determinamos el order para usar los vértices.
        uint[] indices =
        {
            0, 2, 1, // T01
            2, 3, 1  // T02
        };


        // Creamos los objetos de OpenGL
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        _vao2 = GL.GenVertexArray();
        _vbo2 = GL.GenBuffer();
        _ebo2 = GL.GenBuffer();

        // Activamos el VAO.
        GL.BindVertexArray(_vao);

        // Para el VBO, hacemos los "Bind" / enlaces de datos con la GOU y los shaders.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

        // EBO
        // Contiene los índices de cómo leer los vértices. El EBO se guarda en el VAO.
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // VAO sabe cómo leer el VBO
        // Atributo 1: Posiciones

        int stride = 4 * sizeof(float);
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        // Atributo 2: color
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false,  stride, 2 * sizeof(float));
        GL.EnableVertexAttribArray(1);

        //Shaders (vertex con MVP wich means Modelo Vista y Proyección) 
        _shader = new Shader("Shaders/textured_mvp.vert", "Shaders/textured.frag");

        //Texture
        _texture = new Texture("Textures/Lenna.png");


        //testUli
        GL.BindVertexArray(_vao2);
                // Para el VBO, hacemos los "Bind" / enlaces de datos con la GOU y los shaders.
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo2);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices2, BufferUsageHint.StaticDraw);

        // EBO
        // Contiene los índices de cómo leer los vértices. El EBO se guarda en el VAO.
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo2);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

        // VAO sabe cómo leer el VBO
        // Atributo 1: Posiciones
        GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
        GL.EnableVertexAttribArray(0);

        // Atributo 2: color
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false,  stride, 2 * sizeof(float));
        GL.EnableVertexAttribArray(1);
        _texture2 = new Texture("Textures/UltraInstict.png");


        //Conectar sampler con texture unit 0
        _shader.Use();
        _shader.SetInt("uText", 0);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        base.OnUpdateFrame(e);
        //tiempo acumulado (segundos)
        _time += (float)e.Time;
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        GL.Clear(ClearBufferMask.ColorBufferBit);

        //Construir matrices
        var model = 
            Matrix4.CreateRotationZ(_time) *
            Matrix4.CreateScale(1.0f);

            var model2 = 
            Matrix4.CreateRotationZ(_time * -1)  *
            Matrix4.CreateScale(1.0f);
        //View: Identidad (no cámara todavia)
        var view = Matrix4.Identity;

        //Projection: Ortho para 2D (encaja bien con quad en [-1,1])
        //left, right, bottom, top, zNear,zFar
        var proj = Matrix4.CreateOrthographicOffCenter(-1, 1f, -1f, 1f, -1f, 1f);

        //Orden típico para OpenGL: MVP = model * view * proj o proj * view * model según convención.
        //Con esta configuración (y el shader uMVP * vec4), suele ir bien con:
        var mvp = model * view * proj;
        var mvp2 = model2 * view * proj;

        //2) Enviar uniform al shader
        _shader.Use();
        _shader.SetMatrix4("uMVP", mvp);

        //3) Bind textura en texture0 y dibujar
        _texture.Use(TextureUnit.Texture0);

        // VAO ya trae el VBO + EBO = formato
        GL.BindVertexArray(_vao);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);

        _shader.Use();
        _shader.SetMatrix4("uMVP", mvp2);
        
        _texture2.Use(TextureUnit.Texture0);
        GL.BindVertexArray(_vao2);
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        SwapBuffers();
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        GL.DeleteBuffer(_ebo);
        GL.DeleteBuffer(_vbo);
        GL.DeleteVertexArray(_vao);
        _shader.Dispose();
    }
}

class Program
{
    static void Main()
    {
        var gws = GameWindowSettings.Default;
        var nws = new NativeWindowSettings
        {
            Title = "E02 - Triangle",
            Size = new Vector2i(800, 600),
            API = ContextAPI.OpenGL,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,
            Flags = ContextFlags.ForwardCompatible
        };

        using var game = new Game(gws, nws);
        game.Run();
    }
}
