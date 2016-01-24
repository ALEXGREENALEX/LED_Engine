#version 330
uniform sampler2D TextureUnit0; //Depth
uniform sampler2D TextureUnit1; //MainTexture

in vec2 f_UV;

uniform vec2 ScreenSize;
uniform bool FXAAEnabled = true;
uniform bool SepiaEnabled = false;

layout(location = 0) out vec3 FragColor;

#define FXAA_QUALITY__PRESET 12 //Default 12, values 10-15, 20-29, 39
#define FXAA_GREEN_AS_LUMA 1 //Or pack Luma in Alpha channel!!!
#include("FXAA_3_11.glsl")

#include("Sepia.glsl")

void main()
{
	vec3 Color = texture2D(TextureUnit1, f_UV).rgb;
	if (FXAAEnabled)
	{
		const float fxaaQualitySubpix = 0.75;
		const float fxaaQualityEdgeThreshold = 0.166;
		const float fxaaQualityEdgeThresholdMin = 0.0833;
		Color = FxaaPixelShader(f_UV, TextureUnit1, vec2(1.0) / ScreenSize, fxaaQualitySubpix, fxaaQualityEdgeThreshold, fxaaQualityEdgeThresholdMin).rgb;
	}
	
	if (SepiaEnabled)
		Color = Sepia(Color);
	
	//Gamma correction
	//const float Gamma = 2.2;
	//Color = pow(Color, vec3(1.0 / Gamma));
	FragColor = Color;
}