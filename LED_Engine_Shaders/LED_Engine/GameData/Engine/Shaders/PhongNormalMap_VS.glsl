#version 330
layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;
layout(location = 2) in vec2 v_UV;
layout(location = 3) in vec3 v_Tan;

#include("Light\LightInfo.glsl")
uniform LightInfo Light;

uniform mat4 ModelView;
uniform mat4 MVP;
uniform mat3 NormalMatrix;

out vec3 f_Position;
out vec3 f_LightDir;
out vec3 f_ViewDir;
out vec2 f_UV;

#include("Fog\VS_Vars.glsl")

void main()
{
	// Transform normal and tangent to eye space
	vec3 N = normalize(NormalMatrix * v_Normal);
	vec3 T = normalize(NormalMatrix * v_Tan);
	vec3 B = normalize(cross(N, T));
	
	// Matrix for transformation to tangent space
	mat3 Transposed_TBN = mat3(
        T.x, B.x, N.x,
        T.y, B.y, N.y,
        T.z, B.z, N.z);
	
	// Transform light direction and view direction to tangent space
	f_Position = vec3(ModelView * vec4(v_Position, 1.0));
	f_LightDir = normalize(Transposed_TBN * (Light.Pos - f_Position));
	f_ViewDir = Transposed_TBN * normalize(-f_Position);
	f_UV = v_UV;
	gl_Position = MVP * vec4(v_Position, 1.0);
	#include("Fog\VS.glsl")
}