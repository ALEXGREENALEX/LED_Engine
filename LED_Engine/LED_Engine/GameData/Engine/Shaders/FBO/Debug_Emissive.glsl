#version 330
uniform sampler2D TextureUnit2;
uniform sampler2D TextureUnit3;

in vec2 f_UV;

layout(location = 0) out vec4 Output0; //Diffuse + Light
layout(location = 1) out vec4 Output1; //AO
layout(location = 2) out vec4 Output2; //SSAO

void main()
{
	vec3 Ke;
	Ke.rg = texture(TextureUnit2, f_UV).ba;
	Ke.b = texture(TextureUnit3, f_UV).a;
	Output0 = vec4(Ke, 1.0);
	Output1 = vec4(0.0);
	Output2 = vec4(0.0);
}