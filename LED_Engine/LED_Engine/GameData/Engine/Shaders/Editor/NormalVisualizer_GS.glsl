#version 330

layout(triangles) in;
layout(line_strip, max_vertices = 6) out;

in vec3 g_Position[];
in vec4 g_Normal[];
in int  g_Return[];

out vec3 f_Color;

uniform mat4 MVP;

const float NormalLength = 0.1;
void main()
{
	for(int i = 0; i < gl_in.length(); i++)
	{
		if (g_Return[0] == 1)
			return;
		
		f_Color = vec3(0.0, 0.0, 1.0);
		gl_Position = MVP * vec4(g_Position[i], 1.0);
		EmitVertex();
		
		f_Color = vec3(1.0, 1.0, 0.0);
		gl_Position += g_Normal[i] * NormalLength;
		EmitVertex();
		
		EndPrimitive();
	}
}