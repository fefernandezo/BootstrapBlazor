// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components.Rendering;

namespace BootstrapBlazor.Components;

/// <summary>
/// Tree 组件
/// </summary>
public partial class Tree
{
    /// <summary>
    /// 获得/设置 Tree 组件实例引用
    /// </summary>
    private ElementReference TreeElement { get; set; }

    [NotNull]
    private string? GroupName { get; set; }

    /// <summary>
    /// 是否允许拖拽，默认不允许
    /// </summary>
    [Parameter] public bool CanDrag { get; set; } = false;

    /// <summary>
    /// 获得 按钮样式集合
    /// </summary>
    private string? ClassString => CssBuilder.Default("tree")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// 获得 Loading 样式集合
    /// </summary>
    private string? LoadingClassString => CssBuilder.Default("table-loading")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// 获得/设置 TreeItem 图标
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private static string? GetIconClassString(TreeItem item) => CssBuilder.Default("tree-icon")
        .AddClass(item.Icon)
        .Build();

    /// <summary>
    /// 获得/设置 TreeItem 小箭头样式
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private static string? GetCaretClassString(TreeItem item) => CssBuilder.Default("fa fa-caret-right")
        .AddClass("invisible", !item.HasChildNode && !item.Items.Any())
        .AddClass("fa-rotate-90", !item.IsCollapsed)
        .Build();

    /// <summary>
    /// 获得/设置 当前行样式
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private string? GetItemClassString(TreeItem item) => CssBuilder.Default("tree-item")
        .AddClass("active", ActiveItem == item || DragoverItem == item)
        .Build();

    /// <summary>
    /// 获得/设置 TreeNode 样式
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    private static string? GetTreeNodeClassString(TreeItem item) => CssBuilder.Default("tree-ul")
        .AddClass("show", !item.IsCollapsed)
        .Build();

    /// <summary>
    /// 获得/设置 选中节点 默认 null
    /// </summary>
    [Parameter]
    public TreeItem? ActiveItem { get; set; }

    /// <summary>
    /// 获得/设置 是否为手风琴效果 默认为 false
    /// </summary>
    [Parameter]
    public bool IsAccordion { get; set; }

    /// <summary>
    /// 获得/设置 是否点击节点时展开或者收缩子项 默认 false
    /// </summary>
    [Parameter]
    public bool ClickToggleNode { get; set; }

    /// <summary>
    /// 获得/设置 是否显示加载骨架屏 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowSkeleton { get; set; }

    /// <summary>
    /// 获得/设置 菜单数据集合
    /// </summary>
    [Parameter]
    [NotNull]
    public List<TreeItem>? Items { get; set; }

    /// <summary>
    /// 获得/设置 是否显示 CheckBox 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowCheckbox { get; set; }

    /// <summary>
    /// 获得/设置 是否显示 Radio 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowRadio { get; set; }

    /// <summary>
    /// 获得/设置 是否显示 Icon 图标 默认 false 不显示
    /// </summary>
    [Parameter]
    public bool ShowIcon { get; set; }

    /// <summary>
    /// 获得/设置 树形控件节点点击时回调委托
    /// </summary>
    [Parameter]
    public Func<TreeItem, Task>? OnTreeItemClick { get; set; }

    /// <summary>
    /// 获得/设置 树形控件节点选中时回调委托
    /// </summary>
    [Parameter]
    public Func<List<TreeItem>, Task>? OnTreeItemChecked { get; set; }

