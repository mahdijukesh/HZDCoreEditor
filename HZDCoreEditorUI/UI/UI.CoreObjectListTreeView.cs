﻿using BrightIdeasSoftware;
using HZDCoreEditorUI.Util;
using System.Collections.Generic;
using System.Linq;

namespace HZDCoreEditorUI.UI
{
    public class CoreObjectListTreeView : TreeListView
    {
        private readonly List<TreeDataNode> _children = new List<TreeDataNode>();
        private readonly OLVColumn[] _defaultColumns;

        public CoreObjectListTreeView()
        {
            CanExpandGetter = CanExpandGetterHandler;
            ChildrenGetter = ChildrenGetterHandler;

            // Columns are hardcoded. Keep them cached in case the view needs to be reset.
            _defaultColumns = new OLVColumn[3];

            _defaultColumns[0] = new OLVColumn("Object", nameof(TreeObjectNode.TypeName))
            {
                Width = 200,
                IsEditable = false,
            };

            _defaultColumns[1] = new OLVColumn("Name", nameof(TreeObjectNode.Name))
            {
                Width = 200,
                IsEditable = false,
            };

            _defaultColumns[2] = new OLVColumn("UUID", nameof(TreeObjectNode.UUID))
            {
                Width = 300,
                IsEditable = false,
            };

            CreateColumns();
        }

        public void RebuildTreeFromObjects(List<object> baseObjects)
        {
            // Sort object list into each category based on the type name
            var categorizedObjects = new Dictionary<string, List<object>>();

            foreach (var obj in baseObjects)
            {
                string typeString = obj.GetType().GetFriendlyName();

                if (!categorizedObjects.TryGetValue(typeString, out List<object> categoryList))
                {
                    categoryList = new List<object>();
                    categorizedObjects.Add(typeString, categoryList);
                }

                categoryList.Add(obj);
            }

            // Register list view categories
            var treeViewRoots = new List<TreeObjectNode>();

            foreach (string key in categorizedObjects.Keys.OrderBy(x => x))
                treeViewRoots.Add(new TreeObjectNode(key, categorizedObjects[key]));

            Roots = treeViewRoots;
        }

        private void CreateColumns()
        {
            AllColumns.AddRange(_defaultColumns);
            RebuildColumns();
        }

        private static bool CanExpandGetterHandler(object model)
        {
            return (model as TreeObjectNode).Children != null;
        }

        private static IEnumerable<TreeObjectNode> ChildrenGetterHandler(object model)
        {
            return (model as TreeObjectNode).Children;
        }
    }
}