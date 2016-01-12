#version 330

layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;

out vec3 f_Color;

uniform mat4 MVP;

void main()
{
	f_Color = normalize(v_Normal);
	gl_Position = MVP * vec4(v_Position, 1.0);
}