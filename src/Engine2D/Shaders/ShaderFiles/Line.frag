#version 330 core

out vec4 fragColor;

uniform float gridSize;
uniform float lineThickness;

void main()
{
    vec2 uv = gl_FragCoord.xy;
    vec2 gridUV = fract(uv / gridSize);

    // Set the grid line color
    vec3 lineColor = vec3(0.2, 0.2, 0.2);

    // Calculate the grid lines
    float horzLine = step(lineThickness, gridUV.y) * step(gridUV.y, 1.0 - lineThickness);
    float vertLine = step(lineThickness, gridUV.x) * step(gridUV.x, 1.0 - lineThickness);

    // Combine the horizontal and vertical lines
    float gridLines = max(horzLine, vertLine);

    // Set the line thickness
    float thickness = 3.0;
    float antiAlias = lineThickness / thickness;

    // Apply anti-aliasing to smooth the lines
    float smoothLines = smoothstep(1.0 - antiAlias, 1.0, gridLines);

    // Mix the line color with the background color
    vec3 backgroundColor = vec3(1.0, 1.0, 1.0); // Adjust as needed
    vec3 finalColor = mix(backgroundColor, lineColor, smoothLines);

    fragColor = vec4(finalColor, 1.0);
}
