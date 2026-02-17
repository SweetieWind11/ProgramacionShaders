﻿using System;                           // Tipos base de C# (Exception, etc.)
using OpenTK.Mathematics;               // Matrices, vectores, MathHelper
using OpenTK.Graphics.OpenGL4;          // Llamadas OpenGL (GL.*)
using OpenTK.Windowing.Common;          // Tipos de eventos (FrameEventArgs, ResizeEventArgs)
using OpenTK.Windowing.Desktop;         // GameWindow y configuración de ventana

// Clase estática con el punto de entrada del programa (Main)
internal static class Program
{
    private static void Main()
    {
        // Configuración del "game loop" (frecuencias por defecto, etc.)
        var gws = GameWindowSettings.Default;

        // Configuración de la ventana y del contexto OpenGL
        var nws = new NativeWindowSettings
        {
            Size = new Vector2i(900, 650),         // Tamaño de ventana en píxeles
            Title = "Cubo 3D con Perspectiva",     // Título de la ventana

            // Pide un contexto OpenGL 3.3 "Core Profile" (pipeline moderno)
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core,

            // ForwardCompatible: deshabilita funciones antiguas (fixed pipeline)
            Flags = ContextFlags.ForwardCompatible
        };

        // Crea la ventana (Game) y arranca el loop
        using var game = new Game(gws, nws);
        game.Run(); // Llama internamente a: OnLoad -> Update/Render en bucle -> OnUnload
    }
}

// Nuestra ventana/juego hereda de GameWindow (OpenTK maneja el loop por nosotros)
public sealed class Game : GameWindow
{
    private Texture _texture;
    // =========================
    // GEOMETRÍA: VÉRTICES
    // =========================
    // Cada vértice tiene 6 floats:
    //  - Posición: X, Y, Z  (3 floats)
    //  - Color:    R, G, B  (3 floats)
private readonly float[] _vertices =
{
    // Adelante
    -1, -1,  1,  0, 0,
     1, -1,  1,  1, 0,
     1,  1,  1,  1, 1,
    -1,  1,  1,  0, 1,

    // Atras
     1, -1, -1,  0, 0,
    -1, -1, -1,  1, 0,
    -1,  1, -1,  1, 1,
     1,  1, -1,  0, 1,

    // Izquierda
    -1, -1, -1,  0, 0,
    -1, -1,  1,  1, 0,
    -1,  1,  1,  1, 1,
    -1,  1, -1,  0, 1,

    // Derecha
     1, -1,  1,  0, 0,
     1, -1, -1,  1, 0,
     1,  1, -1,  1, 1,
     1,  1,  1,  0, 1,

    // Arriba
    -1,  1,  1,  0, 0,
     1,  1,  1,  1, 0,
     1,  1, -1,  1, 1,
    -1,  1, -1,  0, 1,

    // Abajo
    -1, -1, -1,  0, 0,
     1, -1, -1,  1, 0,
     1, -1,  1,  1, 1,
    -1, -1,  1,  0, 1,
};


    private readonly uint[] _indices =
    {
        0,1,2, 2,3,0,
        4,5,6, 6,7,4,
        8,9,10, 10,11,8,
        12,13,14, 14,15,12,
        16,17,18, 18,19,16,
        20,21,22, 22,23,20
    };
    
    // IDs de objetos OpenGL
    private int _vao; // Vertex Array Object: guarda el "layout" de atributos (cómo leer el VBO)
    private int _vbo; // Vertex Buffer Object: guarda los vértices en GPU
    private int _ebo; // Element Buffer Object: guarda los índices en GPU

    // Shader program y ubicación del uniform
    private int _shaderProgram; // Programa linkeado (vertex + fragment)
    private int _uMvpLocation;  // Ubicación en GPU del uniform "uMVP"

    // Animación / Cámara
    private float _angleDeg;      // Ángulo acumulado de rotación (en grados)
    private Matrix4 _projection;  // Matriz de proyección (perspectiva)
    private Mesh _mesh;
    private float _angle;
    private Shader _shader;


    // Constructor: llama al constructor base (GameWindow) con settings
    public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

    // =========================
    // ONLOAD: corre UNA VEZ
    // =========================

   
    protected override void OnLoad()
    {
        GL.ClearColor(0.08f, 0.09f, 0.12f, 1f);
        GL.Enable(EnableCap.DepthTest);

        _shader = new Shader("Shaders/cube.vert", "Shaders/cube.frag");
        _texture = new Texture("Textures/Evanescence.png");

        _shader.Use();
        _shader.SetInt("uTex", 0);

        _mesh = new Mesh(_vertices, _indices);

        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f,
            100f
        );
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        _angle += 60f * (float)e.Time;
        if (KeyboardState.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Escape))
            Close();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        Matrix4 model =
            Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_angle)) *
            Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_angle * 0.6f));

        Matrix4 view = Matrix4.CreateTranslation(0, 0, -5);
        Matrix4 mvp = model * view * _projection;

        _shader.Use();
        _shader.SetMatrix4("uMVP", mvp);

        _texture.Use(TextureUnit.Texture0);
        _mesh.Draw();

        SwapBuffers();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, Size.X, Size.Y);
        _projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45f),
            Size.X / (float)Size.Y,
            0.1f,
            100f
        );
    }

    protected override void OnUnload()
    {
        _mesh.Dispose();
        _shader.Dispose();
        _texture.Dispose();
    }
}