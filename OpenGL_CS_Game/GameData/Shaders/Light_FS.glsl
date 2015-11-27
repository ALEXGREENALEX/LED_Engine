#version 330

in vec2 f_texcoord;
in vec3 f_color;
uniform sampler2D MainTexture;

out vec4 FragColor;

#include("Fog\VarsFS.glsl")

void main()
{
    vec4 Color = texture(MainTexture, f_texcoord);
    FragColor = Color * vec4(f_color, 0.0) * 4.0;
	
	#include("Fog\FS.glsl")
}