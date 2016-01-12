#version 330

layout (location = 0) in vec3 v_Position;

smooth out vec3 Cubemap_UV;

uniform mat4 MVP;

void main()
{
    Cubemap_UV = v_Position;
    gl_Position = MVP * vec4(v_Position, 1.0);
}
