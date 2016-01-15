#version 330
layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;
layout(location = 2) in vec2 v_UV;

uniform mat4 MVP;
uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 ModelView;
uniform mat3 NormalMatrix;

out vec3 f_Position;
out vec3 f_Normal;
out vec2 f_UV;

#include("Fog\VS_Vars.glsl")

void main()
{
	f_Position = vec3(ModelView * vec4(v_Position, 1.0));
	f_Normal = normalize(NormalMatrix * v_Normal);
	f_UV = v_UV;
	gl_Position = MVP * vec4(v_Position, 1.0);
	#include("Fog\VS.glsl")
}