#version 330
uniform sampler2D TextureUnit0;

in vec3 f_Position;
in vec2 f_UV;
in vec3 f_Normal;

#include("Light\LightInfo.glsl")
uniform LightInfo Light;

out vec4 FragColor;

#include("Fog\FS_Vars.glsl")

void main()
{
	vec4 DiffuseMap = texture(TextureUnit0, f_UV);
	
	vec3 RayVector = vec3(f_Position - Light.Pos);
	vec3 LightVector = normalize(RayVector);
	vec3 LightColor = Light.Ld * dot(-LightVector, f_Normal);
	
	// Расчет коэф. затухания
	float Distance = length(RayVector) * 2.28; // 2.28 - random value, used just for increase attenuation
	float LightIntensity = 1.0;
    float Attenuation = LightIntensity / (Light.Att.x + Light.Att.y * Distance + Light.Att.z * Distance * Distance);
	
    FragColor = DiffuseMap * vec4(LightColor, 0.0) * Attenuation;
	
	#include("Fog\FS.glsl")
}