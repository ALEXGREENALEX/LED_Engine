// return vec3: Color, RGB
// ------------------------------------------------------------------
// vec3 InColor: Input Color, RGB
// float Opacity: range [0..1], default 0.5
vec3 Sepia(vec3 InColor, float Opacity = 0.5)
{
	vec3 Result;
	Result.r = (InColor.r * 0.393) + (InColor.g * 0.769) + (InColor.b * 0.189);
	Result.g = (InColor.r * 0.349) + (InColor.g * 0.686) + (InColor.b * 0.168);    
	Result.b = (InColor.r * 0.272) + (InColor.g * 0.534) + (InColor.b * 0.131);
	return mix(InColor, Result, Opacity);
}