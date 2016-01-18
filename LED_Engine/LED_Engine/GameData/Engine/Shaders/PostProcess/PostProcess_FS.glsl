#version 330
uniform sampler2D TextureUnit0; //Depth
uniform sampler2D TextureUnit1; //MainTexture

in vec2 f_UV;

uniform vec2 ScreenSize;
//uniform vec2 CameraNearFar;
uniform bool FXAAEnabled = true;
uniform bool SepiaEnabled = false;

layout(location = 0) out vec4 FragColor;

#include("FXAA.glsl")
#include("Sepia.glsl")

void main()
{
	vec3 Color = vec3(0.0);
	if (FXAAEnabled)
		Color = FXAA(TextureUnit1, f_UV, ScreenSize);
	
	if (SepiaEnabled)
		Color = Sepia(Color);
	
	//Gamma correction
	//const float Gamma = 2.2;
	//FragColor = pow(FragColor, vec3(1.0 / Gamma));
	FragColor = vec4(Color, 1.0);
}