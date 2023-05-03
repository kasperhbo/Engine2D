#version 330 core

out vec4 color;

in vec2 fTexCoords;
in vec4 fColor;

uniform float uTime;
uniform sampler2D TEX_SAMPLER;

void main()
{
    color = texture(TEX_SAMPLER, fTexCoords);
}