// return float
// ------------------------------------------------------------------
// vec3 InColor: Input Color, RGB
float GrayScale(vec3 InColor)
{
	return dot(InColor, vec3(0.299, 0.587, 0.114));
}

// return vec3
// ------------------------------------------------------------------
// vec3 InColor: Input Color, RGB
// float Opacity: range [0..1], default 0.5
vec3 GrayScale(vec3 InColor, float Opacity)
{
	float Gray = GrayScale(InColor);
	return mix(InColor, vec3(Gray, Gray, Gray), Opacity);
}