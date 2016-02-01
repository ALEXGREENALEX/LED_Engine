#version 330
uniform sampler2D TextureUnit0; // Depth
uniform sampler2D TextureUnit1; //vec4 Diffuse
uniform sampler2D TextureUnit2; //vec2 Normal.xy,	vec2  Emissive.xy
uniform sampler2D TextureUnit3; //vec3 EyePosition,	float Emissive.z
uniform sampler2D TextureUnit4; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit5; //vec3 Ka * AO_Map,	FREE

in vec2 f_UV;

//uniform vec2 ScreenSize;
//uniform vec2 ClipPlanes; // zNear, zFar

#include("Light.glsl")
//#include("SSLR_1.glsl")
//#include("SSLR_2.glsl")
#include("SSAO.glsl")

layout(location = 0) out vec4 Output0; //Diffuse + Light
layout(location = 1) out vec4 Output1; //AO
layout(location = 2) out vec4 Output2; //SSAO

void main()
{
	vec3 Position = texture(TextureUnit3, f_UV).rgb;
	
	float Nx = texture(TextureUnit2, f_UV).r;
	float Ny = texture(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx * Nx - Ny * Ny));
	
	vec3 Kd = texture(TextureUnit1, f_UV).rgb; // Ignore Alpha
	vec3 Ke = vec3(texture(TextureUnit2, f_UV).ba, texture(TextureUnit3, f_UV).a);
	vec3 Ks = texture(TextureUnit4, f_UV).rgb;
	vec3 Ka = texture(TextureUnit5, f_UV).rgb;
	float Shininess = texture(TextureUnit4, f_UV).a;
	
	vec3 AO;
	Output0 = vec4(CalcLight(Position, Normal, Kd, Ks, Ka, Ke, Shininess, AO), 1.0); //Light
	Output1 = vec4(AO, 1.0);
/////////////////////////////////////////////////////////////////////////////////////////////////////
	//SSLR 1
	//vec4 R = SSLR(TextureUnit3, TextureUnit1, Position, Normal, Kd.b);
	//Output0.rgb = mix(Output0.rgb, R.rgb, R.a);
/////////////////////////////////////////////////////////////////////////////////////////////////////
	//SSLR 2
	//vec4 R = SSLR(TextureUnit0, TextureUnit1, f_UV, Normal);
	//Output0.rgb = R.rgb;
/////////////////////////////////////////////////////////////////////////////////////////////////////
	//SSAO
	float S = SSAO(TextureUnit3, f_UV, Position, Normal);
	Output2 = vec4(S, S, S, 1.0);
/////////////////////////////////////////////////////////////////////////////////////////////////////
}