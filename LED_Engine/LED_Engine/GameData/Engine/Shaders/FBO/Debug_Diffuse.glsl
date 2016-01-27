#version 330
uniform sampler2D TextureUnit1;

in vec2 f_UV;

layout(location = 0) out vec4 Output0; //Diffuse + Light
layout(location = 1) out vec4 Output1; //AO
layout(location = 2) out vec4 Output2; //SSAO

void main()
{
	vec3 Kd = texture(TextureUnit1, f_UV).rgb;
	Output0 = vec4(Kd, 1.0);
	Output1 = vec4(0.0);
	Output2 = vec4(0.0);
}