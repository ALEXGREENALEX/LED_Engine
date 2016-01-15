#version 330
layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;
layout(location = 2) in vec2 v_UV;

uniform float RefractIndex;  // Index of refraction

uniform vec3 CameraPos;
uniform mat4 ModelMatrix;
uniform mat4 MVP;

out vec2 f_UV;
out vec3 ReflectDir;
out vec3 RefractDir;

#include("Fog\VS_Vars.glsl")

void main()
{
	vec3 worldPos = vec3(ModelMatrix * vec4(v_Position, 1.0));
	vec3 worldNorm = vec3(ModelMatrix * vec4(v_Normal, 0.0));
	vec3 worldView = normalize(CameraPos - worldPos);

	ReflectDir = reflect(-worldView, worldNorm);
	RefractDir = refract(-worldView, worldNorm, RefractIndex);
	
	#include("Fog\VS.glsl")
	
	f_UV = v_UV;
	gl_Position = MVP * vec4(v_Position, 1.0);
}