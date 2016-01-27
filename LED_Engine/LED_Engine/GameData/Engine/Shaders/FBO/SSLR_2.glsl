//Based on: http://www.gamedev.net/topic/638355-screen-space-reflections-issues/

#include_String_Once("uniform mat4 ProjectionMatrix;")
#include_String_Once("uniform vec2 ClipPlanes;") // zNear, zFar

#include_Once("LinearDepth.glsl")

// Raytracing to get reflected color
vec4 SSLR_RayTrace(sampler2D DepthTexture, sampler2D ColorTexture, vec2 UV, vec3 reflectionVector)
{
	vec4 color = vec4(0.0f);
	float stepSize = 0.01; //rayStepSize
	
	float size = length(reflectionVector.xy);
	reflectionVector = normalize(reflectionVector / size);
	reflectionVector = reflectionVector * stepSize;
	
	vec2 sampledPosition = UV; // Current sampling position is at current fragment
	float startDepth = LinearDepth(DepthTexture, UV);
	float currentDepth = startDepth; // Current depth at current fragment
	
	// Raytrace as long as in texture space of depth buffer (between 0 and 1)
	while(sampledPosition.x <= 1.0 && sampledPosition.x >= 0.0 && sampledPosition.y <= 1.0 && sampledPosition.y >= 0.0)
	{
		// Update sampling position by adding reflection vector's xy and y components
		sampledPosition = sampledPosition + reflectionVector.xy;
		
		// Updating depth values
		currentDepth = currentDepth + reflectionVector.z * startDepth;
		float sampledDepth = LinearDepth(DepthTexture, sampledPosition);
		
		// If current depth is greater than sampled depth of depth buffer, intersection is found
		if(currentDepth > sampledDepth)
		{
			// Delta is for stop the raytracing after the first intersection is found
			// Not using delta will create "repeating artifacts"
			float delta = (currentDepth - sampledDepth);
			if(delta < 0.003f)
			{
				color = vec4(texture(ColorTexture, sampledPosition).rgb, 1.0);
				break;
			}
		}
	}
	return color;
}

vec4 SSLR(sampler2D DepthTexture, sampler2D ColorTexture, vec2 UV, vec3 Normal)
{
	// Eye position, camera is at (0, 0, 0), we look along negative z, add near plane to correct parallax
	vec3 eyePosition = normalize(vec3(0, 0, ClipPlanes.x));
	vec3 reflectionVector = vec3(ProjectionMatrix * vec4(reflect(-eyePosition, Normal), 0.0));
	
	// Call raytrace to get reflected color, and return it
	return SSLR_RayTrace(DepthTexture, ColorTexture, UV, reflectionVector);
}