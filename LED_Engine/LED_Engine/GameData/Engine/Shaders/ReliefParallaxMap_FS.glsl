#version 330
uniform sampler2D TextureUnit0; //Diffuse
uniform sampler2D TextureUnit1; //Normal
uniform sampler2D TextureUnit2; //Specular
uniform sampler2D TextureUnit3; //Height

in vec3 LightVector;
in vec3 EyeVector;
in vec2 f_UV;

uniform vec3 ScaleBiasShin;
uniform vec3 LightDiff;
uniform vec3 LightSpec;

layout (location = 0) out vec4 FragColor;

void main()
{
	vec3 lightVector = normalize(LightVector);
	vec3 eyeVector = normalize(EyeVector);
	
	// first, find the parallax displacement by reading only the height map
	float parallaxOffset = texture2D(TextureUnit3, f_UV).r * ScaleBiasShin.x - ScaleBiasShin.y;
	vec2 newTexCoords = f_UV + (parallaxOffset * eyeVector.xy); // displace texcoords according to viewer

	// knowing the displacement, read RGB, Normal and Gloss
	vec3 diffuseColor = texture2D(TextureUnit0, newTexCoords).rgb;
	
	// build a usable normal vector
	//vec3 normal = texture2D(TextureUnit1, newTexCoords).rgb;
	//normal.xy = normal.xy * 2.0 - 1.0;
	//normal = normalize(normal);
	///// OLD /////
	vec3 normal;
	normal.xy = texture2D(TextureUnit1, newTexCoords).rg * 2.0 - 1.0;
	normal.z = sqrt(1.0 - normal.x * normal.x - normal.y * normal.y); // z = sqrt(1-x^2-y^2)
	
	float gloss = texture2D(TextureUnit2, newTexCoords).r;
	
	// tweaked phong lighting
	float lambert = max(dot(lightVector, normal), 0.0);
	FragColor = vec4(LightDiff * diffuseColor, 1.0) * lambert;

	if (lambert > 0.0)
	{
		float specular = pow(clamp(dot(reflect(-lightVector, normal), eyeVector), 0.0, 1.0), ScaleBiasShin.z);
		FragColor += vec4(LightSpec * diffuseColor, 1.0) * (specular * gloss);
	}
}