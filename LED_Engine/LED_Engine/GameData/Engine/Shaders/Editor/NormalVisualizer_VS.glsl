#version 330

layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;

out vec3 g_Position;
out vec4 g_Normal;
out int  g_Return;

uniform mat4 MVP;
uniform mat4 ModelView;
uniform mat4 P_Matrix;

void main()
{
	if(length(ModelView * vec4(v_Position, 1.0)) >= 10.0)
		g_Return = 1;
	else
		g_Return = 0;
	
	g_Position = v_Position;
	mat4 NormalMatrix = transpose(inverse(ModelView));
	g_Normal = normalize(P_Matrix * NormalMatrix * vec4(v_Normal, 0.0));
}