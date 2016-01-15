#version 330
uniform sampler2D TextureUnit0;

#include("Light\MaterialInfo.glsl")
in MaterialInfo f_Material;
in vec2 f_UV;

out vec4 FragColor;

#include("Fog\FS_Vars.glsl")

void main()
{
	FragColor = f_Material.Kd;
	
	if (TexUnits[0])
		FragColor *= texture(TextureUnit0, f_UV);
	
	#include("Fog\FS.glsl")
}