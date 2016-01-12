#version 330

in vec2 f_UV;
out vec4 FragColor;

uniform sampler2D Texture;

#include("Fog\VarsFS.glsl")

void main()
{
    FragColor = texture(Texture, f_UV);
	
	#include("Fog\FS.glsl")
}