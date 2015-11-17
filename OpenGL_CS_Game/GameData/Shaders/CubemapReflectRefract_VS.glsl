#version 430

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;

out vec3 ReflectDir;
out vec3 RefractDir;

//Fog
out vec3 Position;

uniform float RefractiveIndex;  // Index of refraction

uniform vec3 CameraPosition;
uniform mat4 ModelMatrix;
uniform mat4 MVP;

void main()
{
    vec3 worldPos = vec3(ModelMatrix * vec4(VertexPosition, 1.0));
    vec3 worldNorm = vec3(ModelMatrix * vec4(VertexNormal, 0.0));
    vec3 worldView = normalize(CameraPosition - worldPos);

    ReflectDir = reflect(-worldView, worldNorm);
    RefractDir = refract(-worldView, worldNorm, RefractiveIndex);
	
	//Fog
    Position = vec3(MVP * vec4(VertexPosition, 1.0));
	
    gl_Position = MVP * vec4(VertexPosition, 1.0);
}