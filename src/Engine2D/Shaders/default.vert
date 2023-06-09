#version 330 core

layout (location=0) in vec2 aPos;
layout (location=1) in vec4 aColor;
layout (location=2) in vec2 aTexCoords;
layout (location=3) in float aTexId;

uniform mat4 uProjection;
uniform mat4 uView;

out vec2 fPos;
out vec4 fColor;
out vec2 fTexCoords;
out float fTexId;

void main() {
    vec4 pos = uProjection *uView* vec4(aPos, 0.0, 1.0);
    fPos = pos.xy;
    fColor = aColor;
    fTexCoords = aTexCoords;
    fTexId = aTexId;

    gl_Position = pos;
}