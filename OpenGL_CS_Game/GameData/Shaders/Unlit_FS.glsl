#version 330

in vec4 f_color;
out vec4 FragColor;

#include("Fog\VarsFS.glsl")

void main()
{
    FragColor = f_color;
	
	#include("Fog\FS.glsl")
}