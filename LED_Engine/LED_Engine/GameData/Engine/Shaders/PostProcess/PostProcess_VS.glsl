#version 330
in vec2 v_UV;

out vec2 f_UV;

void main(void)
{
	f_UV = (v_UV + 1.0) / 2.0;
	gl_Position = vec4(v_UV, 0.0, 1.0);
}