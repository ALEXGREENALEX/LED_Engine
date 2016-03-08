#version 330
uniform sampler2D TextureUnit2;
uniform sampler2D TextureUnit3;
uniform sampler2D TextureUnit5;
uniform sampler2D TextureUnit6;

#include("SSAO.glsl")

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = texture(TextureUnit3, f_UV).rgb;
	
	float Nx = texture(TextureUnit2, f_UV).r;
	float Ny = texture(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx * Nx - Ny * Ny));
	
	vec3 Ka = texture(TextureUnit5, f_UV).rgb;
	float SSAO_Factor = SSAO(TextureUnit3, TextureUnit2, TextureUnit6, f_UV, Position, Normal);
	FragColor = vec4(Ka * SSAO_Factor, 1.0);
	//FragColor = vec4(vec3(SSAO_Factor), 1.0);
}