#version 430

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;

out vec3 ReflectDir;
out vec3 RefractDir;

uniform float RefractiveIndex;  // Index of refraction

uniform vec3 WorldCameraPosition;
uniform mat4 ModelMatrix;
uniform mat4 MVP;

void main()
{
    vec3 worldPos = vec3(ModelMatrix * vec4(VertexPosition, 1.0));
    vec3 worldNorm = vec3(ModelMatrix * vec4(VertexNormal, 0.0));
    vec3 worldView = normalize(WorldCameraPosition - worldPos);

    ReflectDir = reflect(-worldView, worldNorm);
    RefractDir = refract(-worldView, worldNorm, RefractiveIndex);
	
    gl_Position = MVP * vec4(VertexPosition, 1.0);
}