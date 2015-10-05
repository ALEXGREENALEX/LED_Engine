#version 120
// FXAA shader, GLSL code adapted from:
// http://horde3d.org/wiki/index.php5?title=Shading_Technique_-_FXAA
// Whitepaper describing the technique:
// http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf

varying vec2 f_texcoord;
uniform sampler2D fbo_texture;
uniform vec2 texCoordOffset;
//uniform float R_fxaaSpanMax;	// 8.0
//uniform float R_fxaaReduceMin;	// 1.0 / 128.0
//uniform float R_fxaaReduceMul;	// 1.0 / 8.0

float FXAA_SPAN_MAX = 8.0;
float FXAA_REDUCE_MUL = 1.0 / 8.0;
float FXAA_REDUCE_MIN = 1.0 / 128.0;

void main()
{
	vec3 luma = vec3(0.299, 0.587, 0.114);	
	float lumaTL = dot(luma, texture2D(fbo_texture, f_texcoord + vec2(-1.0, -1.0) * texCoordOffset).xyz);
	float lumaTR = dot(luma, texture2D(fbo_texture, f_texcoord + vec2( 1.0, -1.0) * texCoordOffset).xyz);
	float lumaBL = dot(luma, texture2D(fbo_texture, f_texcoord + vec2(-1.0,  1.0) * texCoordOffset).xyz);
	float lumaBR = dot(luma, texture2D(fbo_texture, f_texcoord + vec2( 1.0,  1.0) * texCoordOffset).xyz);
	float lumaM  = dot(luma, texture2D(fbo_texture, f_texcoord).xyz);

	vec2 dir;
	dir.x = -((lumaTL + lumaTR) - (lumaBL + lumaBR));
	dir.y = ((lumaTL + lumaBL) - (lumaTR + lumaBR));
	
	float dirReduce = max((lumaTL + lumaTR + lumaBL + lumaBR) * FXAA_REDUCE_MUL * 0.25, FXAA_REDUCE_MIN);
	float inverseDirAdjustment = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);
	
	dir = min(vec2(FXAA_SPAN_MAX, FXAA_SPAN_MAX), 
		max(vec2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), dir * inverseDirAdjustment)) * texCoordOffset;

	vec3 result1 = 0.5 * (
		texture2D(fbo_texture, f_texcoord.xy + (dir * vec2(1.0/3.0 - 0.5))).xyz +
		texture2D(fbo_texture, f_texcoord.xy + (dir * vec2(2.0/3.0 - 0.5))).xyz);

	vec3 result2 = result1 * 0.5 + 0.25 * (
		texture2D(fbo_texture, f_texcoord.xy + (dir * vec2(0.0/3.0 - 0.5))).xyz +
		texture2D(fbo_texture, f_texcoord.xy + (dir * vec2(3.0/3.0 - 0.5))).xyz);

	float lumaMin = min(lumaM, min(min(lumaTL, lumaTR), min(lumaBL, lumaBR)));
	float lumaMax = max(lumaM, max(max(lumaTL, lumaTR), max(lumaBL, lumaBR)));
	float lumaResult2 = dot(luma, result2);
	
	if(lumaResult2 < lumaMin || lumaResult2 > lumaMax)
		gl_FragColor = vec4(result1, 1.0);
	else
		gl_FragColor = vec4(result2, 1.0);
}