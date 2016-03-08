#version 330
uniform sampler2D TextureUnit0; //Depth
uniform sampler2D TextureUnit1; //vec4 Diffuse
uniform sampler2D TextureUnit2; //vec2 Normal.xy,	vec2  Emissive.xy
uniform sampler2D TextureUnit3; //vec3 EyePosition,	float Emissive.z
uniform sampler2D TextureUnit4; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit5; //vec3 AO,			float Reflection factor
uniform sampler2D TextureUnit6; //vec3 RandNormals

in vec2 f_UV;

//uniform vec2 ScreenSize;
//uniform vec2 ClipPlanes; // zNear, zFar
uniform mat4 ViewMatrixInv;

#include("Light.glsl")
#include("SSAO.glsl")
#include("Fog.glsl")

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = texture(TextureUnit3, f_UV).rgb;
	vec3 WorldPosition = vec3(ViewMatrixInv * vec4(Position, 1.0));
	
	float Nx = texture(TextureUnit2, f_UV).r;
	float Ny = texture(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx * Nx - Ny * Ny));
	
	vec3 Kd = texture(TextureUnit1, f_UV).rgb; //Ignore Alpha
	vec3 Ke = vec3(texture(TextureUnit2, f_UV).ba, texture(TextureUnit3, f_UV).a);
	vec3 Ks = texture(TextureUnit4, f_UV).rgb;
	vec3 Ka = texture(TextureUnit5, f_UV).rgb;
	float Shininess = texture(TextureUnit4, f_UV).a;
	float ReflectionFactor = texture(TextureUnit5, f_UV).a;
	
	float SSAO_Factor = SSAO(TextureUnit3, TextureUnit2, TextureUnit6, f_UV, Position, Normal);
	//FragColor.rgb = vec3(SSAO_Factor, SSAO_Factor, SSAO_Factor);
	//SSAO_Factor = 1.0; //This Disable SSAO
	
	vec3 FragColorLight = CalcLight(Position, Normal, Kd, Ks, Ka * SSAO_Factor, Ke, Shininess);
	FragColor.rgb = mix(FragColorLight, Kd, ReflectionFactor);
	FragColor.rgb *= SSAO_Factor;
	FragColor.rgb = Fog(FragColor.rgb, Position, WorldPosition); //Fog
	
	FragColor.a = sqrt(dot(FragColor.rgb, vec3(0.299, 0.587, 0.114))); //Luma for FXAA
}