    /// <summary>
    /// 获得/设置 节点展开前回调委托
    /// </summary>
    [Parameter]
    public Func<TreeItem, Task>? OnExpandNode { get; set; }

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        GroupName = this.GetHashCode().ToString();
    }

    /// <summary>
    /// OnParametersSet 方法
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        //if (ActiveItem != null)
        //{
        //    var item = ActiveItem;
        //    while (item.Parent != null)
        //    {
        //        item.Parent.IsExpanded = true;
        //        item = item.Parent;
        //    }
        //}
    }

    /// <summary>
    /// OnAfterRenderAsync 方法
    /// </summary>
    /// <param name="firstRender"></param>
    /// <returns></returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await JSRuntime.InvokeVoidAsync(TreeElement, "bb_tree");
        }
    }

    /// <summary>
    /// 选中节点时触发此方法
    /// </summary>
    /// <returns></returns>
    private async Task OnClick(TreeItem item)
    {
        ActiveItem = item;
        if (ClickToggleNode)
        {
            await OnExpandRowAsync(item);
        }

        if (OnTreeItemClick != null)
        {
            await OnTreeItemClick(item);
        }

        if (ShowRadio)
        {
            await OnRadioClick(item);
        }
        else if (ShowCheckbox)
        {
            item.Checked = !item.Checked;
            var status = item.Checked ? CheckboxState.Checked : CheckboxState.UnChecked;
            await OnStateChanged(status, item);
        }
    }

    /// <summary>
    /// 更改节点是否展开方法
    /// </summary>
    /// <param name="item"></param>
    private async Task OnExpandRowAsync(TreeItem item)
    {
        if (IsAccordion)
        {
            foreach (var rootNode in Items.Where(p => !p.IsCollapsed && p != item))
            {
                rootNode.IsCollapsed = true;
            }
        }
        item.IsCollapsed = !item.IsCollapsed;
        if (OnExpandNode != null)
        {
            await OnExpandNode(item);
        }
    }

    /// <summary>
    /// 节点 Checkbox 状态改变时触发此方法
    /// </summary>
    /// <param name="state"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    private async Task OnStateChanged(CheckboxState state, TreeItem item)
    {
        // 向下级联操作
        item.CascadeSetCheck(item.Checked);

        if (OnTreeItemChecked != null)
        {
            await OnTreeItemChecked(GetCheckedItems().ToList());
        }
    }

    /// <summary>
    /// 获得 所有选中节点集合
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TreeItem> GetCheckedItems() => Items.Aggregate(new List<TreeItem>(), (t, item) =>
    {
        t.Add(item);
        t.AddRange(item.GetAllSubItems());
        return t;
    }).Where(i => i.Checked);

    private async Task OnRadioClick(TreeItem item)
    {
        if (ActiveItem != null)
        {
            ActiveItem.Checked = false;
        }
        ActiveItem = item;
        ActiveItem.Checked = true;

        // 其他设置为 false
        if (OnTreeItemChecked != null)
        {
            await OnTreeItemChecked(new List<TreeItem> { item });
        }
    }

    private static CheckboxState CheckState(TreeItem item)
    {
        return item.Checked ? CheckboxState.Checked : CheckboxState.UnChecked;
    }

    /// <summary>
    /// 拖动的Item
    /// </summary>
    private TreeItem? DragItem { get; set; }

    /// <summary>
    /// 拖拽到的Item
    /// </summary>
    private TreeItem? DragoverItem { get; set; }

    /// <summary>
    /// 是否在上面
    /// </summary>
    private bool IsUp { get; set; } = false;

    /// <summary>
    /// 拖拽到空位上
    /// </summary>
    private TreeItem? DragSpace { get; set; }

    /// <summary>
    /// 保存拖拽时展开状态，释放时恢复
    /// </summary>
    private bool DragItemIsCollapsed { get; set; }

    private async Task OnDragStart(TreeItem item)
    {
        DragItemIsCollapsed = item.IsCollapsed;
        // 如果展开了就先收缩起来，防止放在自己的子节点上
        if (!item.IsCollapsed)
        {
            await OnExpandRowAsync(item);
        }

        DragItem = item;
    }

    private void OnDragEnd(TreeItem item)
    {
        item.IsCollapsed = DragItemIsCollapsed;
        DragItem = null;
    }

    private void OnDragEnter(TreeItem item)
    {
        if (DragItem != null && !Equals(DragItem, item))
        {
            DragoverItem = item;
        }
    }

    private void OnDragLeave()
    {
        DragoverItem = null;
    }

    private void OnDrop()
    {
        if (DragItem == null)
        {
            return;
        }
        if (DragoverItem != null)
        {
            if (RemoveTreeItem(Items, DragItem))
            {
                DragoverItem.Items.Add(DragItem);
                StateHasChanged();
            }

            DragoverItem = null;
            DragItem = null;
            return;
        }

        if (DragSpace != null)
        {
            var ret = GetItemIndex(Items, DragSpace);
            if (ret != null)
            {
                var index = IsUp ? ret.Value.index : ret.Value.index + 1;
                // 处理拖动节点与释放在同一个List上的BUG
                if (ret.Value.items.Contains(DragItem))
                {
                    var oldIndex = ret.Value.items.IndexOf(DragItem);
                    if (oldIndex <= index)
                    {
                        index--;
                    }
                }
                if (RemoveTreeItem(Items, DragItem))
                {
                    ret.Value.items.Insert(index, DragItem);
                    DragSpace = null;
                    DragItem = null;
                }

            }
        }
    }

    private (List<TreeItem> items, int index)? GetItemIndex(List<TreeItem> items, TreeItem item)
    {
        var index = items.IndexOf(item);
        if (index >= 0)
        {
            return (items, index);
        }
        var children = items.Where(x => x.Items.Any());
        if (children.Any())
        {
            foreach (var child in children)
            {
                var ret = GetItemIndex(child.Items, item);
                if (ret != null)
                {
                    return ret;
                }
            }
        }
        return null;
    }

    private bool RemoveTreeItem(List<TreeItem> items, TreeItem item)
    {
        if (items.Any(x => x == item))
        {
            items.Remove(item);
            return true;
        }

        var children = items.Where(x => x.Items.Any());
        if (children.Any())
        {
            foreach (var child in children)
            {
                if (RemoveTreeItem(child.Items, item))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDragSpaceEnter(TreeItem item, bool isUp)
    {
        if (DragItem == item)
        {
            return;
        }

        IsUp = isUp;
        DragSpace = item;
        StateHasChanged();
    }
}
