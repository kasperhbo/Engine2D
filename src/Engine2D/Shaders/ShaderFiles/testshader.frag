#version 330 core

layout(location = 0) out vec4 fragColor;

in vec4 v_color;
in vec2 v_texCoords;
in float v_texID;


void main(){
    fragColor = v_color;
}