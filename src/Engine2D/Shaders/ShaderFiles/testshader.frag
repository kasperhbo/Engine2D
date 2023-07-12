#version 330 core

layout(location = 0) out vec4 fragColor;

in vec4  v_color;
in vec2  v_texCoords;
in float v_texID;

uniform sampler2D uTextures[8];

void main(){    
    vec4 texColor;
    // This may look bad, but it is intentional, openGL minimum spec does not require dynamic indexing with variables into texture arrays, so this switch is required on AMD GPUs.
    switch (int(v_texID)) {
        case 0:
         texColor = v_color;
        break;
        case 1:
         texColor = v_color * texture(uTextures[1], v_texCoords);
        break;
        case 2:
         texColor = v_color * texture(uTextures[2], v_texCoords);
        break;
        case 3:
         texColor = v_color * texture(uTextures[3], v_texCoords);
        break;
        case 4:
         texColor = v_color * texture(uTextures[4], v_texCoords);
        break;
        case 5:
         texColor = v_color * texture(uTextures[5], v_texCoords);
        break;
        case 6:
         texColor = v_color * texture(uTextures[6], v_texCoords);
        break;
        case 7:
         texColor = v_color * texture(uTextures[7], v_texCoords);
        break;
        default :
         texColor = v_color;
        break;
    }
        
     fragColor = texColor;
}