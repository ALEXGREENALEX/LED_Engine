#version 430

layout (binding = 0) uniform sampler2D TextureDiffuse;
layout (binding = 1) uniform sampler2D TextureHeight;
layout (binding = 2) uniform sampler2D TextureNormal;
layout (binding = 3) uniform sampler2D TextureSpecular;

in vec3 LightVector;
in vec3 EyeVector;
in vec2 TexCoord;

uniform vec3 ScaleBiasShininess;
uniform vec3 LightDiffuseColor;
uniform vec3 LightSpecularColor;

layout (location = 0) out vec4 FragColor;

void main()
{
	vec3 lightVector = normalize(LightVector);
	vec3 eyeVector = normalize(EyeVector);
	
	// first, find the parallax displacement by reading only the height map
	float parallaxOffset = texture2D(TextureHeight, TexCoord).r * ScaleBiasShininess.x - ScaleBiasShininess.y;
	vec2 newTexCoords = TexCoord + (eyeVector.xy * parallaxOffset); // displace texcoords according to viewer

	// knowing the displacement, read RGB, Normal and Gloss
	vec3 diffuseColor = texture2D(TextureDiffuse, newTexCoords).rgb;
	
	// build a usable normal vector
	//vec3 normal = texture2D(TextureNormal, newTexCoords).rgb;
	//normal.xy = normal.xy * 2.0 - 1.0;
	//normal = normalize(normal);
	///// OLD /////
	vec3 normal = vec3(0);
	normal.xy = texture2D(TextureNormal, newTexCoords).rg * 2.0 - 1.0;
	normal.z = sqrt(1.0 - normal.x * normal.x - normal.y * normal.y); // z = sqrt(1-x^2-y^2)
  
	float gloss = texture2D(TextureSpecular, newTexCoords).r;
	
	// tweaked phong lighting
	float lambert = max(dot(lightVector, normal), 0.0);
	FragColor = vec4(LightDiffuseColor * diffuseColor, 1.0) * lambert;

	if (lambert > 0.0)
	{
		float specular = pow(clamp(dot(reflect(-lightVector, normal), eyeVector), 0.0, 1.0), ScaleBiasShininess.z);
		FragColor += vec4(LightSpecularColor * diffuseColor, 1.0) * (specular * gloss);
	}
}
/*
#version 440

in vec4 eyeCordFs;
in vec4 eyeNormalFs;

out vec3 outputColor;

uniform vec4 lightPosition;

void main()
{
    vec4 s = normalize(lightPosition - eyeCordFs);
    vec4 r = reflect(-s, eyeNormalFs);
    vec4 v = normalize(-eyeCordFs);
    float spec = max(dot(v, r), 0.0);
    float diff = max(dot(eyeNormalFs, s), 0.0);

    vec3 diffColor = diff * vec3(1, 0, 0);
    vec3 specColor = pow(spec, 3) * vec3(1, 1, 1);
    vec3 ambientColor = vec3(0.1, 0.1, 0.1);

    outputColor = diffColor + 0.5 * specColor + ambientColor; 
}*/