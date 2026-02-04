#version 330 core

in vec2 vUV;
out vec4 FragColor;

//Luz básica.
uniform sampler2D uTex;

uniform vec3 uLightDir; //Dirección hacia la luz.
uniform vec3 uLightColor; //Color hacia la luz.
uniform vec3 uBaseColor; //Color base del material.
uniform float uAmbient; //Luz Ambiente [0....1].

void main()
{
    //Pa pruebas definamos una normal que apunta hacia Z+
    vec3 N = vec3(0.0, 0.0, 1.0);

    //uLightDir dirección de la cara hacia la Luz.
    vec3 L = normalize(uLightDir);

    //Difusión: Cuanta luz pega según el angulo.
    float diff = max(dot(N,L), 0.0);

    //Intensidad final con ambiente.
    float intensity = clamp(uAmbient + diff, 0.0, 1.0);

    //Color del material = textura * tinte.
    vec3 tex = texture(uTex, vUV).rgb;
    vec3 base = tex * uBaseColor;

    //Iluminación 
    vec3 color = base * (uLightColor * intensity);

    FragColor = vec4(color, 1.0);
}