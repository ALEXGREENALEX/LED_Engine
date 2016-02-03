#version 330
uniform sampler2D TextureUnit0; // Depth
uniform sampler2D TextureUnit1; // Position
uniform sampler2D TextureUnit2; // Diffuse + Light
uniform sampler2D TextureUnit3; // AO
uniform sampler2D TextureUnit4; // SSAO

in vec2 f_UV;

//uniform vec2 ScreenSize;
uniform mat4 ViewMatrixInv;
//uniform vec2 ClipPlanes; // zNear, zFar

//#include("SSLR_1.glsl")
//#include("SSLR_2.glsl")
#include("SSAO.glsl")
#include("Glow.glsl")
#include("Fog.glsl")

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Position = texture(TextureUnit1, f_UV).rgb;
	vec3 WorldPosition = vec3(ViewMatrixInv * vec4(Position, 1.0));
	
	vec3 DiffuseLight = texture(TextureUnit2, f_UV).rgb;
	vec3 AO = texture(TextureUnit3, f_UV).rgb;
	
	vec3 Summ = DiffuseLight + AO * SSAO_Blur(TextureUnit4, f_UV, 4.0) + Glow(TextureUnit2, f_UV, 8.0);
	FragColor.rgb = Fog(Summ, Position, WorldPosition); //Fog
	
/////////////////////////////////////////////////////////////////////////////////////////////////////
	//SSLR 1
	//vec4 R = SSLR(TextureUnit3, TextureUnit1, Position, Normal, Kd.b);
	//FragColor.rgb = mix(FragColor.rgb, R.rgb, R.a);
/////////////////////////////////////////////////////////////////////////////////////////////////////
	//SSLR 2
	//vec4 R = SSLR(TextureUnit0, TextureUnit1, f_UV, Normal);
	//FragColor.rgb = R.rgb;
/////////////////////////////////////////////////////////////////////////////////////////////////////
	FragColor.a = sqrt(dot(FragColor.rgb, vec3(0.299, 0.587, 0.114))); //Luma for FXAA
	//FragColor.a = 1.0;
}