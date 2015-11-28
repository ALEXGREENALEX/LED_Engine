#version 330

in vec2 f_texcoord;
out vec4 FragColor;

uniform sampler2D maintexture;

#include("Fog\VarsFS.glsl")

void main()
{
    FragColor = texture(maintexture, f_texcoord);
	
	#include("Fog\FS.glsl")
}