#version 430

in vec3 ReflectDir;
in vec3 RefractDir;

//fog
in vec3 Position;
struct FogInfo {
  float MaxDist;
  float MinDist;
  vec3 Color;
};
uniform FogInfo Fog;
uniform bool FogEnabled;

layout(binding=0) uniform samplerCube CubeMapTex;

layout(location = 0) out vec4 FragColor;

uniform vec4 MaterialColor;
uniform float ReflectionFactor;

void main()
{
    vec4 reflectColor = texture(CubeMapTex, ReflectDir);
    vec4 refractColor = texture(CubeMapTex, RefractDir);
	
	reflectColor = mix(reflectColor, reflectColor * MaterialColor, MaterialColor.a);
	refractColor = mix(refractColor, refractColor * MaterialColor, MaterialColor.a);
		if(FogEnabled)
	{
		float dist = length(Position);
		float fogFactor = (Fog.MaxDist - dist) /
						(Fog.MaxDist - Fog.MinDist);
		fogFactor = clamp(fogFactor, 0.0, 1.0);
		vec4 shadeColor = mix(refractColor, reflectColor, ReflectionFactor);
		FragColor = mix(vec4(Fog.Color, 1.0), shadeColor, fogFactor);
	}
	else
		FragColor = mix(refractColor, reflectColor, ReflectionFactor);
}