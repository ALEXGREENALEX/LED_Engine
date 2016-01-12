vec3 Sepia(vec3 Color)
{
	vec3 Result;
	Result.r = (Color.r * 0.393) + (Color.g * 0.769) + (Color.b * 0.189);
	Result.g = (Color.r * 0.349) + (Color.g * 0.686) + (Color.b * 0.168);    
	Result.b = (Color.r * 0.272) + (Color.g * 0.534) + (Color.b * 0.131);
	return Result;
}