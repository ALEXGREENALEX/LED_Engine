#version 330

in vec2 f_UV;
out vec4 FragColor;

uniform sampler2D MainTexture;

void main()
{
    FragColor = texture(MainTexture, f_UV);
}