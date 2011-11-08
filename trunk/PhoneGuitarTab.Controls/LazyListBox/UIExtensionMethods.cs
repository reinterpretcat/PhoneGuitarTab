// Type: UIExtensionMethods.ExtensionMethods
// Assembly: UIExtensionMethods, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Assembly location: G:\Development\_PhoneGuitarTabGit\ExternalLibs\LazyListBox\UIExtensionMethods.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PhoneGuitarTab.Controls.Extensions
{
    public static class ExtensionMethods
    {
        private static List<Action> workItems;

        public static FrameworkElement FindVisualChild(this FrameworkElement root, string name)
        {
            FrameworkElement frameworkElement1 = root.FindName(name) as FrameworkElement;
            if (frameworkElement1 != null)
                return frameworkElement1;
            foreach (FrameworkElement frameworkElement2 in ExtensionMethods.GetVisualDescendents(root))
            {
                FrameworkElement frameworkElement3 = frameworkElement2.FindName(name) as FrameworkElement;
                if (frameworkElement3 != null)
                    return frameworkElement3;
            }
            return (FrameworkElement)null;
        }

        public static FrameworkElement GetVisualParent(this FrameworkElement node)
        {
            try
            {
                return VisualTreeHelper.GetParent((DependencyObject)node) as FrameworkElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception while trying to get parent. " + ex.Message);
            }
            return (FrameworkElement)null;
        }

        public static FrameworkElement GetVisualChild(this FrameworkElement node, int index)
        {
            try
            {
                return VisualTreeHelper.GetChild((DependencyObject)node, index) as FrameworkElement;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Concat(new object[4]
        {
          (object) "Exception while trying to get child index ",
          (object) index,
          (object) ". ",
          (object) ex.Message
        }));
            }
            return (FrameworkElement)null;
        }

        public static IEnumerable<FrameworkElement> GetVisualChildren(this FrameworkElement root)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount((DependencyObject)root); ++i)
                yield return VisualTreeHelper.GetChild((DependencyObject)root, i) as FrameworkElement;
        }

        public static IEnumerable<FrameworkElement> GetVisualAncestors(this FrameworkElement node)
        {
            for (FrameworkElement parent = ExtensionMethods.GetVisualParent(node); parent != null; parent = ExtensionMethods.GetVisualParent(parent))
                yield return parent;
        }

        public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> list, T head)
        {
            yield return head;
            foreach (T obj in list)
                yield return obj;
        }

        public static VisualStateGroup GetVisualStateGroup(this FrameworkElement root, string groupName)
        {
            foreach (FrameworkElement frameworkElement in ExtensionMethods.PrependWith<FrameworkElement>(ExtensionMethods.GetVisualAncestors(root), root))
            {
                foreach (object obj in (IEnumerable)VisualStateManager.GetVisualStateGroups(frameworkElement))
                {
                    VisualStateGroup visualStateGroup = obj as VisualStateGroup;
                    if (visualStateGroup != null && visualStateGroup.Name == groupName)
                        return visualStateGroup;
                }
            }
            return (VisualStateGroup)null;
        }

        public static bool TestVisibility(this FrameworkElement item, FrameworkElement viewport, Orientation orientation, bool wantVisible)
        {
            GeneralTransform generalTransform = item.TransformToVisual((UIElement)viewport);
            Point point1 = generalTransform.Transform(new Point(0.0, 0.0));
            Point point2 = generalTransform.Transform(new Point(item.ActualWidth, item.ActualHeight));
            double num1;
            double num2;
            double num3;
            double num4;
            if (orientation == Orientation.Vertical)
            {
                num1 = point1.Y;
                num2 = point2.Y;
                num3 = 0.0;
                num4 = Math.Min(viewport.ActualHeight, double.IsNaN(viewport.Height) ? double.PositiveInfinity : viewport.Height);
            }
            else
            {
                num1 = point1.X;
                num2 = point2.X;
                num3 = 0.0;
                num4 = Math.Min(viewport.ActualWidth, double.IsNaN(viewport.Width) ? double.PositiveInfinity : viewport.Width);
            }
            bool flag = wantVisible;
            if (num1 >= num4 || num2 <= num3)
                flag = !wantVisible;
            return flag;
        }

        public static IEnumerable<T> GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation) where T : FrameworkElement
        {
            return Enumerable.TakeWhile<T>(Enumerable.SkipWhile<T>(items, (Func<T, bool>)(item => ExtensionMethods.TestVisibility((FrameworkElement)item, viewport, orientation, false))), (Func<T, bool>)(item => ExtensionMethods.TestVisibility((FrameworkElement)item, viewport, orientation, true)));
        }

        public static void GetVisibleItems<T>(this IEnumerable<T> items, FrameworkElement viewport, Orientation orientation, out List<T> beforeItems, out List<T> visibleItems, out List<T> afterItems) where T : FrameworkElement
        {
            beforeItems = new List<T>();
            visibleItems = new List<T>();
            afterItems = new List<T>();
            ExtensionMethods.VisibleSearchMode visibleSearchMode = ExtensionMethods.VisibleSearchMode.Before;
            foreach (T obj in items)
            {
                switch (visibleSearchMode)
                {
                    case ExtensionMethods.VisibleSearchMode.Before:
                        if (ExtensionMethods.TestVisibility((FrameworkElement)obj, viewport, orientation, false))
                        {
                            beforeItems.Add(obj);
                            break;
                        }
                        else
                        {
                            visibleItems.Add(obj);
                            visibleSearchMode = ExtensionMethods.VisibleSearchMode.During;
                            break;
                        }
                    case ExtensionMethods.VisibleSearchMode.During:
                        if (ExtensionMethods.TestVisibility((FrameworkElement)obj, viewport, orientation, true))
                        {
                            visibleItems.Add(obj);
                            break;
                        }
                        else
                        {
                            afterItems.Add(obj);
                            visibleSearchMode = ExtensionMethods.VisibleSearchMode.After;
                            break;
                        }
                    default:
                        afterItems.Add(obj);
                        break;
                }
            }
        }

        public static IEnumerable<FrameworkElement> GetVisualDescendents(this FrameworkElement root)
        {
            Queue<IEnumerable<FrameworkElement>> toDo = new Queue<IEnumerable<FrameworkElement>>();
            toDo.Enqueue(ExtensionMethods.GetVisualChildren(root));
            while (toDo.Count > 0)
            {
                IEnumerable<FrameworkElement> children = toDo.Dequeue();
                foreach (FrameworkElement root1 in children)
                {
                    yield return root1;
                    toDo.Enqueue(ExtensionMethods.GetVisualChildren(root1));
                }
            }
        }

        public static IEnumerable<T> GetVisualDescendents<T>(this FrameworkElement root, bool allAtSameLevel) where T : FrameworkElement
        {
            bool found = false;
            foreach (FrameworkElement frameworkElement in ExtensionMethods.GetVisualDescendents(root))
            {
                if (frameworkElement is T)
                {
                    found = true;
                    yield return frameworkElement as T;
                }
                else if (found && allAtSameLevel)
                    break;
            }
        }

        [Conditional("DEBUG")]
        public static void PrintDescendentsTree(this FrameworkElement root)
        {
            List<string> results = new List<string>();
            ExtensionMethods.GetChildTree(root, "", "  ", results);
            foreach (string message in results)
                Debug.WriteLine(message);
        }

        [Conditional("DEBUG")]
        private static void GetChildTree(this FrameworkElement root, string prefix, string addPrefix, List<string> results)
        {
            string str = (!string.IsNullOrEmpty(root.Name) ? root.Name : "[Anon]") + " " + root.GetType().Name;
            results.Add(prefix + str);
            foreach (FrameworkElement root1 in ExtensionMethods.GetVisualChildren(root))
                ExtensionMethods.GetChildTree(root1, prefix + addPrefix, addPrefix, results);
        }

        [Conditional("DEBUG")]
        public static void PrintAncestorTree(this FrameworkElement node)
        {
            List<string> children = new List<string>();
            ExtensionMethods.GetAncestorVisualTree(node, children);
            string str1 = "";
            foreach (string str2 in children)
            {
                Debug.WriteLine(str1 + str2);
                str1 = str1 + "  ";
            }
        }

        [Conditional("DEBUG")]
        private static void GetAncestorVisualTree(this FrameworkElement node, List<string> children)
        {
            string str = (string.IsNullOrEmpty(node.Name) ? "[Anon]" : node.Name) + ": " + node.GetType().Name;
            children.Insert(0, str);
            FrameworkElement visualParent = ExtensionMethods.GetVisualParent(node);
            if (visualParent == null)
                return;
            ExtensionMethods.GetAncestorVisualTree(visualParent, children);
        }

        public static double GetVerticalScrollOffset(this ListBox list)
        {
            return (ExtensionMethods.FindVisualChild((FrameworkElement)list, "ScrollViewer") as ScrollViewer).VerticalOffset;
        }

        public static double GetHorizontalScrollOffset(this ListBox list)
        {
            return (ExtensionMethods.FindVisualChild((FrameworkElement)list, "ScrollViewer") as ScrollViewer).HorizontalOffset;
        }

        private static bool Internal_SetVerticalScrollOffset(this ListBox list, double offset)
        {
            ScrollViewer scrollViewer = ExtensionMethods.FindVisualChild((FrameworkElement)list, "ScrollViewer") as ScrollViewer;
            if (scrollViewer == null)
                return false;
            scrollViewer.ScrollToVerticalOffset(offset);
            if (list is ISupportOffsetChanges)
                (list as ISupportOffsetChanges).VerticalOffsetChanged(offset);
            return true;
        }

        private static bool Internal_SetHorizontalScrollOffset(this ListBox list, double offset)
        {
            ScrollViewer scrollViewer = ExtensionMethods.FindVisualChild((FrameworkElement)list, "ScrollViewer") as ScrollViewer;
            if (scrollViewer == null)
                return false;
            scrollViewer.ScrollToHorizontalOffset(offset);
            if (list is ISupportOffsetChanges)
                (list as ISupportOffsetChanges).HorizontalOffsetChanged(offset);
            return true;
        }

        public static void SetVerticalScrollOffset(this ListBox list, double offset)
        {
            if (ExtensionMethods.Internal_SetVerticalScrollOffset(list, offset))
                return;
            ExtensionMethods.ScheduleOnNextRender((Action)(() => ExtensionMethods.Internal_SetVerticalScrollOffset(list, offset)));
        }

        public static void SetHorizontalScrollOffset(this ListBox list, double offset)
        {
            if (ExtensionMethods.Internal_SetHorizontalScrollOffset(list, offset))
                return;
            ExtensionMethods.ScheduleOnNextRender((Action)(() => ExtensionMethods.Internal_SetHorizontalScrollOffset(list, offset)));
        }

        public static void ScheduleOnNextRender(Action action)
        {
            if (ExtensionMethods.workItems == null)
            {
                ExtensionMethods.workItems = new List<Action>();
                CompositionTarget.Rendering += new EventHandler(ExtensionMethods.DoWorkOnRender);
            }
            ExtensionMethods.workItems.Add(action);
        }

        private static void DoWorkOnRender(object sender, EventArgs args)
        {
            Debug.WriteLine("DoWorkOnRender running... if you see this message a lot then something is wrong!");
            CompositionTarget.Rendering -= new EventHandler(ExtensionMethods.DoWorkOnRender);
            List<Action> list = ExtensionMethods.workItems;
            ExtensionMethods.workItems = (List<Action>)null;
            foreach (Action action in list)
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    Debug.WriteLine("Exception while doing work for " + action.Method.Name + ". " + ex.Message);
                }
            }
        }

        private enum VisibleSearchMode
        {
            Before,
            During,
            After,
        }
    }
}
