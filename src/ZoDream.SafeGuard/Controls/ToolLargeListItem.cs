﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoDream.SafeGuard.Models;
using ZoDream.Shared.Routes;
using ZoDream.Shared.Controls;

namespace ZoDream.SafeGuard.Controls
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.SafeGuard.Controls"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根
    /// 元素中:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ZoDream.SafeGuard.Controls;assembly=ZoDream.SafeGuard.Controls"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误:
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:ToolLargeListItem/>
    ///
    /// </summary>
    public class ToolLargeListItem : Control
    {
        static ToolLargeListItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ToolLargeListItem), new FrameworkPropertyMetadata(typeof(ToolLargeListItem)));
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Label {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Label.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(ToolLargeListItem), new PropertyMetadata(null));


        /// <summary>
        /// 字体图标
        /// </summary>
        public string Icon {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(ToolLargeListItem), new PropertyMetadata(null));




        public double IconSize {
            get { return (double)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(ToolLargeListItem), new PropertyMetadata(30.0));



        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, string.IsNullOrWhiteSpace(Label) ? "PointerOver2" : "PointerOver", true);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, "Normal", true);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (DataContext is ToolItem o && !string.IsNullOrWhiteSpace(o.Uri))
            {
                ShellManager.GoToAsync(o.Uri, new Dictionary<string, object>
                {
                    {"tool", o}
                });
            }
        }
    }
}
