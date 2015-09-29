attribute vec2 v_coord;
uniform sampler2D fbo_texture;
varying vec2 f_texcoord;

void main(void)
{
	gl_Position = vec4(v_coord, 0.0, 1.0);
	f_texcoord = (v_coord + 1.0) / 2.0;
}
/*
#version 120
attribute vec3 position;
attribute vec2 texCoord;

varying vec2 texCoord0;
uniform mat4 T_MVP;

void main()
{
    gl_Position = T_MVP * vec4(position, 1.0);
    texCoord0 = texCoord; 
}
*/