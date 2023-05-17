#version 330

out vec4 outColor;

in vec2 texCoord;

uniform sampler2D lightTexture;
uniform sampler2D sceneTexture;

void main()
{
    vec4 tempColor = vec4(1,0,0,1);
    vec4 lightColor = texture2D(lightTexture, texCoord);
    vec4 sceneColor = texture2D(sceneTexture, texCoord);
    
    vec4 endColor = sceneColor * vec4(lightColor.r, lightColor.g, lightColor.b, 1);
        
    outColor = endColor;
}
 //   outputColor = mix(texture(texture0, texCoord), texture(texture1, texCoord), 0.2);