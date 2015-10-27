#version 330

in vec2 f_texcoord;

uniform sampler2D fbo_texture;
uniform vec2 ScreenSize;

out vec4 FragColor;

vec4 Sepia()
{
	vec4 Result = vec4(1.0);
	vec4 Color = texture2D(fbo_texture, f_texcoord);
	Result.r = (Color.r * 0.393) + (Color.g * 0.769) + (Color.b * 0.189);
	Result.g = (Color.r * 0.349) + (Color.g * 0.686) + (Color.b * 0.168);    
	Result.b = (Color.r * 0.272) + (Color.g * 0.534) + (Color.b * 0.131);
	Result.a = 1.0;
	return Result;
}

vec4 FXAA()
{
	vec4 Result = vec4(1.0);
	
	float FXAA_SPAN_MAX = 8.0;
	float FXAA_REDUCE_MUL = 1.0 / 8.0;
	float FXAA_REDUCE_MIN = 1.0 / 128.0;
	
	vec3 luma = vec3(0.299, 0.587, 0.114);	
	float lumaTL = dot(luma, texture2D(fbo_texture, f_texcoord + vec2(-1.0, -1.0) / ScreenSize).xyz);
	float lumaTR = dot(luma, texture2D(fbo_texture, f_texcoord + vec2( 1.0, -1.0) / ScreenSize).xyz);
	float lumaBL = dot(luma, texture2D(fbo_texture, f_texcoord + vec2(-1.0,  1.0) / ScreenSize).xyz);
	float lumaBR = dot(luma, texture2D(fbo_texture, f_texcoord + vec2( 1.0,  1.0) / ScreenSize).xyz);
	float lumaM  = dot(luma, texture2D(fbo_texture, f_texcoord).xyz);

	vec2 dir;
	dir.x = -((lumaTL + lumaTR) - (lumaBL + lumaBR));
	dir.y = ((lumaTL + lumaBL) - (lumaTR + lumaBR));
	
	float dirReduce = max((lumaTL + lumaTR + lumaBL + lumaBR) * FXAA_REDUCE_MUL * 0.25, FXAA_REDUCE_MIN);
	float inverseDirAdjustment = 1.0 / (min(abs(dir.x), abs(dir.y)) + dirReduce);
	
	dir = min(vec2(FXAA_SPAN_MAX, FXAA_SPAN_MAX), 
		max(vec2(-FXAA_SPAN_MAX, -FXAA_SPAN_MAX), dir * inverseDirAdjustment)) / ScreenSize;

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
		Result = vec4(result1, 1.0);
	else
		Result = vec4(result2, 1.0);
	return Result;
}

void main()
{
	FragColor = mix(FXAA(), Sepia(), 0.2);
}