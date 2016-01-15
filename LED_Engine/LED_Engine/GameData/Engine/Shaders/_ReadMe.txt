--------------------------------
	TextureUnits
--------------------------------
Part of the Fragment Shader with samplers, like:
	"uniform sampler2D TextureUnit0;"
must be ONLY WITH TextureUnit0, TextureUnit1 ... TextureUnitN,
where N = 'MaxTextureImageUnits' (or change "TextureUnit" string in Engine).


--------------------------------
	Include Function
--------------------------------
1) Include func. delete all comments from shaders!!!
2) Register is not important.
3) FileName from main Shader, Like: "NormalMap.glsl", "Fog\vs.glsl"
--------------------------------
Insert all text from file:
	#include("FileName")

Insert string with number N:
	#include("FileName", N)

Insert strings from N to M:
	#include("FileName", N, M)

#include_once(), #includeOnce(), ... some with "once" ("#include*once*"):
	Include doesn't work if shader has text like in string\file

#include_string(), #includeString(), ... some with "string" ("#include*string*"):
	Include text to shader. Not usable, but work!!!

It's maked just for combinations like:
	1) #IncludeOnceString("Some code")
	2) #Include_once_string("Some code")
	3) #InClUdE_____OnCe_____StRiNg("Some code")...