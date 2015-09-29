uniform sampler2D fbo_texture;
varying vec2 f_texcoord;

void main(void)
{
	if (f_texcoord.x - f_texcoord.y <= 0)
	{
		vec4 Color = texture2D(fbo_texture, f_texcoord);
		gl_FragColor.r = (Color.r * 0.393) + (Color.g * 0.769) + (Color.b * 0.189);
		gl_FragColor.g = (Color.r * 0.349) + (Color.g * 0.686) + (Color.b * 0.168);    
		gl_FragColor.b = (Color.r * 0.272) + (Color.g * 0.534) + (Color.b * 0.131);
	}
	else
		gl_FragColor = texture2D(fbo_texture, f_texcoord);
}
/*
#version 120
// FXAA shader, GLSL code adapted from:
// http://horde3d.org/wiki/index.php5?title=Shading_Technique_-_FXAA
// Whitepaper describing the technique:
// http://developer.download.nvidia.com/assets/gamedev/files/sdk/11/FXAA_WhitePaper.pdf

varying vec2 texCoord0;
uniform sampler2D R_filterTexture;
uniform vec3 R_inverseFilterTextureSize;
uniform float R_fxaaSpanMax;	// 8.0
uniform float R_fxaaReduceMin;	// 1.0 / 128.0
uniform float R_fxaaReduceMul;	// 1.0 / 8.0

void main()
{
	vec2 texCoordOffset = R_inverseFilterTextureSize.xy;
	
	vec3 luma = vec3(0.299, 0.587, 0.114);	
	float lumaTL = dot(luma, texture2D(R_filterTexture, texCoord0.xy + (vec2(-1.0, -1.0) * texCoordOffset)).xyz);
	float lumaTR = dot(luma, texture2D(R_filterTexture, texCoord0.xy + (vec2(1.0, -1.0) * texCoordOffset)).xyz);
	float lumaBL = dot(luma, texture2D(R_filterTexture, texCoord0.xy + (vec2(-1.0, 1.0) * texCoordOffset)).xyz);
	float lumaBR = dot(luma, texture2D(R_filterTexture, texCoord0.xy + (vec2(1.0, 1.0) * texCoordOffset)).xyz);
	float lumaM  = dot(luma, texture2D(R_filterTexture, texCoord0.xy).xyz);

	vec2 dir;
	dir.x = -((lumaTL + lumaTR) - (lumaBL + lumaBR));
	dir.y = ((lumaTL + lumaBL) - (lumaTR + lumaBR));
	
	float dirReduce = max((lumaTL + lumaTR + lumaBL + lumaBR) * (R_fxaaReduceMul * 0.25), R_fxaaReduceMin);
	float inverseDirAdjustment = 1.0/(min(abs(dir.x), abs(dir.y)) + dirReduce);
	
	dir = min(vec2(R_fxaaSpanMax, R_fxaaSpanMax), 
		max(vec2(-R_fxaaSpanMax, -R_fxaaSpanMax), dir * inverseDirAdjustment)) * texCoordOffset;

	vec3 result1 = (1.0/2.0) * (
		texture2D(R_filterTexture, texCoord0.xy + (dir * vec2(1.0/3.0 - 0.5))).xyz +
		texture2D(R_filterTexture, texCoord0.xy + (dir * vec2(2.0/3.0 - 0.5))).xyz);

	vec3 result2 = result1 * (1.0/2.0) + (1.0/4.0) * (
		texture2D(R_filterTexture, texCoord0.xy + (dir * vec2(0.0/3.0 - 0.5))).xyz +
		texture2D(R_filterTexture, texCoord0.xy + (dir * vec2(3.0/3.0 - 0.5))).xyz);

	float lumaMin = min(lumaM, min(min(lumaTL, lumaTR), min(lumaBL, lumaBR)));
	float lumaMax = max(lumaM, max(max(lumaTL, lumaTR), max(lumaBL, lumaBR)));
	float lumaResult2 = dot(luma, result2);
	
	if(lumaResult2 < lumaMin || lumaResult2 > lumaMax)
		gl_FragColor = vec4(result1, 1.0);
	else
		gl_FragColor = vec4(result2, 1.0);
}
*/