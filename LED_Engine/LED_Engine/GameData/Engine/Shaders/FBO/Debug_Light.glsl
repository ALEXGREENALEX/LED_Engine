#version 330
uniform sampler2D TextureUnit1; //vec4 Diffuse
uniform sampler2D TextureUnit2; //vec3 Normal.xy,	float EyePosition.xy
uniform sampler2D TextureUnit3; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit4; //vec3 Emissive,	float EyePosition.z
uniform sampler2D TextureUnit5; //vec3 Ka * AO_Map,	FREE

in vec2 f_UV;

#include("Light.glsl")

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = vec3(
		texture2D(TextureUnit2, f_UV).ba,
		texture2D(TextureUnit4, f_UV).a);
	
	/*float Nx = texture2D(TextureUnit2, f_UV).r;
	float Ny = texture2D(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx*Nx - Ny*Ny));*/
	vec3 Normal = vec3(0.0, 1.0, 0.0);
	
	vec3 Kd = vec3(1.0);
	vec3 Ks = texture2D(TextureUnit3, f_UV).rgb;
	vec3 Ke = vec3(0.0);
	vec3 Ka = texture2D(TextureUnit5, f_UV).rgb;
	float Shininess = texture2D(TextureUnit3, f_UV).a;
	
	FragColor.rgb = CalcLight(Kd, Ks, Ka, Ke, Shininess, Normal, Position); //Light
	FragColor.a = 1.0;
}