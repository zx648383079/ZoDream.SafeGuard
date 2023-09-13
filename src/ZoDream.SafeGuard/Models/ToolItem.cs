using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.SafeGuard.Models
{
    public class ToolItem
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Icon { get; set; } = string.Empty;

        public string Uri { get; set; } = string.Empty;

        public Type? Target { get; set; }

        public ToolItem()
        {
            
        }

        public ToolItem(string title, string icon)
        {
            Title = title;
            Icon = icon;
        }

        public ToolItem(string title, string icon, string uri): this(title, icon)
        {
            Uri = uri;
        }
    }

    public class ToolGroupItem
    {
        public string Title { get; set; } = string.Empty;

        public List<ToolItem> Items { get; set; } = new();

        public ToolGroupItem()
        {
            
        }

        public ToolGroupItem(string title)
        {
            Title = title;
        }
    }
}
