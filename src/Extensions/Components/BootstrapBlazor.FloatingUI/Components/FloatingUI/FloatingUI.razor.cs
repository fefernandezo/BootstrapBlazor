// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using Microsoft.AspNetCore.Components;

namespace BootstrapBlazor.Components;

public partial class FloatingUI
{
    private string? ClassString => CssBuilder.Default("bb-float-ui")
        .AddClassFromAttributes(AdditionalAttributes)
        .Build();

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
    /// 子组件实例化悬浮组件方法
    /// </summary>
    /// <param name="element"></param>
    internal void RegisterFloatElement(FloatingElement element) => Element = element;

    private RenderFragment RenderFloatingElement() => builder =>
    {
        builder.OpenElement(0, "div");
        builder.AddAttribute(1, "class", "bb-float-dropdown");
        if (Element != null)
        {
            builder.AddContent(2, Element.ChildContent);
        }
        builder.CloseElement();
    };
}
