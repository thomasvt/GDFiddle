using System.Drawing;

namespace GDFiddle.UI
{
    internal class AreaScope : IDisposable
    {
        private readonly Stack<Rectangle> _areaStack;

        public AreaScope(Stack<Rectangle> areaStack)
        {
            _areaStack = areaStack;
        }

        public void Dispose()
        {
            _areaStack.Pop();
        }
    }
}
