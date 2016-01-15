#version 330
uniform samplerCube TextureUnit0;

smooth in vec3 f_UV;

layout (location = 0) out vec4 FragColor;

void main()
{
	FragColor = texture(TextureUnit0, f_UV);
}
