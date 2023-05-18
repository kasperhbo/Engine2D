#version 330 core

in vec2 fPos;
in vec4 fColor;
in vec2 fTexCoords;
in float fTexId;

#define MAX_LIGHTS 300
uniform vec2 uLightPosition[MAX_LIGHTS];
uniform vec3 uLightColour[MAX_LIGHTS];
uniform float uIntensity[MAX_LIGHTS];
uniform float uMinLighting = 1;
uniform int uNumLights;

uniform sampler2D uTextures[8];

out vec4 color;

float distance(vec2 a, vec2 b) {
    vec2 c = b - a;
    return c.x * c.x + c.y * c.y;
}

float calculateLighting(float d, float intensity) {
    return 1.0 / (1.0 + (0.001 / intensity) * d);
}

void main () {
    vec4 texColor;

    vec3 totalLighting = vec3(0.0);
    for (int i = 0; i < uNumLights; i++) {
        float dist = distance(uLightPosition[i], fPos);
        float attenuation = calculateLighting(dist, uIntensity[i]);
        totalLighting += uLightColour[i] * attenuation;
    }
    //    totalLighting += vec3(1.0f) * uMinLighting;

    totalLighting.x = max(totalLighting.x, uMinLighting);
    totalLighting.y = max(totalLighting.y, uMinLighting);
    totalLighting.z = max(totalLighting.z, uMinLighting);
    
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
        default:
        texColor = fColor;
    }

    texColor *= vec4(totalLighting, 1.0);

    color = texColor;
}