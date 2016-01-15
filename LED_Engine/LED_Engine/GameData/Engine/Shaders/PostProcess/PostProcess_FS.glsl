#version 330
uniform sampler2D TextureUnit0; //TextureFBO

in vec2 f_UV;

uniform vec2 ScreenSize;
uniform bool FXAAEnabled;
uniform bool SepiaEnabled;

out vec4 FragColor;

#include("FXAA.glsl")
#include("Sepia.glsl")

void main()
{
	vec3 Color = texture2D(TextureUnit0, f_UV).rgb;
	
	if (FXAAEnabled)
		Color = FXAA(TextureUnit0, f_UV);
	
	if (SepiaEnabled)
		Color = Sepia(Color);
	
	FragColor = vec4(Color, 1.0);
}