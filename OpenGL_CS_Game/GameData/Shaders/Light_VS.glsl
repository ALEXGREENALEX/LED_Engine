#version 330

in vec3 VertexPosition;
in vec3 VertexNormal;
in vec2 VertexTexCoord;

out vec2 f_texcoord;
out vec3 f_color;

uniform mat4 MVP;
uniform mat4 ModelMatrix;
uniform vec3 LightPosition;
uniform vec3 LightDiffuseColor;

#include("Fog\VarsVS.glsl")

void main()
{	
	vec4 normal = transpose(inverse(ModelMatrix)) * vec4(VertexNormal, 0.0);
	normal = normalize(normal);
	
	vec4 worldpos = vec4(VertexPosition, 1.0) * transpose(ModelMatrix);
	vec4 lightVector = normalize(worldpos - vec4(LightPosition, 1.0));
	
	f_color = LightDiffuseColor * dot(-lightVector, normal);
	
	gl_Position = MVP * vec4(VertexPosition, 1.0);
	f_texcoord = VertexTexCoord;
	
	#include("Fog\VS.glsl")
}