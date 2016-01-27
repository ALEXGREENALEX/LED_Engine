#version 330
uniform sampler2D TextureUnit0; // Depth
uniform sampler2D TextureUnit1; //vec4 Diffuse
uniform sampler2D TextureUnit2; //vec2 Normal.xy,	vec2  Emissive.xy
uniform sampler2D TextureUnit3; //vec3 EyePosition,	float Emissive.z
uniform sampler2D TextureUnit4; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit5; //vec3 Ka * AO_Map,	FREE

in vec2 f_UV;

#include("Light.glsl")

layout(location = 0) out vec4 Output0; //Diffuse + Light
layout(location = 1) out vec4 Output1; //AO
layout(location = 2) out vec4 Output2; //SSAO

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
	
	vec3 AO;
	Output0.rgb = CalcLight(Position, Normal, Kd, Ks, Ka, Ke, Shininess, AO); //Light
	Output0.rgb += AO;
	Output0.a = 1.0;
	Output1 = vec4(0.0);
	Output2 = vec4(0.0);
}