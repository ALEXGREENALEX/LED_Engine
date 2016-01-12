#version 330

layout(location = 0) in vec3 v_Position;

uniform mat4 MVP;
uniform vec4 MaterialColor;

out vec4 f_Color;

#include("Fog\VarsVS.glsl")

void main()
{
    gl_Position = MVP * vec4(v_Position, 1.0);
    f_Color = MaterialColor;
	#include("Fog\VS.glsl")
}