using Avalonia.Collections;
using Avalonia.Threading;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.ViewModels
{
    public class RootTreeNodeModel : TreeNodeModel
    {
        public IAvaloniaReadOnlyList<TreeNodeModel> VisibleChildren => _visibleChildren;
        private AvaloniaList<TreeNodeModel> _visibleChildren = new AvaloniaList<TreeNodeModel>();

        public RootTreeNodeModel()
        {
            _root = new Root(this);
        }

        class Root : ITreeNodeRoot
        {
            private readonly RootTreeNodeModel _root;
            private bool _updateEnqueued;

            public Root(RootTreeNodeModel root)
            {
                _root = root;
            }

            public void EnqueueUpdate()
            {
                if (!_updateEnqueued)
                {
                    _updateEnqueued = true;
                    Update();
                    //Dispatcher.UIThread.Post(Update, DispatcherPriority.Background);
                }
            }                       

            private static void AppendItems(AvaloniaList<TreeNodeModel> list, TreeNodeModel node)
            {
                list.Add(node);
                if (node.IsExpanded)
                    foreach (var ch in node.Children)
                        AppendItems(list, ch);
            }

            public void Update()
            {
                _updateEnqueued = false;
                var list = new AvaloniaList<TreeNodeModel>();
                AppendItems(list, _root);

                _root._visibleChildren = new AvaloniaList<TreeNodeModel>(list);
                _root.RaisePropertyChanged(nameof(_root.VisibleChildren));
            }
        }

        public void ForceResync() => ((Root)_root).Update();
    }

    public class TreeNodeModel : PropertyChangedBase
    {       
        private List<TreeNodeModel> _children = new List<TreeNodeModel>();
        protected ITreeNodeRoot _root;
        public int Level { get; private set; }

        protected interface ITreeNodeRoot
        {
            void EnqueueUpdate();
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                SetAndRaise(ref _isExpanded, value);
                _root?.EnqueueUpdate();
            }
        }

        private bool _isDirectory;
        public bool IsDirectory
        {
            get => _isDirectory;
            set
            {
                SetAndRaise(ref _isDirectory, value);
            }
        }

        private string _itemName;
        public string ItemName
        {
            get => _itemName;
            set => SetAndRaise(ref _itemName, value);
        }

        private string _itemPath;
        public string ItemPath
        {
            get => _itemPath;
            set => SetAndRaise(ref _itemPath, value);
        }

        public IReadOnlyList<TreeNodeModel> Children => _children;

        public void InsertChild(int index, TreeNodeModel child)
        {
            if (child._root != null)
                throw new InvalidOperationException();
            _children.Insert(index, child);
            child.SetRoot(_root, Level + 1);
            _root?.EnqueueUpdate();
        }

        public void RemoveChildAt(int index)
        {
            var child = _children[index];
            _children.RemoveAt(index);
            child.SetRoot(null, 0);
            _root?.EnqueueUpdate();
        }

        protected void SetRoot(ITreeNodeRoot root, int level)
        {
            _root = root;
            Level = root == null ? -1 : level;
            foreach (var child in _children)
                child.SetRoot(root, level + 1);
        }

        public void RemoveChild(TreeNodeModel child)
        {
            var idx = _children.IndexOf(child);
            if (idx != -1)
                RemoveChildAt(idx);
        }
        public void AddChild(TreeNodeModel child) => InsertChild(_children.Count, child);
    }
}
