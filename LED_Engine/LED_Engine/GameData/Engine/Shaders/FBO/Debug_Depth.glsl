#version 330
uniform sampler2D TextureUnit0; // Depth

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

#include("LinearDepth.glsl")

void main()
{
	float Depth = LinearDepth(TextureUnit0, f_UV);
	FragColor = vec4(Depth, Depth, Depth, 1.0);
}