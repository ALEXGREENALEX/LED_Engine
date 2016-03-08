// http://forum.devmaster.net/t/ssao-feedback/21608/9
#include_StringOnce("uniform vec2 ScreenSize;")

const float SampleScale = 5.0; //5.0
const int SampleCount = 10; //10
const float DepthScale = 0.1; //0.001
const float JitterScale = 0.001; //0.1

float SSAO(sampler2D ViewPosTexture, sampler2D NormalTexture, sampler2D RandNormalTexture, vec2 UV, vec3 ViewPos, vec3 Normal)
{	
	vec3 random = texture(RandNormalTexture, UV).xyz * 2.0 - 1.0;
	
	float occlusion = 0.0;	     //< value where everything will accumulate
	float incx = SampleScale / ScreenSize.x;  //< step size in x-direction
	float incy = SampleScale / ScreenSize.y;  //< step size in y-direction
	
	/* Just variables for computing sample point */
	float dx0 = incx;
	float dy0 = incy;
	
	float ang = 0.0;   //< Angle
	float ViewPosDist = length(ViewPos);
	
	for(int i = 0; i < SampleCount; i++)
	{
		// Compute coordinates (note they're jittered!)
		float dzx = (dx0 + JitterScale * random.x) / ViewPosDist;
		float dzy = (dy0 + JitterScale * random.y) / ViewPosDist;
		
		float angle = ang * 3.1415 / 180.0;
		
		// TODO: Do this faster + more precision might be needed (although it runs nice so far)
		float dx = cos(angle) * dzx - sin(angle) * dzy;
		float dy = sin(angle) * dzx + cos(angle) * dzy;
		
		// Sample position and normal texture
		vec2 SampleUV = UV + vec2(dx, dy);
		vec3 pos = texture(ViewPosTexture, SampleUV).xyz - ViewPos;
		vec3 norm = texture(NormalTexture, SampleUV).xyz; //tex_data ???
		norm.z = sqrt(1.0 - norm.x * norm.x - norm.y * norm.y);
		
		// Normalize and scale
		vec3 v = normalize(pos);
		float d = length(pos) * DepthScale;
		
		/* Heavy wizardy and deep magic */
		/* This code is actually computing amount of occlusion between two points - note that it's absolutely necessary to fine tune the
		* constant numbers here to get the best result! */
		occlusion += (1.0 - clamp(max(dot(norm, -v), 0.0) - 0.15, 0.0, 1.0)) *
		clamp(max(dot(Normal, v), 0.0) - 0.15, 0.0, 1.0) *
		(1.0 - 1.0 / sqrt(0.5 / (d * d * 100.0) + 1.0));
		
		// Continue sampling in a "sphere" around the point
		dx0 += incx;
		dy0 += incy;
		ang += 360.0 / float(SampleCount);
	}
	
	occlusion /= float(SampleCount);  // The actual occlusion is average over samples
	return pow(1.0 - clamp(occlusion, 0.0, 1.0), 2.0);  // The result (e.g. SSAO result)
}