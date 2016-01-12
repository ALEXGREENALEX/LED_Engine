#version 330

smooth in vec3 Cubemap_UV;

uniform samplerCube CubemapTex;

layout (location = 0) out vec4 FragColor;

void main()
{
    FragColor = texture(CubemapTex, Cubemap_UV);
}
