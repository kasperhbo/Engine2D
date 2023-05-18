uniform sampler2D u_image;

varying vec2 v_texCoords;
varying vec4 v_position;

uniform vec3 ambient_light; // set as (0.3, 0.3, 0.3), night!

vec2 point_light_pos = vec2(-0.4, 0.3);
vec3 point_light_col = vec3(0.999, 0.999, 0.999);
float point_light_intensity = 0.4;

void main()
{
    vec4 frag_color = texture2D(u_image, v_texCoords);
    if(frag_color.a < 1.0)
        discard;

    float distance = distance(point_light_pos, v_position.xy);
    float diffuse = 0.0;

    if (distance <= point_light_intensity)
    diffuse =  1.0 - abs(distance / point_light_intensity);

    gl_FragColor = vec4(min(frag_color.rgb * ((point_light_col * diffuse) + ambient_light), frag_color.rgb), 1.0);
}