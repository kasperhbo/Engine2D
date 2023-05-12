#version 330 core
layout (location=0) in vec3 aPos;
layout (location=1) in vec4 aColor;
layout (location=2) in vec2 aTexCoords;
layout (location=3) in 
float aTexId;

uniform mat4 uProjection;
uniform mat4 uView;

uniform vec3 uPointLightPos;

out vec4 fColor;
out vec2 fTexCoords;
out float fTexId;

out vec4 fPosition;
out vec4 fLightPos;

void main()
{
    fColor = aColor;
    fTexCoords = aTexCoords;
    fTexId = aTexId;
    
    fPosition =  vec4(aPos, 1.0);
    fLightPos =  vec4(uPointLightPos, 1.0);    
    
    gl_Position = uProjection * uView * vec4(aPos, 1.0);
}