#version 330
uniform sampler2D TextureUnit0; // DiffuseMap
uniform sampler2D TextureUnit1; // NormalMap;

in vec3 f_Position;
in vec3 f_LightDir;
in vec3 f_ViewDir;
in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

#include("Light\FS_Vars.glsl")
#include("Fog\FS_Vars.glsl")

void main()
{
	vec4 DiffuseMap = texture(TextureUnit0, f_UV);
	vec3 NormalMap = normalize(texture(TextureUnit1, f_UV).rgb * 2.0 - 1.0);
	
	MaterialInfo f_Material = Material;
	f_Material.Kd = DiffuseMap;
	
	vec3 LightResult = CalcLight(f_Material, NormalMap, f_Position, f_LightDir, f_ViewDir);
	FragColor = vec4(LightResult, DiffuseMap.a);
	//#include("Fog\FS.glsl")
}