## AccessControlHelper

[![WeihanLi.AspNetMvc.AccessControlHelper](https://img.shields.io/nuget/v/WeihanLi.AspNetMvc.AccessControlHelper.svg)](http://www.nuget.org/packages/WeihanLi.AspNetMvc.AccessControlHelper/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/WeihanLi.AspNetMvc.AccessControlHelper.svg)](http://www.nuget.org/packages/WeihanLi.AspNetMvc.AccessControlHelper/)

### Build Status

[![Build status](https://ci.appveyor.com/api/projects/status/ht69a1o8b9ss9v8a?svg=true)](https://ci.appveyor.com/project/WeihanLi/accesscontroldemo)

[![Build Status](https://travis-ci.org/WeihanLi/AccessControlHelper.svg?branch=master)](https://travis-ci.org/WeihanLi/AccessControlHelper)

### Intro

由于项目需要，需要在 基于 Asp.net mvc 的 Web 项目框架中做权限的控制，于是才有了这个权限控制组件。

项目基于 .NETStandard，同时支持 asp.net mvc（.NET faremwork4.5以上） 和 asp.net core 项目（asp.net 2.0以上），基于 ASP.NET MVC 和 ASP.NET Core 实现的对 `Action` 的访问控制以及页面元素的权限控制。

### GetStarted

1. Nuget Package <https://www.nuget.org/packages/WeihanLi.AspNetMvc.AccessControlHelper/> 
   
   安装权限控制组件 `WeihanLi.AspNetMvc.AccessControlHelper` 

   asp.net:

   ```
   Install-Package WeihanLi.AspNetMvc.AccessControlHelper
   ```

   asp.net core:

   ```
   dotnet add package WeihanLi.AspNetMvc.AccessControlHelper
   ```

1. 实现自己的权限控制显示策略类

    - 实现页面元素显示策略接口 `IControlAccessStrategy`
    - 实现 `Action` 访问显示策略接口 `IActionAccessStrategy`

    示例代码：
    
    - ASP.NET Mvc
   
         1. [ActionAccessStragety](https://github.com/WeihanLi/AccessControlHelper/blob/master/samples/PowerControlDemo/Helper/AccessStrategy.cs)

    - ASP.NET Core

        1. [ActionAccessStragety](https://github.com/WeihanLi/AccessControlHelper/blob/master/samples/AccessControlDemo/Services/ActionAccessStrategy.cs)

        1. [ControlAccessStrategy ](https://github.com/WeihanLi/AccessControlHelper/blob/master/samples/AccessControlDemo/Services/ControlAccessStrategy.cs)


1. 程序启动时注册自己的显示策略

    - asp.net mvc

    基于Autofac实现的依赖注入，在autofac注册的最后 注册显示策略，参考：<https://github.com/WeihanLi/AccessControlHelper/blob/master/samples/PowerControlDemo/Global.asax.cs#L23>

    ``` csharp
    //autofac ContainerBuilder
    var builder = new ContainerBuilder();
    // etc...

    // register accesss control
    builder.RegisterAccessControlHelper<ActionAccessStrategy, ControlAccessStrategy>();
    ```
    
    - asp.net core

    在 `Startup` 文件中注册显示策略，参考<https://github.com/WeihanLi/AccessControlHelper/blob/master/samples/AccessControlDemo/Startup.cs>

    ``` csharp
    // Configure
    app.UseAccessControlHelper();

    // ConfigureServices
    services.AddAccessControlHelper<ActionAccessStrategy, ControlAccessStrategy>();
    ```
    

1. 控制 `Action` 的方法权限

    通过 `AccessControl` 和 `NoAccessControl` Filter 来控制 `Action` 的访问权限，如果Action上定义了 `NoAccessControl` 可以忽略上级定义的 `AccessControl`，可以设置 Action 对应的 `AccessKey`

    使用示例：
    ``` csharp
    [NoAccessControl]
    public IActionResult Index()
    {
        return View();
    }

    [AccessControl]
    public IActionResult About()
    {
        ViewData["Message"] = "Your application description page.";

        return View();
    }

    [AccessControl(AccessKey = "Contact")]
    public IActionResult Contact()
    {
        ViewData["Message"] = "Your contact page.";

        return View();
    }
    ```

1. 控制页面元素的显示

    通过 `HtmlHelper` 扩展方法来实现权限控制

    - `SparkContainer` 使用

       ``` csharp
       @using(Html.SparkContainer("div",new { @class="container",custom-attribute = "abcd" }))
       {
           @Html.Raw("1234")
       }

       @using (Html.SparkContainer("span",new { @class = "custom_p111" }, "F7A17FF9-3371-4667-B78E-BD11691CA852"))
       {
           @:12344
       }
       ```

       没有权限访问就不会渲染到页面上，有权限访问的时候渲染得到的 Html 如下：

       ``` html
       <div class="container" custom-attribute="abcd">1234</div>

       <span class="custome_p111">12344</span>
       ```


### Contact

如果您在使用中遇到了问题，欢迎随时与我联系。

Contact me: <weihanli@outlook.com>
