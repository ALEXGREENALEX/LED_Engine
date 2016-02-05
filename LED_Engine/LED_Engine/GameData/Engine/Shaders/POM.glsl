//POM, based on: http://steps3d.narod.ru/tutorials/parallax-mapping-tutorial.html
vec2 ParallaxOcclusionMapping(sampler2D HeightTexture, vec2 UV, vec3 ViewVector, float Scale)
{
	const float MinSteps  = 10.0;
	const float MaxSteps  = 20.0;
	
	float NumSteps = MaxSteps + ViewVector.z * (MinSteps - MaxSteps);
	float Step = 1.0 / NumSteps;
	vec2 dTexCoords = -Step * Scale * ViewVector.xy / ViewVector.z; //Offset for one layer
	float Height = 1.0; //Height of the layer
	vec2 NewTexCoords = UV;
	float TextureHeight = texture(HeightTexture, NewTexCoords).r;
	
	float StepCount = 0.0;
	while(TextureHeight < Height)
	{
		Height -= Step;
		NewTexCoords += dTexCoords;
		TextureHeight = texture(HeightTexture, NewTexCoords).r;
		StepCount++;
		if (StepCount > MaxSteps)
			break;
	}
	
	vec2 PrevPoint = NewTexCoords - dTexCoords;
	float hPrev = texture(HeightTexture, PrevPoint).r - (Height + Step); // < 0
	float hCur = TextureHeight - Height; // > 0
	
	return mix(NewTexCoords, PrevPoint, hCur / (hCur - hPrev));
}