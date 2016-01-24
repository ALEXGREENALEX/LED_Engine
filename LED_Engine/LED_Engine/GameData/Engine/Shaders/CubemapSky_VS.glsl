#version 330
layout (location = 0) in vec3 v_Position;

uniform mat4 MVP;

smooth out vec3 f_UV;

void main()
{
    f_UV = v_Position;
    gl_Position = MVP * vec4(v_Position, 1.0);
}