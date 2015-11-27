using System;

namespace TOSExpViewer.Core
{
    public class LuaInterface : MarshalByRefObject
    {
        public void WriteLine(String s)
        {
            Console.WriteLine(s);
        }

        public void IsInstalled(int v)
        {
            Console.WriteLine("installed! " + v);
        }
    }
}
