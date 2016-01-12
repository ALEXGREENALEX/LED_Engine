#version 330

in vec3 v_Position;
in vec2 v_UV;

out vec2 f_UV;

uniform mat4 MVP;

#include("Fog\VarsVS.glsl")

void main()
{
    gl_Position = MVP * vec4(v_Position, 1.0);
    f_UV = v_UV;
	#include("Fog\VS.glsl")
}