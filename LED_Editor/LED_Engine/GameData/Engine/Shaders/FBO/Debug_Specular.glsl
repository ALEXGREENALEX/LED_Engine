#version 330
uniform sampler2D TextureUnit4;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Ks = texture(TextureUnit4, f_UV).rgb;
	FragColor = vec4(Ks, 1.0);
}