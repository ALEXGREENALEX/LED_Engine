#version 430

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;
layout (location = 2) in vec2 VertexTexCoord;
layout (location = 3) in vec4 VertexTangent;

out vec3 LightVector;
out vec3 EyeVector;
out vec2 TexCoord;

uniform mat4 ModelViewMatrix;
uniform mat3 NormalMatrix;
uniform mat4 MVP;
uniform vec3 LightPosition;
uniform vec3 CameraPosition;

void main()
{
	vec3 norm = normalize(NormalMatrix * VertexNormal);
	vec3 tang = normalize(NormalMatrix * (VertexTangent.xyz * VertexTangent.w));
	vec3 bitan = cross(norm, tang);
  
	// need positions in tangent space
	vec3 vertex = vec3(ModelViewMatrix * vec4(VertexPosition, 1.0));
	
	vec3 temp = LightPosition - vertex;
	LightVector.x = dot(temp, tang);
	LightVector.y = dot(temp, bitan);
	LightVector.z = dot(temp, norm);
	
	temp = CameraPosition - vertex;
	EyeVector.x = dot(temp, tang);
	EyeVector.y = dot(temp, bitan);
	EyeVector.z = dot(temp, norm);
  
  	TexCoord = VertexTexCoord;
	gl_Position = MVP * vec4(VertexPosition, 1.0);
}
/*
#version 440

layout (location = 0) in vec3 VertexPosition;
layout (location = 1) in vec3 VertexNormal;

out vec4 eyeCordFs;
out vec4 eyeNormalFs;

uniform mat4 ModelMatrix;
uniform mat4 ViewMatrix;
uniform mat4 NormalMatrix;
uniform mat4 MVP;

void main()
{   
    mat4 modelView = ViewMatrix * ModelMatrix;
    mat4 normalMatrix = transpose(inverse(modelView));
    vec4 eyeNorm = normalize(normalMatrix * vec4(VertexNormal, 0.0));
    vec4 eyeCord = modelView * vec4(VertexPosition, 1.0);

    eyeCordFs = eyeCord;
    eyeNormalFs = eyeNorm;

    gl_Position = MVP * vec4(VertexPosition, 1.0);
}*/