#version 430

layout(binding = 0) uniform sampler2D ColorTex;
layout(binding = 1) uniform sampler2D NormalMapTex;
layout(location = 0) out vec4 FragColor;

in vec3 LightDir;
in vec2 TexCoord;
in vec3 ViewDir;

uniform vec3 LightIntensity; // A,D,S intensity

uniform vec3 MaterialKa;            // Ambient reflectivity
uniform vec3 MaterialKs;            // Specular reflectivity
uniform float MaterialShininess;    // Specular shininess factor

#include("Fog\VarsFS.glsl")

vec3 PhongModel (vec3 norm, vec3 diffR)
{
    vec3 r = reflect(-LightDir, norm);
    vec3 ambient = LightIntensity * MaterialKa;
    float sDotN = max(dot(LightDir, norm), 0.0);
    vec3 diffuse = LightIntensity * diffR * sDotN;

    vec3 spec = vec3(0.0);
    if (sDotN > 0.0)
        spec = LightIntensity * MaterialKs * pow(max(dot(r, ViewDir), 0.0), MaterialShininess);

    return ambient + diffuse + spec;
}

void main()
{
    vec4 normal = 2.0 * texture(NormalMapTex, TexCoord) - 1.0;
    vec4 texColor = texture(ColorTex, TexCoord);
    FragColor = vec4(PhongModel(normal.xyz, texColor.rgb), 1.0);
	#include("Fog\FS.glsl")
}