using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Cultiverse.UI
{
    public static class TouchDragDrop
    {
        public static event EventHandler<TouchEventArgs> Drop;

        public static void FireDrop(Object sender, TouchEventArgs touchEventArgs)
        {
            var evt = Drop;
            if (evt != null)
                evt(sender, touchEventArgs);
        }

        public static event EventHandler<TouchEventArgs> Drag;

        public static void FireDrag(Object sender, TouchEventArgs touchEventArgs)
        {
            var evt = Drag;
            if (evt != null)
                evt(sender, touchEventArgs);
        }
    }
}
