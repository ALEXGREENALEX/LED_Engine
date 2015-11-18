float FogDist = length(vec3(MVP * vec4(VertexPosition, 1.0)));
FogFactor =	(FogMaxDist - FogDist) /
			(FogMaxDist - FogMinDist);
FogFactor = clamp(FogFactor, 0.0, 1.0);