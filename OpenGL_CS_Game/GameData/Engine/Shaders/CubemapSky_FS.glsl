#version 430

smooth in vec3 CubemapTexCoords;

layout (binding = 0) uniform samplerCube CubemapTex;
layout (location = 0) out vec4 FragColor;

void main()
{
    FragColor = texture(CubemapTex, CubemapTexCoords);
}
