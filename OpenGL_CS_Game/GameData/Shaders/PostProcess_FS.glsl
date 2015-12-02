#version 330

in vec2 f_texcoord;

uniform sampler2D TextureFBO;
uniform vec2 ScreenSize;

out vec4 FragColor;

#include("PostProcess\FXAA.glsl")
#include("PostProcess\Sepia.glsl")

void main()
{
	FragColor = mix(FXAA(), Sepia(), 0.0);
}