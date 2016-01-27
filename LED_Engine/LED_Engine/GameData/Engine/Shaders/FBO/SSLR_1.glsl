//Based on: https://www.opengl.org/discussion_boards/showthread.php/181724-screen-space-reflections

#include_String_Once("uniform mat4 ProjectionMatrix;")

// Consts should help improve performance
const float rayStep = 0.25;
const float minRayStep = 0.1;
const float maxSteps = 20;
const float searchDist = 5.0;
const float searchDistInv = 0.2;
const int numBinarySearchSteps = 5;
const float ReflectionSpecularFalloffExponent = 3.0;

vec3 SSLR_BinarySearch(sampler2D gPosition, vec3 dir, inout vec3 hitCoord, out float dDepth)
{
	float depth;
	
	for(int i = 0; i < numBinarySearchSteps; i++)
	{
		vec4 projectedCoord = ProjectionMatrix * vec4(hitCoord, 1.0);
		projectedCoord.xy /= projectedCoord.w;
		projectedCoord.xy = projectedCoord.xy * 0.5 + 0.5;

		depth = texture(gPosition, projectedCoord.xy).z;
		dDepth = hitCoord.z - depth;

		if(dDepth > 0.0)
			hitCoord += dir;

		dir *= 0.5;
		hitCoord -= dir;	
	}
	
	vec4 projectedCoord = ProjectionMatrix * vec4(hitCoord, 1.0);
	projectedCoord.xy /= projectedCoord.w;
	projectedCoord.xy = projectedCoord.xy * 0.5 + 0.5;

	return vec3(projectedCoord.xy, depth);
}

vec4 SSLR_RayCast(sampler2D gPosition, vec3 dir, inout vec3 hitCoord, out float dDepth)
{
	dir *= rayStep;
	float depth;

	for(int i = 0; i < maxSteps; i++)
	{
		hitCoord += dir;
		
		vec4 projectedCoord = ProjectionMatrix * vec4(hitCoord, 1.0);
		projectedCoord.xy /= projectedCoord.w;
		projectedCoord.xy = projectedCoord.xy * 0.5 + 0.5;

		depth = texture(gPosition, projectedCoord.xy).z;
		dDepth = hitCoord.z - depth;

		if(dDepth < 0.0)
			return vec4(SSLR_BinarySearch(gPosition, dir, hitCoord, dDepth), 1.0);
	}
	
	return vec4(0.0, 0.0, 0.0, 0.0);
}

vec4 SSLR(sampler2D gPosition, sampler2D gEffect, vec3 viewPos, vec3 viewNormal, float specular)
{
	if(specular == 0.0)
		return vec4(0.0);

	// Reflection vector
	vec3 reflected = normalize(reflect(viewPos, viewNormal));

	// Ray cast
	vec3 hitPos = viewPos;
	float dDepth;
	vec4 coords = SSLR_RayCast(gPosition, reflected * max(minRayStep, -viewPos.z), hitPos, dDepth);
	vec2 dCoords = abs(vec2(0.5, 0.5) - coords.xy);
	float screenEdgefactor = clamp(1.0 - (dCoords.x + dCoords.y), 0.0, 1.0);

	// Get color
	return vec4(texture(gEffect, coords.xy).rgb,
		pow(specular, ReflectionSpecularFalloffExponent) *
		screenEdgefactor * clamp(-reflected.z, 0.0, 1.0) *
		clamp((searchDist - length(viewPos - hitPos)) * searchDistInv, 0.0, 1.0) * coords.w);
}