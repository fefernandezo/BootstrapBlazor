// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components;

/// <summary>
/// 
/// </summary>
public partial class FloatingUI
{
    private string? ClassString => CssBuilder.Default("bb-float-ui")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

    /// <summary>
    /// 获得/设置 触发方法 默认 hover
    /// </summary>
    [Parameter]
    public string? Trigger { get; set; }

    /// <summary>
    /// 获得/设置 RenderFragment 实例
    /// </summary>
    [Parameter]
#if NET6_0_OR_GREATER
    [EditorRequired]
#endif
    [NotNull]
    public RenderFragment? ChildContent { get; set; }

    private FloatingElement? Element { get; set; }

    private bool IsRendered { get; set; }

    private string? FloatingElementId => Element?.Id;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        Trigger ??= "hover";
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="firstRender"></param>
    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            IsRendered = true;
            StateHasChanged();
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override Task InvokeInitAsync() => InvokeVoidAsync("init", Id, new { Trigger });

    /// <summary>
    /// 子组件实例化悬浮组件方法
    /// </summary>
    /// <param name="element"></param>
    internal void RegisterFloatElement(FloatingElement element) => Element = element;

    private RenderFragment RenderFloatingElement() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "bb-float-dropdown d-none");
        if (Element != null)
        {
            builder.AddAttribute(2, "id", Element.Id);
            builder.AddContent(3, Element.ChildContent);
        }
        builder.CloseElement();
    };
}
