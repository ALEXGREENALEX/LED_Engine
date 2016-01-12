#version 330
layout (location = 0) in vec3 v_Position;
layout (location = 1) in vec3 v_Normal;
layout (location = 2) in vec2 v_UV;
layout (location = 3) in vec3 v_Tan;

out vec3 LightDir;
out vec2 f_UV;
out vec3 ViewDir;

uniform vec4 EyeLightPos; // Light position in eye coords.

uniform mat4 ModelView;
uniform mat4 MVP;

#include("Fog\VarsVS.glsl")

void main()
{
	mat3 NormalMatrix = transpose(inverse(mat3(ModelView)));
	// Transform normal and tangent to eye space
	vec3 N = normalize(NormalMatrix * v_Normal);
	vec3 T = normalize(NormalMatrix * v_Tan);
	vec3 B = normalize(cross(N, T));
	
	// Matrix for transformation to tangent space
	mat3 toObjectLocal = mat3(
        T.x, B.x, N.x,
        T.y, B.y, N.y,
        T.z, B.z, N.z);
	
	// Transform light direction and view direction to tangent space
	vec3 pos = vec3(ModelView * vec4(v_Position, 1.0));
	LightDir = normalize(toObjectLocal * (EyeLightPos.xyz - pos));
	ViewDir = toObjectLocal * normalize(-pos);
	
	f_UV = v_UV;
	gl_Position = MVP * vec4(v_Position, 1.0);
	#include("Fog\VS.glsl")
}