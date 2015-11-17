#version 430

layout (location = 0) in vec3 VertexPosition;

smooth out vec3 CubemapTexCoords;

uniform mat4 MVP;

void main()
{
    CubemapTexCoords = VertexPosition;
    gl_Position = MVP * vec4(VertexPosition,1.0);
}
