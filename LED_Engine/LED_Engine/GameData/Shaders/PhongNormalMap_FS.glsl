#version 330

uniform sampler2D ColorTex;
uniform sampler2D NormalMapTex;

layout(location = 0) out vec4 FragColor;

in vec3 LightDir;
in vec2 f_UV;
in vec3 ViewDir;

uniform vec3 LightIntensity; // A,D,S intensity

uniform vec3 MaterialKa;            // Ambient reflectivity
uniform vec3 MaterialKs;            // Specular reflectivity
uniform float MaterialShin;    // Specular shininess factor

#include("Fog\VarsFS.glsl")

vec3 PhongModel(vec3 norm, vec3 diffR)
{
	vec3 r = reflect(-LightDir, norm);
	vec3 ambient = LightIntensity * MaterialKa;
	float sDotN = max(dot(LightDir, norm), 0.0);
	vec3 diffuse = LightIntensity * diffR * sDotN;

    vec3 spec = vec3(0.0);
	if (sDotN > 0.0)
        spec = LightIntensity * MaterialKs * pow(max(dot(r, ViewDir), 0.0), MaterialShin);

	return ambient + diffuse + spec;
}

void main()
{
	vec3 normal = texture(NormalMapTex, f_UV).rgb * 2.0 - 1.0;
	vec3 texColor = texture(ColorTex, f_UV).rgb;
	FragColor = vec4(PhongModel(normal, texColor), 1.0);
	#include("Fog\FS.glsl")
}