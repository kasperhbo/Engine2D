#version 330

out vec4 outColor;

in vec2 texCoord;

uniform sampler2D lightTexture;
uniform sampler2D sceneTexture;

void main()
{
    vec4 lightColor = texture2D(lightTexture, texCoord);
    vec4 sceneColor = texture2D(sceneTexture, texCoord);

    outColor = sceneColor * vec4(lightColor.r, lightColor.g, lightColor.b, 1);
    
}
 //   outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);