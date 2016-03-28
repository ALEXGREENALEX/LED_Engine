#version 330
uniform sampler2D TextureUnit2;
uniform sampler2D TextureUnit3;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Ke;
	Ke.rg = texture(TextureUnit2, f_UV).ba;
	Ke.b = texture(TextureUnit3, f_UV).a;
	FragColor = vec4(Ke, 1.0);
}