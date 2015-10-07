#version 430

layout (location = 0) in vec3 VertexPosition;

out vec3 ReflectDir;

uniform mat4 MVP;

void main()
{
    ReflectDir = VertexPosition;
    gl_Position = MVP * vec4(VertexPosition,1.0);
}
