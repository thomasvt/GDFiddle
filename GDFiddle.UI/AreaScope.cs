using System.Drawing;

namespace GDFiddle.UI
{
    internal class AreaScope : IDisposable
    {
        private readonly Stack<RectangleF> _areaStack;

        public AreaScope(Stack<RectangleF> areaStack)
        {
            _areaStack = areaStack;
        }

        public void Dispose()
        {
            _areaStack.Pop();
        }
    }
}
