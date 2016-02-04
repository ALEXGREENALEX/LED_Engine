#version 330
uniform sampler2D TextureUnit0; //Depth
uniform sampler2D TextureUnit1; //vec4 Diffuse
uniform sampler2D TextureUnit2; //vec2 Normal.xy,	vec2  Emissive.xy
uniform sampler2D TextureUnit3; //vec3 EyePosition,	float Emissive.z
uniform sampler2D TextureUnit4; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit5; //vec3 Ka * AO_Map,	FREE

in vec2 f_UV;

//uniform vec2 ScreenSize;
//uniform vec2 ClipPlanes; // zNear, zFar
uniform mat4 ViewMatrixInv;

#include("Light.glsl")
#include("Fog.glsl")

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = texture(TextureUnit3, f_UV).rgb;
	vec3 WorldPosition = vec3(ViewMatrixInv * vec4(Position, 1.0));
	
	float Nx = texture(TextureUnit2, f_UV).r;
	float Ny = texture(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx * Nx - Ny * Ny));
	
	vec3 Kd = texture(TextureUnit1, f_UV).rgb; // Ignore Alpha
	vec3 Ke = vec3(texture(TextureUnit2, f_UV).ba, texture(TextureUnit3, f_UV).a);
	vec3 Ks = texture(TextureUnit4, f_UV).rgb;
	vec3 Ka = texture(TextureUnit5, f_UV).rgb;
	float Shininess = texture(TextureUnit4, f_UV).a;
	
	FragColor.rgb = CalcLight(Position, Normal, Kd, Ks, Ka, Ke, Shininess);
	FragColor.rgb = Fog(FragColor.rgb, Position, WorldPosition); //Fog
	FragColor.a = sqrt(dot(FragColor.rgb, vec3(0.299, 0.587, 0.114))); //Luma for FXAA
}