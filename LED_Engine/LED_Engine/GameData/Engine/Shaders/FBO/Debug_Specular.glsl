#version 330
uniform sampler2D TextureUnit3;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Ks = texture2D(TextureUnit3, f_UV).rgb;
	FragColor = vec4(Ks, 1.0);
}