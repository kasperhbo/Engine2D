#version 330

out vec4 outColor;

in vec2 texCoord;

uniform sampler2D lightTexture;
uniform sampler2D sceneTexture;

uniform float uGlobalLightIntensity = .3;

vec4 end;

void main()
{
    vec4 lightColor = texture2D(lightTexture, texCoord);
    vec4 sceneColor = texture2D(sceneTexture, texCoord);
    
    float lightIntensity = lightColor.r;
    lightIntensity += uGlobalLightIntensity;
    outColor = vec4(sceneColor.rgb, lightIntensity);
}
 //   outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);