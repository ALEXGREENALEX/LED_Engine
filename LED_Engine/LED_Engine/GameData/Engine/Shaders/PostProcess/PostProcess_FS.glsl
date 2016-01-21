#version 330
uniform sampler2D TextureUnit0; //Depth
uniform sampler2D TextureUnit1; //MainTexture

in vec2 f_UV;

uniform vec2 ScreenSize;
uniform bool FXAAEnabled = true;
uniform bool SepiaEnabled = false;

layout(location = 0) out vec3 FragColor;

#include("FXAA.glsl")
#include("Sepia.glsl")

void main()
{
	vec3 Color = texture2D(TextureUnit1, f_UV).rgb;
	if (FXAAEnabled)
		Color = FXAA(TextureUnit1, f_UV, ScreenSize);
	
	if (SepiaEnabled)
		Color = Sepia(Color);
	
	//Gamma correction
	//const float Gamma = 2.2;
	//Color = pow(Color, vec3(1.0 / Gamma));
	FragColor = Color;
}