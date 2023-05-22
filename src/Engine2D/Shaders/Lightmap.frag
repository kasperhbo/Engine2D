#version 330 core

in vec2 fPos;

/**
 * The lighting uniform variables.
 * MAX_LIGHTS can be changed here. If you want more lights make sure to change the RenderBatch::addPointLight fuction as well.
 */
#define MAX_LIGHTS 10
uniform vec2 uLightPosition[MAX_LIGHTS];
uniform vec3 uLightColor[MAX_LIGHTS];
uniform float uIntensity[MAX_LIGHTS];
uniform float uMinLighting;
uniform int uNumLights;

out vec4 color;

float distance(vec2 a, vec2 b) {
    vec2 c = b - a;
    return c.x * c.x + c.y * c.y;
}

float calculateLighting(float d, float intensity) {
    return 1.0 / (1.0 + (0.001 / intensity) * d);
}

void main () {
    // Total lighting accumulation variable
    vec3 totalLighting = vec3(0.0);
    // Eventhough the arrays are crated with MAX_LIGHTS size, the arrays are iterated over only [uNumLights] times
    for (int i = 0; i < uNumLights; i++) {
        // Distance between the current pixel and the light position
        float dist = distance(uLightPosition[i], fPos);
        // calculate brightness using the attenuation function
        float attenuation = calculateLighting(dist, uIntensity[i]);
        // accumulate the value into total lighting by adding
        totalLighting += uLightColor[i] * attenuation;
    }
    // Take minimum lighting into account
    totalLighting.x = max(totalLighting.x, uMinLighting);
    totalLighting.y = max(totalLighting.y, uMinLighting);
    totalLighting.z = max(totalLighting.z, uMinLighting);

    color = vec4(totalLighting, 1.0);
}