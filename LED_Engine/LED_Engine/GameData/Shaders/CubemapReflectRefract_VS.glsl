#version 330

layout (location = 0) in vec3 v_Position;
layout (location = 1) in vec3 v_Normal;

out vec3 ReflectDir;
out vec3 RefractDir;

uniform float RefractIndex;  // Index of refraction

uniform vec3 CameraPos;
uniform mat4 M_Matrix;
uniform mat4 MVP;

#include("Fog\VarsVS.glsl")

void main()
{
    vec3 worldPos = vec3(M_Matrix * vec4(v_Position, 1.0));
    vec3 worldNorm = vec3(M_Matrix * vec4(v_Normal, 0.0));
    vec3 worldView = normalize(CameraPos - worldPos);

    ReflectDir = reflect(-worldView, worldNorm);
    RefractDir = refract(-worldView, worldNorm, RefractIndex);
	
	#include("Fog\VS.glsl")
	
    gl_Position = MVP * vec4(v_Position, 1.0);
}