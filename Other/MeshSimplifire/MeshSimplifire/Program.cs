using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MeshSimplifire
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> L = new List<string>(File.ReadAllLines(args[0]));
            List<string> L2 = new List<string>();

            for (int i = 0; i < L.Count; i++)
            {
                try
                {
                    if (L[i].Substring(0, 2).ToLower() != "s ")
                        if (L[i].Substring(0, 2).ToLower() != "g ")
                            if (L[i].Substring(0, 7).ToLower() != "mtllib ")
                                if (L[i][0] != '#')
                                    L2.Add(L[i]);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                }
            }

            File.WriteAllLines(Path.GetFileNameWithoutExtension(args[0]) + "_new" + Path.GetExtension(args[0]), L2.ToArray());
        }
    }
}
