#version 330

in vec2 f_texcoord;
out vec4 outputColor;

uniform sampler2D maintexture;

void main()
{
    outputColor = texture(maintexture, vec2(f_texcoord.x, -f_texcoord.y));
}