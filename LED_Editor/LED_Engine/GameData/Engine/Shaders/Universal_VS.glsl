#version 330
layout(location = 0) in vec3 v_Position;
layout(location = 1) in vec3 v_Normal;
layout(location = 2) in vec2 v_UV;
layout(location = 3) in vec3 v_Tan;

uniform mat4 MVP;
uniform mat4 ModelView;
uniform mat3 NormalMatrix;

out vec3 f_EyePosition; // Position in Eye space
out vec2 f_UV;			// Texture Coords
out mat3 f_TBN;			// Inversed TBN Matrix (for convert Tangent to Eye space)

void main()
{
	// Transform normals and tangents and Bi-normals to Eye space
	vec3 N = normalize(NormalMatrix * v_Normal);
	vec3 T = normalize(NormalMatrix * v_Tan);
	vec3 B = normalize(cross(N, T));
	f_TBN = mat3(T, B, N); // Transposed(Inversed) TBN Matrix, if (Ortoganal) then Transposed(M) = Inversed(M)!
	
	f_UV = v_UV;
	f_EyePosition = vec3(ModelView * vec4(v_Position, 1.0));
	gl_Position = MVP * vec4(v_Position, 1.0);
}