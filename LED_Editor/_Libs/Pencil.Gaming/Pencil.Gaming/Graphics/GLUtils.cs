#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using Pencil.Gaming.MathUtils;

namespace Pencil.Gaming.Graphics {
	public delegate T[] TArrayFromRetrievedData<T>(Vector4[] vertexs,Vector3[] normals,Vector2[] texCoords);

	public static partial class GL {
		public static class Utils {
			#region Image Loading

			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="path">The path to the image file.</param>
			public static int LoadImage(string path) {
				using (Bitmap bmp = new Bitmap(path)) {
					return LoadImage(bmp);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="path">The path to the image file.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			public static int LoadImage(string path, bool square) {
				using (Bitmap bmp = new Bitmap(path)) {
					return LoadImage(bmp, square);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="path">The path to the image file.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			/// <param name="tmin">The required minimization filter.</param>
			/// <param name="tmag">The required maximalization filter.</param> 
			public static int LoadImage(string path, bool square, TextureMinFilter tmin, TextureMagFilter tmag) {
				using (Bitmap bmp = new Bitmap(path)) {
					return LoadImage(bmp, square, tmin, tmag);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="file">The stream containing the image bytes.</param>
			public static int LoadImage(Stream file) {
				using (Bitmap bmp = new Bitmap(file)) {
					return LoadImage(bmp);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="file">The stream containing the image bytes.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			public static int LoadImage(Stream file, bool square) {
				using (Bitmap bmp = new Bitmap(file)) {
					return LoadImage(bmp, square);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="file">The stream containing the image bytes.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			/// <param name="tmin">The required minimization filter.</param>
			/// <param name="tmag">The required maximalization filter.</param> 
			public static int LoadImage(Stream file, bool square, TextureMinFilter tmin, TextureMagFilter tmag) {
				using (Bitmap bmp = new Bitmap(file)) {
					return LoadImage(bmp, square, tmin, tmag);
				}
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="bmp">The Bitmap representing the image.</param>
			public static int LoadImage(Bitmap bmp) {
				return LoadImage(bmp, true, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="bmp">The Bitmap representing the image.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			public static int LoadImage(Bitmap bmp, bool square) {
				return LoadImage(bmp, square, TextureMinFilter.Nearest, TextureMagFilter.Nearest);
			}
			/// <summary>
			/// Loads an image.
			/// </summary>
			/// <returns>The OpenGL ID representing the image.</returns>
			/// <param name="bmp">The Bitmap representing the image.</param>
			/// <param name="square">A value indicating whether the image is a power of two texture.</param>
			/// <param name="tmin">The required minimization filter.</param>
			/// <param name="tmag">The required maximalization filter.</param> 
			public static int LoadImage(Bitmap bmp, bool square, TextureMinFilter tmin, TextureMagFilter tmag) {
				BitmapData bmpData = bmp.LockBits(
					new System.Drawing.Rectangle(Point.Empty, bmp.Size), 
					ImageLockMode.ReadOnly, 
					System.Drawing.Imaging.PixelFormat.Format32bppArgb
				);
				
				int result;
				TextureTarget target = (square ? TextureTarget.Texture2D : TextureTarget.TextureRectangle);

				try {
					GL.GenTextures(1, out result);
					GL.BindTexture(target, result);
					GL.TexParameter(target, TextureParameterName.TextureMagFilter, (int)tmag);
					GL.TexParameter(target, TextureParameterName.TextureMinFilter, (int)tmin);
					GL.TexImage2D(
						target, 
						0, 
						PixelInternalFormat.Rgba,
						bmp.Width,
						bmp.Height,
						0,
						Pencil.Gaming.Graphics.PixelFormat.Bgra,
						PixelType.UnsignedByte,
						bmpData.Scan0
					);
					GL.BindTexture(target, 0);
				} finally {
					bmp.UnlockBits(bmpData);
				}

				return result;
			}

			#endregion

			#region Cylinder Generation

			/// <summary>
			/// Draws the cylinder.
			/// </summary>
			/// <param name="lod">Level of detail with which the cylinder was created.</param>
			public static void DrawCylinderArrays(int lod) {
				GL.DrawArrays(BeginMode.TriangleStrip, 0, (lod + 1) * 2);
				GL.DrawArrays(BeginMode.TriangleFan, (lod + 1) * 2, lod);
				GL.DrawArrays(BeginMode.TriangleFan, (lod + 1) * 2 + lod, lod);
			}

			/// <summary>
			/// Generates a cylinder.
			/// </summary>
			/// <param name="vertices">Vertices.</param>
			/// <param name="normals">Normals.</param>
			/// <param name="height">Height.</param>
			/// <param name="radius">Radius.</param>
			/// <param name="lod">Level of detail.</param>
			public static void CreateCylinder(out Vector4[] vertices, out Vector3[] normals, float height, float radius, int lod) {
				int sideVCount = (lod + 1) * 2;
				int topVCount = lod;

				int totalVCount = sideVCount + topVCount * 2;

				vertices = new Vector4[totalVCount];
				normals = new Vector3[totalVCount];

				for (int i = 0; i <= lod; ++i) {
                    float rad = (i / (float)lod) * MathHelper.TwoPi;
					float xNorm = (float)Math.Cos(rad);
					float zNorm = (float)Math.Sin(rad);
					float xVert = xNorm * radius;
					float zVert = zNorm * radius;

					normals[2 * i + 0] = new Vector3(xNorm, 0f, zNorm);
					normals[2 * i + 1] = new Vector3(xNorm, 0f, zNorm);
					vertices[2 * i + 0] = new Vector4(xVert, -height / 2f, zVert, 1f);
					vertices[2 * i + 1] = new Vector4(xVert, height / 2f, zVert, 1f);

					if (i != lod) {
						normals[sideVCount + i] = -Vector3.UnitY;
						vertices[sideVCount + i] = new Vector4(xVert, -height / 2f, zVert, 1f);

						normals[sideVCount + topVCount + i] = Vector3.UnitY;
						vertices[sideVCount + topVCount + i] = new Vector4(xVert, height / 2f, zVert, 1f);
					}
				}
			}

			#endregion
		
			public static Vector3 UnProject(Vector3 winCoords, Matrix4 modelView, Matrix4 proj, Rectanglei viewport) {
				Matrix4 finalMatrix = Matrix4.Invert(modelView * proj);

				Vector4 inVec = new Vector4(winCoords, 1f);
				inVec.X = (inVec.X - viewport.X) / viewport.Width;
				inVec.Y = (inVec.Y - viewport.Y) / viewport.Height;

				inVec.X = inVec.X * 2 - 1;
				inVec.Y = inVec.Y * 2 - 1;
				inVec.Z = inVec.Z * 2 - 1;

				Vector4 outVec = Vector4.Transform(inVec, finalMatrix);
				if (Math.Abs(outVec.Z) < 10f * float.Epsilon) {
					throw new Exception();
				}

				outVec.X /= outVec.W;
				outVec.Y /= outVec.W;
				outVec.Z /= outVec.W;
				return outVec.Xyz;
			}
		}
	}
}

