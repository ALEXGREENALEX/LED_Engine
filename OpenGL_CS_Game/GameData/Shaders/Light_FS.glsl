#version 330

in vec3 f_VertexPosition;
in vec2 f_TexCoord;
in vec4 f_Normal;
in mat4 f_ModelMatrix;

uniform sampler2D MainTexture;

uniform vec3 LightPosition;
uniform vec3 LightDiffuseColor;

out vec4 FragColor;

#include("Fog\VarsFS.glsl")

void main()
{
	vec4 worldpos = vec4(f_VertexPosition, 1.0) * f_ModelMatrix;
	vec4 lightVector = normalize(worldpos - vec4(LightPosition, 1.0));
	
	vec3 LightColor = LightDiffuseColor * dot(-lightVector, f_Normal);
	
    vec4 Color = texture(MainTexture, f_TexCoord);
    FragColor = Color * vec4(LightColor, 0.0) * 4.0;
	
	#include("Fog\FS.glsl")
}