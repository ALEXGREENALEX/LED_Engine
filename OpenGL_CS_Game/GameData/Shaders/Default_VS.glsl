#version 330

layout(location = 0) in vec3 VertexPosition;

uniform mat4 MVP;
uniform vec4 MaterialColor;

out vec4 f_color;

void main()
{
    gl_Position = MVP * vec4(VertexPosition, 1.0);
    f_color = MaterialColor;
}