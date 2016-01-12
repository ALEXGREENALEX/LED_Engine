#version 330

in vec4 f_Color;
out vec4 FragColor;

#include("Fog\VarsFS.glsl")

void main()
{
    FragColor = f_Color;
	
	#include("Fog\FS.glsl")
}