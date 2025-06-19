// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License
// See the LICENSE file in the project root for more information.
// Maintainer: Argo Zhang(argo@live.ca) Website: https://www.blazor.zone

using Microsoft.Extensions.Localization;
using System.Data;

namespace UnitTest.Services;

public class AddAutoClearTableDynamicObjectTypeCacheTaskTest
{
    [Fact]
    public void Table_Ok()
    {
        var context = new TestContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddBootstrapBlazor();

        // 注入服务
        context.Services.AddAutoClearTableDynamicObjectTypeCacheTask();

        var localizer = context.Services.GetRequiredService<IStringLocalizer<Foo>>();
        var items = Foo.GenerateFoo(localizer, 2);
        var cut = context.RenderComponent<BootstrapBlazorRoot>(pb =>
        {
            pb.AddChildContent<Table<DynamicObject>>(pb =>
            {
                pb.Add(a => a.DynamicContext, CreateDynamicContext(localizer));
            });
        });

        // 调用清理方法
        var type = Type.GetType("Microsoft.AspNetCore.Components.Reflection.ComponentProperties, Microsoft.AspNetCore.Components");
        Assert.NotNull(type);

        var cacheField = type.GetField("_cachedWritersByType", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Assert.NotNull(cacheField);

        var cacheInstance = cacheField.GetValue(null);
        Assert.NotNull(cacheInstance);

        var countProperty = cacheInstance.GetType().GetProperty("Count", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.NotNull(countProperty);

        var count = countProperty.GetValue(cacheInstance, null);
        Assert.NotNull(count);
    }

    public static DataTableDynamicContext CreateDynamicContext(IStringLocalizer<Foo> localizer)
    {
        var UserData = CreateDataTable(localizer);
        return new DataTableDynamicContext(UserData, (context, col) =>
        {
            var propertyName = col.GetFieldName();
            // 使用 Text 设置显示名称示例
            col.Text = localizer[nameof(Foo.Name)];
        });
    }

    private static DataTable CreateDataTable(IStringLocalizer<Foo> localizer)
    {
        var userData = new DataTable();
        userData.Columns.Add(nameof(Foo.Name), typeof(string));
        userData.Columns.Add("Id", typeof(int));

        var index = 0;
        Foo.GenerateFoo(localizer, 2).ForEach(f =>
        {
            userData.Rows.Add(f.Name, index++);
        });
        return userData;
    }
}
