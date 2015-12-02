#version 330

in vec3 VertexPosition;
in vec3 VertexNormal;
in vec2 VertexTexCoord;

out vec3 f_VertexPosition;
out vec4 f_Normal;
out vec2 f_TexCoord;
out mat4 f_ModelMatrixT;

uniform mat4 MVP;
uniform mat4 ModelMatrix;

#include("Fog\VarsVS.glsl")

void main()
{
	vec4 normal = transpose(inverse(ModelMatrix)) * vec4(VertexNormal, 0.0);
	f_Normal = normalize(normal);
	f_ModelMatrixT = transpose(ModelMatrix);
	gl_Position = MVP * vec4(VertexPosition, 1.0);
	f_VertexPosition = VertexPosition;
	f_TexCoord = VertexTexCoord;
	#include("Fog\VS.glsl")
}