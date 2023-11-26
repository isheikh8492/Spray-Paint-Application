using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Spray_Paint_Application.Service
{
    public class HistoryManager
    {
        public Stack<HistoryAction> UndoStack { get; } = new Stack<HistoryAction>();
        public Stack<HistoryAction> RedoStack { get; } = new Stack<HistoryAction>();

        public void AddAction(HistoryAction action)
        {
            UndoStack.Push(action);
            RedoStack.Clear();
        }

        public HistoryAction Undo()
        {
            if (UndoStack.Count > 0)
            {
                var action = UndoStack.Pop();
                RedoStack.Push(action);
                return action;
            }
            return null;
        }

        public HistoryAction Redo()
        {
            if (RedoStack.Count > 0)
            {
                var action = RedoStack.Pop();
                UndoStack.Push(action);
                return action;
            }
            return null;
        }
    }
}
