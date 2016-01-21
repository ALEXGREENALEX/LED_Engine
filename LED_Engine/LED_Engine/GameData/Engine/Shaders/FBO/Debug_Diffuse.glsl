#version 330
uniform sampler2D TextureUnit1;

in vec2 f_UV;

layout(location = 0) out vec4 FragColor;

void main()
{
	vec3 Kd = texture2D(TextureUnit1, f_UV).rgb;
	FragColor = vec4(Kd, 1.0);
}