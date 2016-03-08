#version 330
uniform sampler2D TextureUnit2; //vec2 Normal.xy,	vec2  Emissive.xy
uniform sampler2D TextureUnit3; //vec3 EyePosition,	float Emissive.z

in vec2 f_UV;

#include("Light.glsl")

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = texture(TextureUnit3, f_UV).rgb;
	
	vec3 Kd = vec3(1.0);
	vec3 Ke = vec3(0.0);
	vec3 Ks = vec3(1.0);
	vec3 Ka = vec3(1.0);
	float Shininess = 50.0;
	
	float Nx = texture(TextureUnit2, f_UV).r;
	float Ny = texture(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx * Nx - Ny * Ny));
	
	FragColor.rgb = CalcLight(Position, Normal, Kd, Ks, Ka, Ke, Shininess); //Light
	FragColor.a = 1.0;
}