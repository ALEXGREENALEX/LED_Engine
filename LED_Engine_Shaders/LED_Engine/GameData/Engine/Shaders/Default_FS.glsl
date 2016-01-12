#version 330
uniform sampler2D TextureUnit0;

#include("Light\MaterialInfo.glsl")
in MaterialInfo f_Material;
in vec2 f_UV;

out vec4 FragColor;

void main()
{
	FragColor = f_Material.Kd;
	
	if (TexUnits[0])
	{
		vec4 TexColor = texture(TextureUnit0, f_UV);
		FragColor = mix(FragColor, TexColor, TexColor.a);
		//FragColor *= texture(TextureUnit0, f_UV);
	}
}