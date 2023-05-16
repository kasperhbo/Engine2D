#version 330 core

struct PointLight {
    vec2 position;
    vec3 lightColor;
    float intensity;
};

#define NR_POINT_LIGHTS 2
uniform PointLight pointLights[NR_POINT_LIGHTS];

in vec4 fColor;
in vec2 fTexCoords;
in float fTexId;

in vec3 fFragPos;

uniform sampler2D uTextures[32];

vec4 color;
out vec4 outColor;

vec3 ambient_light = vec3(0,0,0);

void main()
{
    if (fTexId > 0) {

        int id = int(fTexId);
        switch (id) {
            case 0:
            color = fColor * texture(uTextures[0], fTexCoords);
            break;
            case 1:
            color = fColor * texture(uTextures[1], fTexCoords);
            break;
            case 2:
            color = fColor * texture(uTextures[2], fTexCoords);
            break;
            case 3:
            color = fColor * texture(uTextures[3], fTexCoords);
            break;
            case 4:
            color = fColor * texture(uTextures[4], fTexCoords);
            break;
            case 5:
            color = fColor * texture(uTextures[5], fTexCoords);
            break;
            case 6:
            color = fColor * texture(uTextures[6], fTexCoords);
            break;
            case 7:
            color = fColor * texture(uTextures[7], fTexCoords);
            break;
            case 8:
            color = fColor * texture(uTextures[8], fTexCoords);
            break;
            case 9:
            color = fColor * texture(uTextures[9], fTexCoords);
            break;
            case 10:
            color = fColor * texture(uTextures[10], fTexCoords);
            break;

            case 11:
            color = fColor * texture(uTextures[11], fTexCoords);
            break;
            case 12:
            color = fColor * texture(uTextures[12], fTexCoords);
            break;
            case 13:
            color = fColor * texture(uTextures[13], fTexCoords);
            break;
            case 14:
            color = fColor * texture(uTextures[14], fTexCoords);
            break;
            case 15:
            color = fColor * texture(uTextures[15], fTexCoords);
            break;
            case 16:
            color = fColor * texture(uTextures[16], fTexCoords);
            break;
            case 17:
            color = fColor * texture(uTextures[17], fTexCoords);
            break;
            case 18:
            color = fColor * texture(uTextures[18], fTexCoords);
            break;
            case 19:
            color = fColor * texture(uTextures[19], fTexCoords);
            break;

            case 20:
            color = fColor * texture(uTextures[20], fTexCoords);
            break;
            case 21:
            color = fColor * texture(uTextures[21], fTexCoords);
            break;
            case 22:
            color = fColor * texture(uTextures[22], fTexCoords);
            break;
            case 23:
            color = fColor * texture(uTextures[23], fTexCoords);
            break;
            case 24:
            color = fColor * texture(uTextures[24], fTexCoords);
            break;
            case 25:
            color = fColor * texture(uTextures[25], fTexCoords);
            break;
            case 26:
            color = fColor * texture(uTextures[26], fTexCoords);
            break;
            case 27:
            color = fColor * texture(uTextures[27], fTexCoords);
            break;
            case 28:
            color = fColor * texture(uTextures[28], fTexCoords);
            break;
            case 29:
            color = fColor * texture(uTextures[29], fTexCoords);
            break;
            case 30:
            color = fColor * texture(uTextures[30], fTexCoords);
            break;
            case 31:
            color = fColor * texture(uTextures[31], fTexCoords);
            break;
        }
    } else {
        color = fColor;
    }
    
    vec3 LColor  = vec3(0,0,0);
    
    float diffuse =0;
    
    for(int i = 0; i < NR_POINT_LIGHTS; i++){
        float distance = length(pointLights[i].position.xy - fFragPos.xy);
        
        if(color.a <= 0)
            discard;

        if (distance <= pointLights[i].intensity)
            diffuse +=  1.0 - abs(distance / pointLights[i].intensity);
        
        LColor += pointLights[i].lightColor.rgb;
    }


    outColor = vec4(min(color.rgb * ((LColor * diffuse) + ambient_light), color.rgb), 1.0);
//    vec4 final = color.rgba + LColor.rgba;
//
//    outColor = final;
//    
//    color = color + LColor;
//    outColor = color;
}