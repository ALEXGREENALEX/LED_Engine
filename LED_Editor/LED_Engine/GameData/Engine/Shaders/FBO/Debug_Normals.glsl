#version 330
uniform sampler2D TextureUnit2;

in vec2 f_UV;

#include("Light.glsl")

layout(location = 0) out vec4 FragColor;

void main()
{
	float Nx = texture(TextureUnit2, f_UV).r;
	float Ny = texture(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx*Nx - Ny*Ny));
	
	FragColor = vec4(Normal * 0.5 + 0.5, 1.0);
}