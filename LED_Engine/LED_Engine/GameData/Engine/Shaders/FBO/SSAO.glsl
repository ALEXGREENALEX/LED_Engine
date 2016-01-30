//Based on: http://blog.evoserv.at/index.php/2012/12/hemispherical-screen-space-ambient-occlusion-ssao-for-deferred-renderers-using-openglglsl/

uniform float distanceThreshold = 5.0;
uniform vec2 filterRadius = vec2(10.0 / 800.0, 10.0 / 600.0);

const vec2 PoissonSamples[] = vec2[]( // These are the Poisson Disk Samples
								vec2(-0.94201624, -0.39906216 ),
								vec2( 0.94558609, -0.76890725 ),
								vec2(-0.094184101,-0.92938870 ),
								vec2( 0.34495938,  0.29387760 ),
								vec2(-0.91588581,  0.45771432 ),
								vec2(-0.81544232, -0.87912464 ),
								vec2(-0.38277543,  0.27676845 ),
								vec2( 0.97484398,  0.75648379 ),
								vec2( 0.44323325, -0.97511554 ),
								vec2( 0.53742981, -0.47373420 ),
								vec2(-0.26496911, -0.41893023 ),
								vec2( 0.79197514,  0.19090188 ),
								vec2(-0.24188840,  0.99706507 ),
								vec2(-0.81409955,  0.91437590 ),
								vec2( 0.19984126,  0.78641367 ),
								vec2( 0.14383161, -0.14100790 ));

float SSAO(sampler2D Position, vec2 UV, vec3 viewPos, vec3 viewNormal)
{
	float ambientOcclusion = 0.0;
	for (int i = 0; i < PoissonSamples.length(); i++)
	{
		// sample at an offset specified by the current Poisson-Disk sample and scale it by a radius (has to be in Texture-Space)
		vec2 sampleTexCoord = UV + PoissonSamples[i] * filterRadius;
		vec3 samplePos = texture(Position, sampleTexCoord).xyz;
		vec3 sampleDir = normalize(samplePos - viewPos);
		
		// angle between SURFACE-NORMAL and SAMPLE-DIRECTION (vector from SURFACE-POSITION to SAMPLE-POSITION)
		float NdotS = max(dot(viewNormal, sampleDir), 0.0);
		// distance between SURFACE-POSITION and SAMPLE-POSITION
		float VPdistSP = distance(viewPos, samplePos);
		
		// a = distance function
		float a = 1.0 - smoothstep(distanceThreshold, distanceThreshold * 2.0, VPdistSP);
		// b = dot-Product
		float b = NdotS;
 
		ambientOcclusion += (a * b);
	}
	return 1.0 - ambientOcclusion / PoissonSamples.length();
}

float SSAO_Blur(sampler2D InTexture, vec2 UV, float KernelSize)
{
	vec2 SizeOfTexel = 1.0 / vec2(textureSize(InTexture, 0));
	float Summ = 0.0;
	vec2 LimH = vec2(0.5 - KernelSize * 0.5);
	for (float i = 0; i < KernelSize; i++)
	{
		for (float j = 0; j < KernelSize; j++)
		{
			vec2 offset = (LimH + vec2(i, j)) * SizeOfTexel;
			Summ += texture(InTexture, UV + offset).r;
		}
	}
	return Summ / (KernelSize * KernelSize);
}

/*the distanceThreshold is a variable that regulates the "strength" of the AO-term based 
on the distance between the current surface position ("viewPos" in the shader code) and
the sample position (“samplePos” in the shader). If a sample is near to the surface position
I want it to have more influence (more AO influence) than a pixel that is further away. So
the distanceThreshold has to be a World-Space distance (or more accurately a View-Space
distance, but since their scale is the same it shouldn’t make a difference). In my scene
the value is 5, but again, it depends or your scene’s size and you most likely have to
experiment to get a value that works for you.

The filterRadius scales the offset given by the Poisson-Disk-samples in texture-space.
Texture space is the [0,1]-space in which you sample your texture. The Poisson-Disk samples
are distributed in a [-1,1]-space as you can see from the values. If we would use these values
to sample the texture, you would basically sample all over the texture and not just within the
vicinity of the current sample. But of course, the AO-term should only be sampled in close
vicinity to the current sample. So the filterRadius is actually a scale in screen-space where
the samples should be placed. In my implementation I use 10 pixels in both directions
(horizontal and vertical). It is calculated as (10 / screenWidth, 10 / screenHeight).
If you scale the Poisson-Disk samples with this value you make sure that your resulting
"sampleTexCoord" are within a 10-pixel radius of your current sample.*/