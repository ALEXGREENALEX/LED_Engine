// return float: Vignette factor
// ------------------------------------------------------------------
// vec2 UV: Texture coords, range for 'x' and 'y' [0..1]
// float Radius: radius of vignette circle, default 0.5 (fit circle) or 0.75
// float Softness: Softness [0..1], default 0.5
float Vignette(vec2 UV, float Radius, float Softness)
{
	float Len = length(UV - vec2(0.5));
	return smoothstep(Radius, Radius - Softness, Len);
}

// return vec3: Color, RGB
// ------------------------------------------------------------------
// vec3 InColor: Input Color, RGB
// vec2 UV: Texture coords, range for 'x' and 'y' [0..1]
// float Radius: radius of vignette circle, default 0.5 (fit circle) - 0.75
// float Softness: range [0..1], default 0.5
// float Opacity: range [0..1], default 0.5
vec3 Vignette(vec3 InColor, vec2 UV, float Radius, float Softness, float Opacity)
{
	if (Opacity <= 0.0)
		return InColor;
	
	float Vignette = Vignette(UV, Radius, Softness);
	return mix(InColor, InColor * Vignette, Opacity);
}