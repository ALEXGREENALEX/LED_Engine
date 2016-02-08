#version 330
uniform sampler2D TextureUnit0; //MainTexture

in vec2 f_UV;

uniform vec2 ScreenSize;
uniform float Gamma = 1.0;

uniform bool FXAAEnabled = true;
uniform vec3 FXAASettings = vec3(0.75, 0.166, 0.0833); //fxaaQualitySubpix, fxaaQualityEdgeThreshold, fxaaQualityEdgeThresholdMin

uniform vec3 VignetteSettings = vec3(0.75, 0.5, 0.5); //Radius, Softness, Opacity
uniform vec2 SepiaGrayscale = vec2(0.0, 0.0); //Opacity of Sepia, GrayScale

layout(location = 0) out vec3 FragColor;

#define FXAA_GATHER4_ALPHA 0 //Use 1 for Nvidia, 0 - for NVidia and Intel
#define FXAA_QUALITY__PRESET 12 //Default 12, values 10-15, 20-29, 39
#include("FXAA_3_11.glsl")

#include("Vignette.glsl")
#include("Sepia.glsl")
#include("GrayScale.glsl")

void main()
{
	vec3 Color;
	if(FXAAEnabled)
		Color = FxaaPixelShader(f_UV, TextureUnit0, vec2(1.0) / ScreenSize, FXAASettings.x, FXAASettings.y, FXAASettings.z).rgb;
	else
		Color = texture(TextureUnit0, f_UV).rgb;
	
	Color = Vignette(Color, f_UV, VignetteSettings.x, VignetteSettings.y, VignetteSettings.z);
	Color = Sepia(Color, SepiaGrayscale.x);
	Color = GrayScale(Color, SepiaGrayscale.y);
	
	//Gamma correction
	Color = pow(Color, vec3(1.0 / Gamma));
	FragColor = Color;
}