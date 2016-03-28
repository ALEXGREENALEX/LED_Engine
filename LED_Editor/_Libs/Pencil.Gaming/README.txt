Pencil.Gaming (Modefied)
=============
Pencil.Gaming is a gaming library for C#, providing support for OpenGL, 
GLFW, OpenAL and Lua. It's a stable, cross-platform, open-source 
(some prefer the term "free") alternative to libraries like XNA, 
which has pretty much died now, OpenTK, which hasn't been updated for 
about a year, and SharpDX, which is not cross-platform. A feature that 
Pencil.Gaming has over most other C# gaming libraries, is that users
 **do not need to install any redistributables besides Mono/.NET!** 

The OpenGL implementation is based on the OpenTK source code.

Functionality and stability
===========================

GLFW3
-----
| Platform       | OpenGL core     | OpenGL extensions | GLFW            | OpenAL    |
| --------------:|:---------------:|:-----------------:|:---------------:|:---------:|
| Linux 64-bit   | Stable          | Stable            | Stable          | Stable    |
| Linux 32-bit   | Stable          | Stable            | **Broken**      | Stable    |
| Windows 64-bit | Stable          | Stable            | Stable          | Stable    |
| Windows 32-bit | Stable          | Stable            | Presumed Stable | Stable    |
| Mac OS X       | Stable          | Stable            | Stable*         | Stable    |

*Both 32 and 64-bit versions provided for Mac OS X, but mono is realistically only available
 for 32-bit, so those are recommended.