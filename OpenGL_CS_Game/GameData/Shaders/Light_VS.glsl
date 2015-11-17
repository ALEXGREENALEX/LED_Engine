#version 330

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
}
/*
#version 330

in vec3 VertexPosition;
in vec3 VertexNormal;
in vec2 VertexTexCoord;

out vec2 f_texcoord;
out vec3 f_color;

uniform mat4 MVP;
uniform mat4 ModelMatrix;
uniform vec3 LightPosition;
uniform vec3 LightColor;

void main()
{
    gl_Position = MVP * vec4(VertexPosition, 1.0);
    f_texcoord = VertexTexCoord;
	
	vec4 normal = transpose(inverse(ModelMatrix)) * vec4(VertexNormal, 0.0);
	normal = normalize(normal);
	
	vec4 worldpos = vec4(VertexPosition, 1.0) * transpose(ModelMatrix);
	vec4 lightVector = worldpos - vec4(LightPosition, 1.0);
	lightVector = normalize(lightVector);
	
	float tmp2 = dot(-lightVector, normal);
	f_color = LightColor * tmp2;
}
*/