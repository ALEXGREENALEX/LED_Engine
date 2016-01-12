--------------------------------
	Include Function
--------------------------------
1) Include func. delete all comments from shaders!!!
2) Register is not important.
3) FileName from main Shader folder, Like: "NormalMap.glsl", "Fog\vs.glsl"
--------------------------------
Insert all text from file:
	#include("FileName")

Insert string with number N:
	#include("FileName", N)

Insert string from N to M:
	#include("FileName", N, M)

#include_once(), #includeOnce(), ... some with "once" ("#include*once*"):
	Include don't work if shader have text like in string\file

#include_string(), #includeString(), ... some with "string" ("#include*string*"):
	Include text to shader. Not used, but work!!!

It's maked just for combinations like:
	1) #IncludeOnceString("Some code"),
	2) #Include_once_string("Some code"),
	3) #InClUdE_____OnCe_____StRiNg("Some code")...