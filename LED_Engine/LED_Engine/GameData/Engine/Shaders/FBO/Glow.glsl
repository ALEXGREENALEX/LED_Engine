vec3 Glow(sampler2D InTexture, vec2 UV, float KernelSize = 4.0)
{
	vec2 SizeOfTexel = 1.0 / vec2(textureSize(InTexture, 0));
	vec3 Summ = vec3(0.0);
	vec2 LimH = vec2(0.5 - KernelSize * 0.5);
	for (float i = 0; i < KernelSize; i++)
	{
		for (float j = 0; j < KernelSize; j++)
		{
			vec2 offset = (LimH + vec2(i, j)) * SizeOfTexel;
			Summ += min(pow(texture(InTexture, UV + offset).r, 2.0), 0.3);
		}
	}
	return Summ / (KernelSize * KernelSize);
}