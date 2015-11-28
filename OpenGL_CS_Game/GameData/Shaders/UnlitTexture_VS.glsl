#version 330

in vec3 VertexPosition;
in vec2 VertexTexCoord;
out vec2 f_texcoord;

uniform mat4 MVP;

#include("Fog\VarsVS.glsl")

void main()
{
    gl_Position = MVP * vec4(VertexPosition, 1.0);
    f_texcoord = VertexTexCoord;
	#include("Fog\VS.glsl")
}