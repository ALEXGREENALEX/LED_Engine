#version 330

in vec2 f_UV;

uniform sampler2D TextureFBO;

uniform vec2 ScreenSize;
uniform bool FXAAEnabled;
uniform bool SepiaEnabled;

out vec4 FragColor;

#include("PostProcess\FXAA.glsl")
#include("PostProcess\Sepia.glsl")

void main()
{
	vec3 Color = texture2D(TextureFBO, f_UV).rgb;
	
	if (FXAAEnabled)
		Color = FXAA(TextureFBO, f_UV);
	
	if (SepiaEnabled)
		Color = Sepia(Color);
	
	FragColor = vec4(Color, 1.0);
}