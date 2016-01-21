#version 330
uniform sampler2D TextureUnit0; // Depth
uniform sampler2D TextureUnit1; //vec4 Diffuse
uniform sampler2D TextureUnit2; //vec3 Normal.xy,	float EyePosition.xy
uniform sampler2D TextureUnit3; //vec3 Specular,	float Shininess
uniform sampler2D TextureUnit4; //vec3 Emissive,	float EyePosition.z
uniform sampler2D TextureUnit5; //vec3 Ka * AO_Map,	FREE

in vec2 f_UV;

//uniform vec2 ScreenSize;
uniform vec2 ClipPlanes; // zNear, zFar

#include("Light.glsl")
#include("Fog.glsl")

layout(location = 0) out vec4 FragColor;

float LinearDepth(float NonLinearDepth, float zNear, float zFar)
{
	return (2.0 * zNear) / (zFar + zNear - NonLinearDepth * (zFar - zNear));
}

/*void Test()
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
}*/

void main()
{
	//Test();
	
	vec3 Position = vec3(
		texture2D(TextureUnit2, f_UV).ba,
		texture2D(TextureUnit4, f_UV).a);
	
	float Nx = texture2D(TextureUnit2, f_UV).r;
	float Ny = texture2D(TextureUnit2, f_UV).g;
	vec3 Normal = vec3(Nx, Ny, sqrt(1.0 - Nx*Nx - Ny*Ny));
	
	vec3 Kd = texture2D(TextureUnit1, f_UV).rgb; // Ignore Alpha
	vec3 Ks = texture2D(TextureUnit3, f_UV).rgb;
	vec3 Ke = texture2D(TextureUnit4, f_UV).rgb;
	vec3 Ka = texture2D(TextureUnit5, f_UV).rgb;
	float Shininess = texture2D(TextureUnit3, f_UV).a;
	
	FragColor.rgb = CalcLight(Kd, Ks, Ka, Ke, Shininess, Normal, Position); //Light
	
	if (FogEnabled)
		FragColor.rgb = Fog(FragColor.rgb, Position);
	
	FragColor.a = 1.0;
}