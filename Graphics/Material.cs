using OpenTK.Graphics.OpenGL4;

public sealed class Material
{
    public Shader Shader { get; }
    public Texture Texture { get; }

    public Material (Shader shader, Texture texture)
    {
        Shader = shader;
        Texture = texture;
    }

    public void Bind()
    {
        Shader.Use();
            
        if (Texture != null)
        {
            
            Texture.Use(TextureUnit.Texture0);

            Shader.SetInt("uTex", 0);
        
        }
    }
}