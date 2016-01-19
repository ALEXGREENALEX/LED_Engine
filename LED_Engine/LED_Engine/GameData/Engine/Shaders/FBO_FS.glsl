#version 330
uniform sampler2D TextureUnit0; //Depth
uniform sampler2D TextureUnit1; //vec3 Diffuse,		float EyePosition.x
uniform sampler2D TextureUnit2; //vec3 Normal,		float EyePosition.y
uniform sampler2D TextureUnit3; //vec3 Emissive,	float EyePosition.z
uniform sampler2D TextureUnit4; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit5; //vec3 Ka * AO_Map,	NOTHING

in vec2 f_UV;

uniform vec2 ScreenSize;
uniform vec2 ClipPlanes; // zNear, zFar
uniform mat4 InvProjMatrix;

#include("Light\Light.glsl")

layout(location = 0) out vec4 FragColor;

float LinearDepth(float NonLinearDepth, float zNear, float zFar)
{
	return (2.0 * zNear) / (zFar + zNear - NonLinearDepth * (zFar - zNear));
}

void Test()
{
	if(f_UV.y > 0.5)
		FragColor = vec4(texture2D(TextureUnit1, f_UV).rgb, 1.0);
	else
	{
		if(f_UV.x < 0.2)
		{
			float Color = LinearDepth(texture(TextureUnit0, f_UV).x, ClipPlanes.x, ClipPlanes.y);
			FragColor = vec4(Color, Color, Color, 1.0);
		}
		if(f_UV.x > 0.2)
			if(f_UV.x < 0.4)
				FragColor = vec4(texture2D(TextureUnit2, f_UV).rgb, 1.0);
		if(f_UV.x > 0.4)
			if(f_UV.x < 0.6)
				FragColor = vec4(texture2D(TextureUnit3, f_UV).rgb, 1.0);
		if(f_UV.x > 0.6)
			if(f_UV.x < 0.8)
				FragColor = vec4(texture2D(TextureUnit4, f_UV).rgb, 1.0);
		if(f_UV.x > 0.8)
			FragColor = vec4(texture2D(TextureUnit5, f_UV).rga, 1.0);
	}
}

void main()
{
	//Test();
	
	vec3 Position = vec3(
		texture2D(TextureUnit1, f_UV).a,
		texture2D(TextureUnit2, f_UV).a,
		texture2D(TextureUnit3, f_UV).a);
	
	vec3 Normal = normalize(texture2D(TextureUnit2, f_UV).rgb);
	vec3 Kd = texture2D(TextureUnit1, f_UV).rgb;
	vec3 Ks = texture2D(TextureUnit4, f_UV).rgb;
	vec3 Ke = texture2D(TextureUnit3, f_UV).rgb;
	float Shininess = texture2D(TextureUnit4, f_UV).a;
	vec3 Ka = texture2D(TextureUnit5, f_UV).rgb;
	
	FragColor = vec4(CalcLight(Kd, Ks, Shininess, Ka, Ke, Normal, Position), 1.0);
}