#version 330
uniform sampler2D TextureUnit4;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Ke = texture2D(TextureUnit4, f_UV).rgb;
	FragColor = vec4(Ke, 1.0);
}