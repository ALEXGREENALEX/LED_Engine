#version 330
uniform samplerCube	TextureUnit0; //CubeMap
uniform sampler2D	TextureUnit1; //DiffuseMap

in vec2 f_UV;
in vec3 ReflectDir;
in vec3 RefractDir;

#include("Other\MaterialInfo.glsl")
uniform MaterialInfo Material;

uniform float ReflectFactor;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 reflectColor = texture(TextureUnit0, ReflectDir).rgb;
	vec3 refractColor = texture(TextureUnit0, RefractDir).rgb;
	
	vec3 DiffColor = Material.Kd.rgb;
	
	if(TexUnits[1])
	{
		vec4 DiffMap = texture(TextureUnit1, f_UV);
		DiffColor = mix(DiffColor, DiffMap.rgb, DiffMap.a);
	}
	
	FragColor = vec4(mix(refractColor, reflectColor, ReflectFactor) * DiffColor, 1.0);
}