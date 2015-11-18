#version 430

layout(binding = 0) uniform samplerCube CubeMapTex;
layout(location = 0) out vec4 FragColor;

in vec3 ReflectDir;
in vec3 RefractDir;

uniform vec4 MaterialColor;
uniform float ReflectionFactor;

#include("Fog\VarsFS.glsl")

void main()
{
    vec4 reflectColor = texture(CubeMapTex, ReflectDir);
    vec4 refractColor = texture(CubeMapTex, RefractDir);
	
	reflectColor = mix(reflectColor, reflectColor * MaterialColor, MaterialColor.a);
	refractColor = mix(refractColor, refractColor * MaterialColor, MaterialColor.a);
	FragColor = mix(refractColor, reflectColor, ReflectionFactor);
	
	#include("Fog\FS.glsl")
}