uniform sampler2D fbo_texture;
varying vec2 f_texcoord;

void main(void)
{
	vec4 Color = texture2D(fbo_texture, f_texcoord);
	gl_FragColor.r = (Color.r * 0.393) + (Color.g * 0.769) + (Color.b * 0.189);
	gl_FragColor.g = (Color.r * 0.349) + (Color.g * 0.686) + (Color.b * 0.168);    
	gl_FragColor.b = (Color.r * 0.272) + (Color.g * 0.534) + (Color.b * 0.131);
	gl_FragColor.a = 1.0;
}