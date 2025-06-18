using MudBlazor;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Server.Shared.Component
{
    public class TBreadCrumCustom
    {
        public List<BreadcrumbItem> LItems { get; set; } = new List<BreadcrumbItem>();

        public Action CambiarEstado;

        public void Update(string uri)
        {

            LItems = new List<BreadcrumbItem>();
            LItems.Add(new BreadcrumbItem(text: "", href: "/Home", icon: ""));

            foreach (var item in uri.Split("/"))
            {
                LItems.Add(new BreadcrumbItem(text: CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ToLower()), href: "#", disabled: true));
            }

            CambiarEstado?.Invoke();
        }



    }
}
