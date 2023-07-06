#version 330 core

in vec2 fPos;
in vec4 fColor;
in vec2 fTexCoords;
in float fTexId;

uniform sampler2D uTextures[8];
//uniform sampler2D uLightmap;

out vec4 color;

void main () {
    vec4 texColor;

    // This may look bad, but it is intentional, openGL minimum spec does not require dynamic indexing with variables into texture arrays, so this switch is required on AMD GPUs.
    switch (int(fTexId)) {
        case 0:
        texColor = fColor;
        break;
        case 1:
        texColor = fColor * texture(uTextures[1], fTexCoords);
        break;
        case 2:
        texColor = fColor * texture(uTextures[2], fTexCoords);
        break;
        case 3:
        texColor = fColor * texture(uTextures[3], fTexCoords);
        break;
        case 4:
        texColor = fColor * texture(uTextures[4], fTexCoords);
        break;
        case 5:
        texColor = fColor * texture(uTextures[5], fTexCoords);
        break;
        case 6:
        texColor = fColor * texture(uTextures[6], fTexCoords);
        break;
        case 7:
        texColor = fColor * texture(uTextures[7], fTexCoords);
        break;
        default :
        texColor = fColor;
        break;
    }
    //vec4 tempColor = vec4(0,0,0,texColor.a);
    // Sample from lightmap and multiply with current fragment color
   // texColor *= texture(uLightmap, (fPos + 1)/2);
    color = texColor;//vec4(texColor.rgba);
}