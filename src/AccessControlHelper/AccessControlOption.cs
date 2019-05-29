﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WeihanLi.AspNetMvc.AccessControlHelper
{
    public class AccessControlOption
    {
        public string AccessHeaderKey { get; set; } = "X-Access-Key";

        public Func<HttpContext, Task> DefaultUnauthorizedOperation { get; set; }
    }
}
