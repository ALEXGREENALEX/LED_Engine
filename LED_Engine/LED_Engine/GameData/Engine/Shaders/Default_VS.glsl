#version 330
layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec2 v_UV;

#include("Light\MaterialInfo.glsl")
uniform MaterialInfo Material;

uniform mat4 MVP;

out vec2 f_UV;
out MaterialInfo f_Material;

void main()
{
	gl_Position = MVP * vec4(v_Position, 1.0);
	f_UV = v_UV;
	f_Material = Material;
}