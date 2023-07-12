#version 330 core

layout (location=0) in vec3 aPos;
layout (location=1) in vec4 aColor;
layout (location=2) in vec2 aTexCoords;
layout (location=3) in float aTexID;

uniform mat4 u_viewMatrix;
uniform mat4 u_projectionMatrix;

out vec4 v_color;
out vec2 v_texCoords;
out float v_texID;

void main(){
    vec4 pos =  u_projectionMatrix * u_viewMatrix * vec4(aPos, 1.0);

    v_texCoords = aTexCoords;    
    v_color = aColor;
    v_texID = aTexID;
    
    gl_Position = pos;    
}